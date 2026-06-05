using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using DeportivoUCN.Application.DTO;
using DeportivoUCN.Application.DTO.Auth;
using DeportivoUCN.Application.DTO.User;
using DeportivoUCN.Application.Interfaces;
using DeportivoUCN.Models.Entities;
using DeportivoUCN.Models.Enums;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace DeportivoUCN.Application.Services;

public class AuthService(
    IUserRepository userRepository,
    IConfiguration configuration) : IAuthService
{
    public async Task<GenericResponse<AuthResponseDto>> RegisterAsync(RegisterRequestDto request)
    {
        var existingEmail = await userRepository.GetByEmailAsync(request.Email);
        if (existingEmail != null)
            throw new InvalidOperationException("El correo electrónico ya está registrado");

        var existingRut = await userRepository.GetByRutAsync(request.Rut);
        if (existingRut != null)
            throw new InvalidOperationException("El RUT ya está registrado");

        if (!Enum.TryParse<UserRole>(request.Role, true, out var role))
        {
            role = UserRole.User;
        }

        var user = new User
        {
            Rut = request.Rut,
            Email = request.Email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
            FirstName = request.FirstName,
            LastName = request.LastName,
            Phone = request.Phone,
            Role = role
        };

        var created = await userRepository.AddAsync(user);

        var token = GenerateJwtToken(created);

        var response = new AuthResponseDto
        {
            Token = token,
            Email = created.Email,
            FirstName = created.FirstName,
            LastName = created.LastName,
            Role = created.Role.ToString()
        };

        return new GenericResponse<AuthResponseDto>("User registered successfully", response);
    }

    public async Task<GenericResponse<AuthResponseDto>> LoginAsync(LoginRequestDto request)
    {
        User? user;
        if (request.EmailOrRut.Contains("@"))
        {
            user = await userRepository.GetByEmailAsync(request.EmailOrRut);
        }
        else
        {
            user = await userRepository.GetByRutAsync(request.EmailOrRut);
        }

        if (user == null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
            throw new UnauthorizedAccessException("Credenciales inválidas");

        if (user.IsBanned)
            throw new UnauthorizedAccessException($"Tu cuenta está penalizada/vetada. Motivo: {user.BanReason}");

        var token = GenerateJwtToken(user);

        var response = new AuthResponseDto
        {
            Token = token,
            Email = user.Email,
            FirstName = user.FirstName,
            LastName = user.LastName,
            Role = user.Role.ToString()
        };

        return new GenericResponse<AuthResponseDto>("Login successful", response);
    }

    public async Task<GenericResponse<IEnumerable<UserResponseDto>>> GetAllUsersAsync()
    {
        var users = await userRepository.GetAllAsync();
        var dtos = users.Select(u => new UserResponseDto
        {
            Id = u.Id,
            Rut = u.Rut,
            Email = u.Email,
            FirstName = u.FirstName,
            LastName = u.LastName,
            Phone = u.Phone,
            Role = u.Role.ToString(),
            IsBanned = u.IsBanned,
            BanReason = u.BanReason,
            CreatedAt = u.CreatedAt
        });

        return new GenericResponse<IEnumerable<UserResponseDto>>("Users retrieved successfully", dtos);
    }

    public async Task<GenericResponse<UserResponseDto>> GetUserByIdAsync(int id)
    {
        var u = await userRepository.GetByIdAsync(id);
        if (u == null)
            throw new KeyNotFoundException($"User with ID {id} not found");

        var dto = new UserResponseDto
        {
            Id = u.Id,
            Rut = u.Rut,
            Email = u.Email,
            FirstName = u.FirstName,
            LastName = u.LastName,
            Phone = u.Phone,
            Role = u.Role.ToString(),
            IsBanned = u.IsBanned,
            BanReason = u.BanReason,
            CreatedAt = u.CreatedAt
        };

        return new GenericResponse<UserResponseDto>("User retrieved successfully", dto);
    }

    public async Task<GenericResponse<bool>> BanUserAsync(int id, BanUserRequestDto request)
    {
        var user = await userRepository.GetByIdAsync(id);
        if (user == null)
            throw new KeyNotFoundException($"User with ID {id} not found");

        user.IsBanned = request.IsBanned;
        user.BanReason = request.IsBanned ? request.BanReason : null;

        await userRepository.UpdateAsync(user);

        return new GenericResponse<bool>(
            request.IsBanned ? "Usuario penalizado con éxito" : "Penalización de usuario removida",
            true);
    }

    private string GenerateJwtToken(User user)
    {
        var keyString = configuration["Jwt:Key"] ?? "DeportivoUCNSuperSecretKeyForAuthentication123!";
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(keyString));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.Role, user.Role.ToString()),
            new Claim("Rut", user.Rut)
        };

        var issuer = configuration["Jwt:Issuer"] ?? "DeportivoUCN";
        var audience = configuration["Jwt:Audience"] ?? "DeportivoUCN-App";

        var token = new JwtSecurityToken(
            issuer: issuer,
            audience: audience,
            claims: claims,
            expires: DateTime.UtcNow.AddDays(7),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
