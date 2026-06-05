namespace DeportivoUCN.Application.DTO.User;

public class UserResponseDto
{
    public int Id { get; set; }
    public string Rut { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
    public bool IsBanned { get; set; }
    public string? BanReason { get; set; }
    public DateTime CreatedAt { get; set; }
}
