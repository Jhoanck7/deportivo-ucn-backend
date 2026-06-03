using DeportivoUCN.Application.DTO;
using DeportivoUCN.Application.DTO.SportBranch;
using DeportivoUCN.Application.Interfaces;
using DeportivoUCN.Models.Entities;

namespace DeportivoUCN.Application.Services;

public class SportBranchService(ISportBranchRepository repository) : ISportBranchService
{
    public async Task<GenericResponse<IEnumerable<SportBranchResponseDto>>> GetAllBranchesAsync()
    {
        var branches = await repository.GetAllAsync();
        
        var dtos = branches.Select(b => new SportBranchResponseDto
        {
            Id = b.Id,
            Name = b.Name,
            TrainingDays = b.TrainingDays,
            TrainingHours = b.TrainingHours,
            TrainingSector = b.TrainingSector,
            AthleteLimit = b.AthleteLimit,
            CoachId = b.CoachId
        });

        return new GenericResponse<IEnumerable<SportBranchResponseDto>>("Sport branches retrieved successfully", dtos);
    }

    public async Task<GenericResponse<SportBranchResponseDto>> GetBranchByIdAsync(int id)
    {
        var branch = await repository.GetByIdAsync(id);
        if (branch == null)
            throw new KeyNotFoundException($"Sport branch with ID {id} not found");

        var dto = new SportBranchResponseDto
        {
            Id = branch.Id,
            Name = branch.Name,
            TrainingDays = branch.TrainingDays,
            TrainingHours = branch.TrainingHours,
            TrainingSector = branch.TrainingSector,
            AthleteLimit = branch.AthleteLimit,
            CoachId = branch.CoachId
        };

        return new GenericResponse<SportBranchResponseDto>("Sport branch retrieved successfully", dto);
    }

    public async Task<GenericResponse<SportBranchResponseDto>> CreateBranchAsync(SportBranchRequestDto request)
    {
        var newBranch = new SportBranch
        {
            Name = request.Name,
            TrainingDays = request.TrainingDays,
            TrainingHours = request.TrainingHours,
            TrainingSector = request.TrainingSector,
            AthleteLimit = request.AthleteLimit,
            CoachId = request.CoachId
        };

        var createdBranch = await repository.AddAsync(newBranch);

        var dto = new SportBranchResponseDto
        {
            Id = createdBranch.Id,
            Name = createdBranch.Name,
            TrainingDays = createdBranch.TrainingDays,
            TrainingHours = createdBranch.TrainingHours,
            TrainingSector = createdBranch.TrainingSector,
            AthleteLimit = createdBranch.AthleteLimit,
            CoachId = createdBranch.CoachId
        };

        return new GenericResponse<SportBranchResponseDto>("Sport branch created successfully", dto);
    }

    public async Task<GenericResponse<bool>> UpdateBranchAsync(int id, SportBranchRequestDto request)
    {
        var branch = await repository.GetByIdAsync(id);
        if (branch == null)
            throw new KeyNotFoundException($"Sport branch with ID {id} not found");

        branch.Name = request.Name;
        branch.TrainingDays = request.TrainingDays;
        branch.TrainingHours = request.TrainingHours;
        branch.TrainingSector = request.TrainingSector;
        branch.AthleteLimit = request.AthleteLimit;
        branch.CoachId = request.CoachId;

        await repository.UpdateAsync(branch);

        return new GenericResponse<bool>("Sport branch updated successfully", true);
    }

    public async Task<GenericResponse<bool>> DeleteBranchAsync(int id)
    {
        var branch = await repository.GetByIdAsync(id);
        if (branch == null)
            throw new KeyNotFoundException($"Sport branch with ID {id} not found");

        await repository.DeleteAsync(id);

        return new GenericResponse<bool>("Sport branch deleted successfully", true);
    }
}