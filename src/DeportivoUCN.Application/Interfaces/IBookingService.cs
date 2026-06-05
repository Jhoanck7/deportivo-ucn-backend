using DeportivoUCN.Application.DTO;
using DeportivoUCN.Application.DTO.Booking;

namespace DeportivoUCN.Application.Interfaces;

public interface IBookingService
{
    Task<GenericResponse<IEnumerable<BookingResponseDto>>> GetAllBookingsAsync();
    Task<GenericResponse<BookingResponseDto>> GetBookingByIdAsync(int id);
    Task<GenericResponse<BookingResponseDto>> CreateBookingAsync(int userId, BookingRequestDto request);
    Task<GenericResponse<bool>> UpdateBookingStatusAsync(int id, string status, string? adminNotes = null);
    Task<GenericResponse<IEnumerable<BookingAvailabilityDto>>> GetCourtAvailabilityAsync(int courtId, DateOnly date);
}
