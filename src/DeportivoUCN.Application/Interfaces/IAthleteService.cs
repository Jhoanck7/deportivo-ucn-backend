using DeportivoUCN.Application.DTO;
using DeportivoUCN.Application.DTO.Athlete;

namespace DeportivoUCN.Application.Interfaces;

public interface IAthleteService
{
    Task<GenericResponse<IEnumerable<AthleteResponseDto>>> GetAllAthletesAsync();
    Task<GenericResponse<AthleteResponseDto>> GetAthleteByIdAsync(int id);
    Task<GenericResponse<AthleteResponseDto>> CreateAthleteAsync(AthleteRequestDto request);
    Task<GenericResponse<bool>> UpdateAthleteAsync(int id, AthleteRequestDto request);
    Task<GenericResponse<bool>> DeleteAthleteAsync(int id);
}
