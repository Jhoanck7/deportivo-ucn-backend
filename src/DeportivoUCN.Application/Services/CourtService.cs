using DeportivoUCN.Application.DTO;
using DeportivoUCN.Application.DTO.Court;
using DeportivoUCN.Application.Interfaces;
using DeportivoUCN.Models.Entities;
using DeportivoUCN.Models.Enums;

namespace DeportivoUCN.Application.Services;

public class CourtService(ICourtRepository repository) : ICourtService
{
    public async Task<GenericResponse<IEnumerable<CourtResponseDto>>> GetAllCourtsAsync()
    {
        var courts = await repository.GetAllAsync();
        var dtos = courts.Select(c => new CourtResponseDto
        {
            Id = c.Id,
            Name = c.Name,
            Description = c.Description,
            Status = c.Status.ToString(),
            PricePerHour = c.PricePerHour
        });

        return new GenericResponse<IEnumerable<CourtResponseDto>>("Courts retrieved successfully", dtos);
    }

    public async Task<GenericResponse<CourtResponseDto>> GetCourtByIdAsync(int id)
    {
        var court = await repository.GetByIdAsync(id);
        if (court == null)
            throw new KeyNotFoundException($"Court with ID {id} not found");

        var dto = new CourtResponseDto
        {
            Id = court.Id,
            Name = court.Name,
            Description = court.Description,
            Status = court.Status.ToString(),
            PricePerHour = court.PricePerHour
        };

        return new GenericResponse<CourtResponseDto>("Court retrieved successfully", dto);
    }

    public async Task<GenericResponse<CourtResponseDto>> CreateCourtAsync(CourtRequestDto request)
    {
        if (!Enum.TryParse<CourtStatus>(request.Status, true, out var status))
        {
            status = CourtStatus.Available;
        }

        var court = new Court
        {
            Name = request.Name,
            Description = request.Description,
            Status = status,
            PricePerHour = request.PricePerHour
        };

        var created = await repository.AddAsync(court);

        var dto = new CourtResponseDto
        {
            Id = created.Id,
            Name = created.Name,
            Description = created.Description,
            Status = created.Status.ToString(),
            PricePerHour = created.PricePerHour
        };

        return new GenericResponse<CourtResponseDto>("Court created successfully", dto);
    }

    public async Task<GenericResponse<bool>> UpdateCourtAsync(int id, CourtRequestDto request)
    {
        var court = await repository.GetByIdAsync(id);
        if (court == null)
            throw new KeyNotFoundException($"Court with ID {id} not found");

        if (!Enum.TryParse<CourtStatus>(request.Status, true, out var status))
        {
            status = CourtStatus.Available;
        }

        court.Name = request.Name;
        court.Description = request.Description;
        court.Status = status;
        court.PricePerHour = request.PricePerHour;

        await repository.UpdateAsync(court);

        return new GenericResponse<bool>("Court updated successfully", true);
    }

    public async Task<GenericResponse<bool>> DeleteCourtAsync(int id)
    {
        var court = await repository.GetByIdAsync(id);
        if (court == null)
            throw new KeyNotFoundException($"Court with ID {id} not found");

        await repository.DeleteAsync(id);

        return new GenericResponse<bool>("Court deleted successfully", true);
    }
}
