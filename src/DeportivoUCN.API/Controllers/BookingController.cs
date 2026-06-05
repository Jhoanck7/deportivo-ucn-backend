using System.Security.Claims;
using DeportivoUCN.Application.DTO;
using DeportivoUCN.Application.DTO.Booking;
using DeportivoUCN.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DeportivoUCN.API.Controllers;

[ApiController]
[Route("api/bookings")]
public class BookingController(IBookingService bookingService) : ControllerBase
{
    [HttpGet]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<GenericResponse<IEnumerable<BookingResponseDto>>>> GetAll()
    {
        var response = await bookingService.GetAllBookingsAsync();
        return Ok(response);
    }

    [HttpGet("{id:int}")]
    [Authorize]
    public async Task<ActionResult<GenericResponse<BookingResponseDto>>> GetById(int id)
    {
        var response = await bookingService.GetBookingByIdAsync(id);
        return Ok(response);
    }

    [HttpPost]
    [Authorize]
    public async Task<ActionResult<GenericResponse<BookingResponseDto>>> Create([FromBody] BookingRequestDto request)
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int userId))
        {
            return Unauthorized(new GenericResponse<object>("No se pudo identificar al usuario autenticado."));
        }

        var response = await bookingService.CreateBookingAsync(userId, request);
        return CreatedAtAction(nameof(GetById), new { id = response.Data?.Id }, response);
    }

    [HttpPut("{id:int}/status")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<GenericResponse<bool>>> UpdateStatus(int id, [FromQuery] string status, [FromQuery] string? notes = null)
    {
        var response = await bookingService.UpdateBookingStatusAsync(id, status, notes);
        return Ok(response);
    }

    [HttpGet("availability")]
    public async Task<ActionResult<GenericResponse<IEnumerable<BookingAvailabilityDto>>>> GetAvailability([FromQuery] int courtId, [FromQuery] string date)
    {
        if (!DateOnly.TryParse(date, out var parsedDate))
        {
            return BadRequest(new GenericResponse<object>("Formato de fecha inválido (use YYYY-MM-DD)."));
        }

        var response = await bookingService.GetCourtAvailabilityAsync(courtId, parsedDate);
        return Ok(response);
    }
}
