using DeportivoUCN.Application.Interfaces;
using DeportivoUCN.Infrastructure.Data;
using DeportivoUCN.Models.Entities;
using DeportivoUCN.Models.Enums;
using Microsoft.EntityFrameworkCore;

namespace DeportivoUCN.Infrastructure.Repositories;

public class BookingRepository(DeportivoUCNContext context) : IBookingRepository
{
    public async Task<IEnumerable<Booking>> GetAllAsync()
    {
        return await context.Bookings
            .Include(b => b.Court)
            .Include(b => b.User)
            .ToListAsync();
    }

    public async Task<Booking?> GetByIdAsync(int id)
    {
        return await context.Bookings
            .Include(b => b.Court)
            .Include(b => b.User)
            .FirstOrDefaultAsync(b => b.Id == id);
    }

    public async Task<Booking> AddAsync(Booking booking)
    {
        await context.Bookings.AddAsync(booking);
        await context.SaveChangesAsync();
        return booking;
    }

    public async Task UpdateAsync(Booking booking)
    {
        context.Bookings.Update(booking);
        await context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var booking = await context.Bookings.FindAsync(id);
        if (booking != null)
        {
            context.Bookings.Remove(booking);
            await context.SaveChangesAsync();
        }
    }

    public async Task<IEnumerable<Booking>> GetBookingsByCourtAndDateAsync(int courtId, DateOnly date)
    {
        return await context.Bookings
            .Where(b => b.CourtId == courtId && b.Date == date && b.Status != BookingStatus.Cancelled)
            .ToListAsync();
    }

    public async Task<bool> HasBookingConflictAsync(int courtId, DateOnly date, int startHour)
    {
        return await context.Bookings
            .AnyAsync(b => b.CourtId == courtId 
                        && b.Date == date 
                        && b.StartHour == startHour 
                        && b.Status != BookingStatus.Cancelled);
    }
}
