using DeportivoUCN.Application.DTO;
using DeportivoUCN.Application.DTO.Auth;
using DeportivoUCN.Application.DTO.User;

namespace DeportivoUCN.Application.Interfaces;

public interface IAuthService
{
    Task<GenericResponse<AuthResponseDto>> RegisterAsync(RegisterRequestDto request);
    Task<GenericResponse<AuthResponseDto>> LoginAsync(LoginRequestDto request);
    Task<GenericResponse<AuthResponseDto>> GoogleLoginAsync(string idToken);
    Task<GenericResponse<UserResponseDto>> UpdateProfileAsync(string email, string rut, string phone);
    Task<GenericResponse<IEnumerable<UserResponseDto>>> GetAllUsersAsync();
    Task<GenericResponse<UserResponseDto>> GetUserByIdAsync(int id);
    Task<GenericResponse<bool>> BanUserAsync(int id, BanUserRequestDto request);
}
