using DeportivoUCN.Application.DTO;
using DeportivoUCN.Application.DTO.Athlete;
using DeportivoUCN.Application.Interfaces;
using DeportivoUCN.Models.Entities;

namespace DeportivoUCN.Application.Services;

public class AthleteService(
    IAthleteRepository repository, 
    ISportBranchRepository branchRepository) : IAthleteService
{
    public async Task<GenericResponse<IEnumerable<AthleteResponseDto>>> GetAllAthletesAsync()
    {
        var athletes = await repository.GetAllAsync();
        var dtos = athletes.Select(a => new AthleteResponseDto
        {
            Id = a.Id,
            FirstName = a.FirstName,
            LastName = a.LastName,
            Rut = a.Rut,
            Email = a.Email,
            Phone = a.Phone,
            BirthDate = a.BirthDate,
            IsActive = a.IsActive,
            SportBranchId = a.SportBranchId,
            SportBranchName = a.SportBranch?.Name
        });

        return new GenericResponse<IEnumerable<AthleteResponseDto>>("Athletes retrieved successfully", dtos);
    }

    public async Task<GenericResponse<AthleteResponseDto>> GetAthleteByIdAsync(int id)
    {
        var athlete = await repository.GetByIdAsync(id);
        if (athlete == null)
            throw new KeyNotFoundException($"Athlete with ID {id} not found");

        var dto = new AthleteResponseDto
        {
            Id = athlete.Id,
            FirstName = athlete.FirstName,
            LastName = athlete.LastName,
            Rut = athlete.Rut,
            Email = athlete.Email,
            Phone = athlete.Phone,
            BirthDate = athlete.BirthDate,
            IsActive = athlete.IsActive,
            SportBranchId = athlete.SportBranchId,
            SportBranchName = athlete.SportBranch?.Name
        };

        return new GenericResponse<AthleteResponseDto>("Athlete retrieved successfully", dto);
    }

    public async Task<GenericResponse<AthleteResponseDto>> CreateAthleteAsync(AthleteRequestDto request)
    {
        if (request.SportBranchId.HasValue)
        {
            var branch = await branchRepository.GetByIdAsync(request.SportBranchId.Value);
            if (branch == null)
                throw new KeyNotFoundException($"Sport branch with ID {request.SportBranchId.Value} not found");

            var count = await repository.GetAthleteCountByBranchIdAsync(request.SportBranchId.Value);
            if (count >= branch.AthleteLimit)
                throw new InvalidOperationException($"Límite de deportistas alcanzado para la rama: {branch.Name} (Límite: {branch.AthleteLimit})");
        }

        var athlete = new Athlete
        {
            FirstName = request.FirstName,
            LastName = request.LastName,
            Rut = request.Rut,
            Email = request.Email,
            Phone = request.Phone,
            BirthDate = request.BirthDate,
            IsActive = request.IsActive,
            SportBranchId = request.SportBranchId
        };

        var created = await repository.AddAsync(athlete);

        var fetched = await repository.GetByIdAsync(created.Id);

        var dto = new AthleteResponseDto
        {
            Id = created.Id,
            FirstName = created.FirstName,
            LastName = created.LastName,
            Rut = created.Rut,
            Email = created.Email,
            Phone = created.Phone,
            BirthDate = created.BirthDate,
            IsActive = created.IsActive,
            SportBranchId = created.SportBranchId,
            SportBranchName = fetched?.SportBranch?.Name
        };

        return new GenericResponse<AthleteResponseDto>("Athlete created successfully", dto);
    }

    public async Task<GenericResponse<bool>> UpdateAthleteAsync(int id, AthleteRequestDto request)
    {
        var athlete = await repository.GetByIdAsync(id);
        if (athlete == null)
            throw new KeyNotFoundException($"Athlete with ID {id} not found");

        if (request.SportBranchId.HasValue && request.SportBranchId != athlete.SportBranchId)
        {
            var branch = await branchRepository.GetByIdAsync(request.SportBranchId.Value);
            if (branch == null)
                throw new KeyNotFoundException($"Sport branch with ID {request.SportBranchId.Value} not found");

            var count = await repository.GetAthleteCountByBranchIdAsync(request.SportBranchId.Value);
            if (count >= branch.AthleteLimit)
                throw new InvalidOperationException($"Límite de deportistas alcanzado para la rama: {branch.Name} (Límite: {branch.AthleteLimit})");
        }

        athlete.FirstName = request.FirstName;
        athlete.LastName = request.LastName;
        athlete.Rut = request.Rut;
        athlete.Email = request.Email;
        athlete.Phone = request.Phone;
        athlete.BirthDate = request.BirthDate;
        athlete.IsActive = request.IsActive;
        athlete.SportBranchId = request.SportBranchId;

        await repository.UpdateAsync(athlete);

        return new GenericResponse<bool>("Athlete updated successfully", true);
    }

    public async Task<GenericResponse<bool>> DeleteAthleteAsync(int id)
    {
        var athlete = await repository.GetByIdAsync(id);
        if (athlete == null)
            throw new KeyNotFoundException($"Athlete with ID {id} not found");

        await repository.DeleteAsync(id);

        return new GenericResponse<bool>("Athlete deleted successfully", true);
    }
}
