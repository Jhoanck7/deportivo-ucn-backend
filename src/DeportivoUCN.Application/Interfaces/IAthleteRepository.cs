using DeportivoUCN.Models.Entities;

namespace DeportivoUCN.Application.Interfaces;

public interface IAthleteRepository
{
    Task<IEnumerable<Athlete>> GetAllAsync();
    Task<Athlete?> GetByIdAsync(int id);
    Task<Athlete> AddAsync(Athlete athlete);
    Task UpdateAsync(Athlete athlete);
    Task DeleteAsync(int id);
    Task<int> GetAthleteCountByBranchIdAsync(int branchId);
}
