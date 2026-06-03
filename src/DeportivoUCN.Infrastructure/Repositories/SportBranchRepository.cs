using DeportivoUCN.Application.Interfaces;
using DeportivoUCN.Infrastructure.Data;
using DeportivoUCN.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace DeportivoUCN.Infrastructure.Repositories;

public class SportBranchRepository(DeportivoUCNContext context) : ISportBranchRepository
{
    public async Task<IEnumerable<SportBranch>> GetAllAsync()
    {
        return await context.SportBranches
            .Include(sb => sb.Coach) 
            .ToListAsync();
    }

    public async Task<SportBranch?> GetByIdAsync(int id)
    {
        return await context.SportBranches
            .Include(sb => sb.Coach)
            .FirstOrDefaultAsync(sb => sb.Id == id);
    }

    public async Task<SportBranch> AddAsync(SportBranch sportBranch)
    {
        await context.SportBranches.AddAsync(sportBranch);
        await context.SaveChangesAsync();
        return sportBranch;
    }

    public async Task UpdateAsync(SportBranch sportBranch)
    {
        context.SportBranches.Update(sportBranch);
        await context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var sportBranch = await context.SportBranches.FindAsync(id);
        if (sportBranch != null)
        {
            context.SportBranches.Remove(sportBranch);
            await context.SaveChangesAsync();
        }
    }
}