using DeportivoUCN.Application.DTO;
using DeportivoUCN.Application.DTO.Coach;

namespace DeportivoUCN.Application.Interfaces;

public interface ICoachService
{
    Task<GenericResponse<IEnumerable<CoachResponseDto>>> GetAllCoachesAsync();
    Task<GenericResponse<CoachResponseDto>> GetCoachByIdAsync(int id);
    Task<GenericResponse<CoachResponseDto>> CreateCoachAsync(CoachRequestDto request);
    Task<GenericResponse<bool>> UpdateCoachAsync(int id, CoachRequestDto request);
    Task<GenericResponse<bool>> DeleteCoachAsync(int id);
}