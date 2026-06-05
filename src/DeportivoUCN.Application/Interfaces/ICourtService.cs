using DeportivoUCN.Application.DTO;
using DeportivoUCN.Application.DTO.Court;

namespace DeportivoUCN.Application.Interfaces;

public interface ICourtService
{
    Task<GenericResponse<IEnumerable<CourtResponseDto>>> GetAllCourtsAsync();
    Task<GenericResponse<CourtResponseDto>> GetCourtByIdAsync(int id);
    Task<GenericResponse<CourtResponseDto>> CreateCourtAsync(CourtRequestDto request);
    Task<GenericResponse<bool>> UpdateCourtAsync(int id, CourtRequestDto request);
    Task<GenericResponse<bool>> DeleteCourtAsync(int id);
}
