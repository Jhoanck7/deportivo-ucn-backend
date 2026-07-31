using DeportivoUCN.Application.DTO;
using DeportivoUCN.Application.DTO.Auth;
using DeportivoUCN.Application.DTO.User;
using DeportivoUCN.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DeportivoUCN.API.Controllers;

[ApiController]
[Route("api/auth")]
// comentario de prueba
public class AuthController(IAuthService authService) : ControllerBase
{
    [HttpPost("register")]
    public async Task<ActionResult<GenericResponse<AuthResponseDto>>> Register([FromBody] RegisterRequestDto request)
    {
        var response = await authService.RegisterAsync(request);
        return Ok(response);
    }

    [HttpPost("login")]
    public async Task<ActionResult<GenericResponse<AuthResponseDto>>> Login([FromBody] LoginRequestDto request)
    {
        var response = await authService.LoginAsync(request);
        return Ok(response);
    }

    [HttpPost("google")]
    public async Task<ActionResult<GenericResponse<AuthResponseDto>>> GoogleLogin([FromBody] GoogleLoginRequestDto request)
    {
        var response = await authService.GoogleLoginAsync(request.IdToken);
        return Ok(response);
    }

    [HttpGet("users")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<GenericResponse<IEnumerable<UserResponseDto>>>> GetAllUsers()
    {
        var response = await authService.GetAllUsersAsync();
        return Ok(response);
    }

    [HttpGet("users/{id:int}")]
    [Authorize]
    public async Task<ActionResult<GenericResponse<UserResponseDto>>> GetUserById(int id)
    {
        var response = await authService.GetUserByIdAsync(id);
        return Ok(response);
    }

    [HttpPut("users/{id:int}/ban")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<GenericResponse<bool>>> BanUser(int id, [FromBody] BanUserRequestDto request)
    {
        var response = await authService.BanUserAsync(id, request);
        return Ok(response);
    }

    [HttpPut("profile")]
    [Authorize]
    public async Task<ActionResult<GenericResponse<UserResponseDto>>> UpdateProfile([FromBody] UpdateProfileRequestDto request)
    {
        var email = User.FindFirst(System.Security.Claims.ClaimTypes.Email)?.Value;
        if (string.IsNullOrEmpty(email))
            return Unauthorized(new { message = "No autorizado" });

        var response = await authService.UpdateProfileAsync(email, request.Rut, request.Phone);
        return Ok(response);
    }
}

public class GoogleLoginRequestDto
{
    public string IdToken { get; set; } = string.Empty;
}

public class UpdateProfileRequestDto
{
    public string Rut { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
}
