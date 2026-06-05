namespace DeportivoUCN.Application.DTO.Athlete;

public class AthleteRequestDto
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Rut { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public DateTime BirthDate { get; set; }
    public bool IsActive { get; set; } = true;
    public int? SportBranchId { get; set; }
}
