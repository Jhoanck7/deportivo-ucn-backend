using DeportivoUCN.Application.DTO;
using DeportivoUCN.Application.DTO.Coach;
using DeportivoUCN.Application.Interfaces;
using DeportivoUCN.Models.Entities;

namespace DeportivoUCN.Application.Services;

public class CoachService(ICoachRepository repository) : ICoachService
{
    public async Task<GenericResponse<IEnumerable<CoachResponseDto>>> GetAllCoachesAsync()
    {
        var coaches = await repository.GetAllAsync();
        
        var dtos = coaches.Select(c => new CoachResponseDto
        {
            Id = c.Id,
            FirstName = c.FirstName,
            LastName = c.LastName,
            Email = c.Email
        });

        return new GenericResponse<IEnumerable<CoachResponseDto>>("Coaches retrieved successfully", dtos);
    }

    public async Task<GenericResponse<CoachResponseDto>> GetCoachByIdAsync(int id)
    {
        var coach = await repository.GetByIdAsync(id);
        if (coach == null)
            throw new KeyNotFoundException($"Coach with ID {id} not found");

        var dto = new CoachResponseDto
        {
            Id = coach.Id,
            FirstName = coach.FirstName,
            LastName = coach.LastName,
            Email = coach.Email
        };

        return new GenericResponse<CoachResponseDto>("Coach retrieved successfully", dto);
    }

    public async Task<GenericResponse<CoachResponseDto>> CreateCoachAsync(CoachRequestDto request)
    {
        var newCoach = new Coach
        {
            FirstName = request.FirstName,
            LastName = request.LastName,
            Email = request.Email
        };

        var createdCoach = await repository.AddAsync(newCoach);

        var dto = new CoachResponseDto
        {
            Id = createdCoach.Id,
            FirstName = createdCoach.FirstName,
            LastName = createdCoach.LastName,
            Email = createdCoach.Email
        };

        return new GenericResponse<CoachResponseDto>("Coach created successfully", dto);
    }

    public async Task<GenericResponse<bool>> UpdateCoachAsync(int id, CoachRequestDto request)
    {
        var coach = await repository.GetByIdAsync(id);
        if (coach == null)
            throw new KeyNotFoundException($"Coach with ID {id} not found");

        coach.FirstName = request.FirstName;
        coach.LastName = request.LastName;
        coach.Email = request.Email;

        await repository.UpdateAsync(coach);

        return new GenericResponse<bool>("Coach updated successfully", true);
    }

    public async Task<GenericResponse<bool>> DeleteCoachAsync(int id)
    {
        var coach = await repository.GetByIdAsync(id);
        if (coach == null)
            throw new KeyNotFoundException($"Coach with ID {id} not found");

        await repository.DeleteAsync(id);

        return new GenericResponse<bool>("Coach deleted successfully", true);
    }
}