using DeportivoUCN.Models.Entities;

namespace DeportivoUCN.Application.Interfaces;

public interface ICourtRepository
{
    Task<IEnumerable<Court>> GetAllAsync();
    Task<Court?> GetByIdAsync(int id);
    Task<Court> AddAsync(Court court);
    Task UpdateAsync(Court court);
    Task DeleteAsync(int id);
}
