using DeportivoUCN.Application.DTO;
using DeportivoUCN.Application.DTO.Booking;
using DeportivoUCN.Application.Interfaces;
using DeportivoUCN.Models.Entities;
using DeportivoUCN.Models.Enums;

namespace DeportivoUCN.Application.Services;

public class BookingService(
    IBookingRepository repository,
    ICourtRepository courtRepository,
    IUserRepository userRepository,
    IEmailService emailService) : IBookingService
{
    public async Task<GenericResponse<IEnumerable<BookingResponseDto>>> GetAllBookingsAsync()
    {
        var bookings = await repository.GetAllAsync();
        var dtos = bookings.Select(b => MapToResponseDto(b));
        return new GenericResponse<IEnumerable<BookingResponseDto>>("Bookings retrieved successfully", dtos);
    }

    public async Task<GenericResponse<BookingResponseDto>> GetBookingByIdAsync(int id)
    {
        var booking = await repository.GetByIdAsync(id);
        if (booking == null)
            throw new KeyNotFoundException($"Booking with ID {id} not found");

        return new GenericResponse<BookingResponseDto>("Booking retrieved successfully", MapToResponseDto(booking));
    }

    public async Task<GenericResponse<BookingResponseDto>> CreateBookingAsync(int userId, BookingRequestDto request)
    {
        var user = await userRepository.GetByIdAsync(userId);
        if (user == null)
            throw new KeyNotFoundException($"User with ID {userId} not found");

        if (user.IsBanned)
            throw new InvalidOperationException($"No se permite realizar reservas. Usuario penalizado: {user.BanReason}");

        var court = await courtRepository.GetByIdAsync(request.CourtId);
        if (court == null)
            throw new KeyNotFoundException($"Court with ID {request.CourtId} not found");

        if (court.Status != CourtStatus.Available)
            throw new InvalidOperationException($"La cancha {court.Name} no está habilitada para arriendos en este momento");

        var dayOfWeek = request.Date.ToDateTime(TimeOnly.MinValue).DayOfWeek;
        if (dayOfWeek == DayOfWeek.Saturday || dayOfWeek == DayOfWeek.Sunday)
            throw new ArgumentException("Las reservas de canchas solo están permitidas de lunes a viernes.");

        if (request.StartHour < 8 || request.StartHour >= 22)
            throw new ArgumentOutOfRangeException("Las reservas deben ser en bloques de 1 hora entre las 08:00 y las 22:00.");

        var minDeposit = court.PricePerHour * 0.5m;
        if (request.DepositAmount < minDeposit)
            throw new ArgumentException($"El monto de abono mínimo para reservar es ${minDeposit} (50% del valor por hora: ${court.PricePerHour}).");

        var hasConflict = await repository.HasBookingConflictAsync(request.CourtId, request.Date, request.StartHour);
        if (hasConflict)
            throw new InvalidOperationException("El bloque horario seleccionado ya está reservado por otro usuario.");

        var booking = new Booking
        {
            CourtId = request.CourtId,
            UserId = userId,
            Date = request.Date,
            StartHour = request.StartHour,
            EndHour = request.StartHour + 1,
            Status = BookingStatus.Pending,
            DepositAmount = request.DepositAmount
        };

        var created = await repository.AddAsync(booking);
        
        var fetched = await repository.GetByIdAsync(created.Id);
        if (fetched == null)
            throw new InvalidOperationException("Error al recuperar la reserva recién creada.");

        var subject = $"Confirmación de Solicitud de Arriendo - Deportivo UCN";
        var body = $"Hola {user.FirstName} {user.LastName},\n\n" +
                   $"Hemos recibido tu solicitud de arriendo de cancha en Deportivo UCN con los siguientes detalles:\n\n" +
                   $"• Cancha: {court.Name}\n" +
                   $"• Fecha: {request.Date:dd/MM/yyyy}\n" +
                   $"• Horario: {request.StartHour:00}:00 - {booking.EndHour:00}:00 hrs\n" +
                   $"• Monto abonado: ${request.DepositAmount}\n" +
                   $"• Monto restante a pagar: ${court.PricePerHour - request.DepositAmount}\n" +
                   $"• Estado: Pendiente de aprobación por administración\n\n" +
                   $"Un administrador revisará tu solicitud a la brevedad. Recibirás una notificación por correo una vez que sea procesada.";
        
        await emailService.SendEmailAsync(user.Email, subject, body);

        return new GenericResponse<BookingResponseDto>("Booking registered successfully", MapToResponseDto(fetched));
    }

    public async Task<GenericResponse<bool>> UpdateBookingStatusAsync(int id, string statusStr, string? adminNotes = null)
    {
        var booking = await repository.GetByIdAsync(id);
        if (booking == null)
            throw new KeyNotFoundException($"Booking with ID {id} not found");

        if (!Enum.TryParse<BookingStatus>(statusStr, true, out var status))
            throw new ArgumentException($"Estado de reserva inválido: {statusStr}");

        booking.Status = status;
        if (adminNotes != null)
        {
            booking.AdminNotes = adminNotes;
        }

        await repository.UpdateAsync(booking);

        if (booking.User != null && booking.Court != null)
        {
            string subject;
            string body;

            switch (status)
            {
                case BookingStatus.Confirmed:
                    subject = "¡Solicitud Aprobada! Confirmación de Arriendo - Deportivo UCN";
                    body = $"Hola {booking.User.FirstName} {booking.User.LastName},\n\n" +
                           $"¡Excelentes noticias! Tu solicitud de arriendo de cancha ha sido ACEPTADA por la administración.\n\n" +
                           $"Detalles del Arriendo:\n" +
                           $"• Cancha: {booking.Court.Name}\n" +
                           $"• Fecha: {booking.Date:dd/MM/yyyy}\n" +
                           $"• Horario: {booking.StartHour:00}:00 - {booking.EndHour:00}:00 hrs\n" +
                           $"• Estado: ACEPTADA / CONFIRMADA\n" +
                           $"• Observaciones del administrador: {booking.AdminNotes ?? "Ninguna"}\n\n" +
                           $"Te esperamos en el recinto deportivo a la hora agendada. ¡Gracias por usar Deportivo UCN!";
                    break;

                case BookingStatus.Cancelled:
                    subject = "Solicitud Rechazada / Cancelada - Deportivo UCN";
                    body = $"Hola {booking.User.FirstName} {booking.User.LastName},\n\n" +
                           $"Te informamos que tu solicitud de arriendo para la cancha '{booking.Court.Name}' el día {booking.Date:dd/MM/yyyy} de {booking.StartHour:00}:00 a {booking.EndHour:00}:00 hrs ha sido RECHAZADA / CANCELADA por la administración.\n\n" +
                           $"Motivo / Observaciones del administrador:\n" +
                           $"{booking.AdminNotes ?? "Sin observaciones especificadas."}\n\n" +
                           $"Si tienes preguntas o deseas realizar otra reserva, comunícate con la administración de Deportivo UCN.";
                    break;

                default:
                    var statusName = status switch
                    {
                        BookingStatus.Completed => "Completada",
                        BookingStatus.NoShow => "No Asistió (Penalizado)",
                        _ => status.ToString()
                    };
                    subject = "Actualización de Estado de Reserva - Deportivo UCN";
                    body = $"Hola {booking.User.FirstName} {booking.User.LastName},\n\n" +
                           $"El estado de tu reserva para la cancha '{booking.Court.Name}' el {booking.Date:dd/MM/yyyy} a las {booking.StartHour:00}:00 hrs ha cambiado a: {statusName}.\n\n" +
                           $"Notas del administrador: {booking.AdminNotes ?? "Ninguna"}";
                    break;
            }

            await emailService.SendEmailAsync(booking.User.Email, subject, body);
        }

        return new GenericResponse<bool>("Booking status updated successfully", true);
    }

    public async Task<GenericResponse<IEnumerable<BookingAvailabilityDto>>> GetCourtAvailabilityAsync(int courtId, DateOnly date)
    {
        var court = await courtRepository.GetByIdAsync(courtId);
        if (court == null)
            throw new KeyNotFoundException($"Court with ID {courtId} not found");

        var bookings = await repository.GetBookingsByCourtAndDateAsync(courtId, date);
        var bookedHours = bookings.Select(b => b.StartHour).ToHashSet();

        var availability = new List<BookingAvailabilityDto>();

        var dayOfWeek = date.ToDateTime(TimeOnly.MinValue).DayOfWeek;
        bool isWeekend = dayOfWeek == DayOfWeek.Saturday || dayOfWeek == DayOfWeek.Sunday;

        for (int hour = 8; hour < 22; hour++)
        {
            availability.Add(new BookingAvailabilityDto
            {
                StartHour = hour,
                EndHour = hour + 1,
                IsAvailable = !isWeekend && court.Status == CourtStatus.Available && !bookedHours.Contains(hour)
            });
        }

        return new GenericResponse<IEnumerable<BookingAvailabilityDto>>("Availability retrieved successfully", availability);
    }

    private static BookingResponseDto MapToResponseDto(Booking b)
    {
        var userFullName = b.User != null ? $"{b.User.FirstName} {b.User.LastName}" : "Unknown";
        var userPhone = b.User?.Phone ?? string.Empty;
        var courtName = b.Court?.Name ?? "Unknown";
        var totalPrice = b.Court?.PricePerHour ?? 0m;

        var cleanPhone = userPhone.Replace(" ", "").Replace("+", "");
        if (cleanPhone.Length == 9 && cleanPhone.StartsWith("9"))
        {
            cleanPhone = "56" + cleanPhone;
        }
        
        var message = $"Hola {b.User?.FirstName}, te contacto de Deportivo UCN para confirmar tu arriendo de la cancha {courtName} el día {b.Date:dd/MM/yyyy} a las {b.StartHour:00}:00 hrs.";
        var encodedMsg = Uri.EscapeDataString(message);
        var waLink = $"https://wa.me/{cleanPhone}?text={encodedMsg}";

        return new BookingResponseDto
        {
            Id = b.Id,
            CourtId = b.CourtId,
            CourtName = courtName,
            UserId = b.UserId,
            UserFullName = userFullName,
            UserPhone = userPhone,
            Date = b.Date,
            StartHour = b.StartHour,
            EndHour = b.EndHour,
            Status = b.Status.ToString(),
            DepositAmount = b.DepositAmount,
            TotalPrice = totalPrice,
            WhatsAppLink = waLink,
            AdminNotes = b.AdminNotes,
            CreatedAt = b.CreatedAt
        };
    }
}
