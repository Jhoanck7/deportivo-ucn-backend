namespace DeportivoUCN.Application.DTO.Athlete;

public class AthleteResponseDto
{
    public int Id { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Rut { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public DateTime BirthDate { get; set; }
    public bool IsActive { get; set; }
    public int? SportBranchId { get; set; }
    public string? SportBranchName { get; set; }
}
