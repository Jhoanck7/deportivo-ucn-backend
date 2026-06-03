
using DeportivoUCN.Models.Entities;

namespace DeportivoUCN.Application.Interfaces;

public interface ISportBranchRepository
{
    Task<IEnumerable<SportBranch>> GetAllAsync();
    Task<SportBranch?> GetByIdAsync(int id);
    Task<SportBranch> AddAsync(SportBranch sportBranch);
    Task UpdateAsync(SportBranch sportBranch);
    Task DeleteAsync(int id);
}