using DeportivoUCN.Application.Interfaces;
using DeportivoUCN.Infrastructure.Data;
using DeportivoUCN.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace DeportivoUCN.Infrastructure.Repositories;

public class UserRepository(DeportivoUCNContext context) : IUserRepository
{
    public async Task<IEnumerable<User>> GetAllAsync()
    {
        return await context.Users.ToListAsync();
    }

    public async Task<User?> GetByIdAsync(int id)
    {
        return await context.Users.FindAsync(id);
    }

    public async Task<User?> GetByEmailAsync(string email)
    {
        return await context.Users.FirstOrDefaultAsync(u => u.Email == email);
    }

    public async Task<User?> GetByRutAsync(string rut)
    {
        return await context.Users.FirstOrDefaultAsync(u => u.Rut == rut);
    }

    public async Task<User> AddAsync(User user)
    {
        await context.Users.AddAsync(user);
        await context.SaveChangesAsync();
        return user;
    }

    public async Task UpdateAsync(User user)
    {
        context.Users.Update(user);
        await context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var user = await context.Users.FindAsync(id);
        if (user != null)
        {
            context.Users.Remove(user);
            await context.SaveChangesAsync();
        }
    }
}
