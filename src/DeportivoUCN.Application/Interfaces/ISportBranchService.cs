
using DeportivoUCN.Application.DTO;
using DeportivoUCN.Application.DTO.SportBranch;

namespace DeportivoUCN.Application.Interfaces;

public interface ISportBranchService
{
    Task<GenericResponse<IEnumerable<SportBranchResponseDto>>> GetAllBranchesAsync();
    Task<GenericResponse<SportBranchResponseDto>> GetBranchByIdAsync(int id);
    Task<GenericResponse<SportBranchResponseDto>> CreateBranchAsync(SportBranchRequestDto request);
    Task<GenericResponse<bool>> UpdateBranchAsync(int id, SportBranchRequestDto request);
    Task<GenericResponse<bool>> DeleteBranchAsync(int id);
}