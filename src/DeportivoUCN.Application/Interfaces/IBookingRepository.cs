using DeportivoUCN.Models.Entities;

namespace DeportivoUCN.Application.Interfaces;

public interface IBookingRepository
{
    Task<IEnumerable<Booking>> GetAllAsync();
    Task<Booking?> GetByIdAsync(int id);
    Task<Booking> AddAsync(Booking booking);
    Task UpdateAsync(Booking booking);
    Task DeleteAsync(int id);
    Task<IEnumerable<Booking>> GetBookingsByCourtAndDateAsync(int courtId, DateOnly date);
    Task<bool> HasBookingConflictAsync(int courtId, DateOnly date, int startHour);
}
