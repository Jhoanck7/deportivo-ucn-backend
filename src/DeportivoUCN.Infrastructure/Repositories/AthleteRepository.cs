using DeportivoUCN.Application.Interfaces;
using DeportivoUCN.Infrastructure.Data;
using DeportivoUCN.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace DeportivoUCN.Infrastructure.Repositories;

public class AthleteRepository(DeportivoUCNContext context) : IAthleteRepository
{
    public async Task<IEnumerable<Athlete>> GetAllAsync()
    {
        return await context.Athletes
            .Include(a => a.SportBranch)
            .ToListAsync();
    }

    public async Task<Athlete?> GetByIdAsync(int id)
    {
        return await context.Athletes
            .Include(a => a.SportBranch)
            .FirstOrDefaultAsync(a => a.Id == id);
    }

    public async Task<Athlete> AddAsync(Athlete athlete)
    {
        await context.Athletes.AddAsync(athlete);
        await context.SaveChangesAsync();
        return athlete;
    }

    public async Task UpdateAsync(Athlete athlete)
    {
        context.Athletes.Update(athlete);
        await context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var athlete = await context.Athletes.FindAsync(id);
        if (athlete != null)
        {
            context.Athletes.Remove(athlete);
            await context.SaveChangesAsync();
        }
    }

    public async Task<int> GetAthleteCountByBranchIdAsync(int branchId)
    {
        return await context.Athletes.CountAsync(a => a.SportBranchId == branchId && a.IsActive);
    }
}
