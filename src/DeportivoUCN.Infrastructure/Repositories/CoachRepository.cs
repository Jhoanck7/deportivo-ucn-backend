using DeportivoUCN.Application.Interfaces;
using DeportivoUCN.Infrastructure.Data;
using DeportivoUCN.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace DeportivoUCN.Infrastructure.Repositories;

public class CoachRepository(DeportivoUCNContext context) : ICoachRepository
{
    public async Task<IEnumerable<Coach>> GetAllAsync()
    {
        return await context.Coaches.ToListAsync();
    }

    public async Task<Coach?> GetByIdAsync(int id)
    {
        return await context.Coaches.FindAsync(id);
    }

    public async Task<Coach> AddAsync(Coach coach)
    {
        await context.Coaches.AddAsync(coach);
        await context.SaveChangesAsync();
        return coach;
    }

    public async Task UpdateAsync(Coach coach)
    {
        context.Coaches.Update(coach);
        await context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var coach = await context.Coaches.FindAsync(id);
        if (coach != null)
        {
            context.Coaches.Remove(coach);
            await context.SaveChangesAsync();
        }
    }
}