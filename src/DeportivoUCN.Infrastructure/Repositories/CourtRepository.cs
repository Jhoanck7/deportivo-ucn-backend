using DeportivoUCN.Application.Interfaces;
using DeportivoUCN.Infrastructure.Data;
using DeportivoUCN.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace DeportivoUCN.Infrastructure.Repositories;

public class CourtRepository(DeportivoUCNContext context) : ICourtRepository
{
    public async Task<IEnumerable<Court>> GetAllAsync()
    {
        return await context.Courts.ToListAsync();
    }

    public async Task<Court?> GetByIdAsync(int id)
    {
        return await context.Courts.FindAsync(id);
    }

    public async Task<Court> AddAsync(Court court)
    {
        await context.Courts.AddAsync(court);
        await context.SaveChangesAsync();
        return court;
    }

    public async Task UpdateAsync(Court court)
    {
        context.Courts.Update(court);
        await context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var court = await context.Courts.FindAsync(id);
        if (court != null)
        {
            context.Courts.Remove(court);
            await context.SaveChangesAsync();
        }
    }
}
