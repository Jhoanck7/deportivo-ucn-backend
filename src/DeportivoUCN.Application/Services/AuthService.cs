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

    public async Task<GenericResponse<AuthResponseDto>> GoogleLoginAsync(string idToken)
    {
        var handler = new JwtSecurityTokenHandler();
        if (!handler.CanReadToken(idToken))
        {
            throw new ArgumentException("El token de Google no es válido");
        }

        var jwtToken = handler.ReadJwtToken(idToken);
        
        // Validar que el token haya sido emitido para nuestro Client ID
        var expectedClientId = configuration["Google:ClientId"];
        var aud = jwtToken.Claims.FirstOrDefault(c => c.Type == "aud")?.Value;
        if (!string.IsNullOrEmpty(expectedClientId) && expectedClientId != "YOUR_GOOGLE_CLIENT_ID" && aud != expectedClientId)
        {
            throw new ArgumentException("El token de Google no fue emitido para esta aplicación (Audience mismatch)");
        }

        var email = jwtToken.Claims.FirstOrDefault(c => c.Type == "email")?.Value;
        if (string.IsNullOrEmpty(email))
        {
            throw new ArgumentException("El token de Google no contiene un correo electrónico");
        }

        var firstName = jwtToken.Claims.FirstOrDefault(c => c.Type == "given_name")?.Value ?? 
                        jwtToken.Claims.FirstOrDefault(c => c.Type == "name")?.Value ?? "Google";
        var lastName = jwtToken.Claims.FirstOrDefault(c => c.Type == "family_name")?.Value ?? "User";

        var user = await userRepository.GetByEmailAsync(email);
        if (user == null)
        {
            user = new User
            {
                Rut = "G-" + Guid.NewGuid().ToString("N")[..8],
                Email = email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(Guid.NewGuid().ToString()),
                FirstName = firstName,
                LastName = lastName,
                Phone = "",
                Role = UserRole.User
            };
            user = await userRepository.AddAsync(user);
        }

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

        return new GenericResponse<AuthResponseDto>("Google login successful", response);
    }

    public async Task<GenericResponse<UserResponseDto>> UpdateProfileAsync(string email, string rut, string phone)
    {
        var user = await userRepository.GetByEmailAsync(email);
        if (user == null)
            throw new KeyNotFoundException("Usuario no encontrado");

        if (!string.IsNullOrEmpty(rut) && rut != user.Rut)
        {
            var existing = await userRepository.GetByRutAsync(rut);
            if (existing != null)
                throw new InvalidOperationException("El RUT ya está registrado por otro usuario");
            user.Rut = rut;
        }

        if (phone != null)
        {
            user.Phone = phone;
        }

        await userRepository.UpdateAsync(user);

        var dto = new UserResponseDto
        {
            Id = user.Id,
            Rut = user.Rut,
            Email = user.Email,
            FirstName = user.FirstName,
            LastName = user.LastName,
            Phone = user.Phone,
            Role = user.Role.ToString(),
            IsBanned = user.IsBanned,
            BanReason = user.BanReason,
            CreatedAt = user.CreatedAt
        };

        return new GenericResponse<UserResponseDto>("Perfil actualizado con éxito", dto);
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
