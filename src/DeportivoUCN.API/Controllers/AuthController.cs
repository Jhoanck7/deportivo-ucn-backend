using DeportivoUCN.Application.DTO;
using DeportivoUCN.Application.DTO.Auth;
using DeportivoUCN.Application.DTO.User;
using DeportivoUCN.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DeportivoUCN.API.Controllers;

[ApiController]
[Route("api/auth")]
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
}
