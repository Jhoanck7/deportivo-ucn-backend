namespace DeportivoUCN.Application.DTO.Auth;

public class RegisterRequestDto
{
    public string Rut { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string Role { get; set; } = "User"; // Can be Admin, Coach, User (but default to User in registration endpoint, or validated)
}
