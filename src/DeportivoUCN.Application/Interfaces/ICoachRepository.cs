using DeportivoUCN.Models.Entities;

namespace DeportivoUCN.Application.Interfaces;

public interface ICoachRepository
{
    Task<IEnumerable<Coach>> GetAllAsync();
    Task<Coach?> GetByIdAsync(int id);
    Task<Coach> AddAsync(Coach coach);
    Task UpdateAsync(Coach coach);
    Task DeleteAsync(int id);
}