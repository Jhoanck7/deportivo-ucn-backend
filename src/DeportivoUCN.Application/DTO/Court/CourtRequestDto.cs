namespace DeportivoUCN.Application.DTO.Court;

public class CourtRequestDto
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Status { get; set; } = "Available"; // Available, Disabled, UnderMaintenance
    public decimal PricePerHour { get; set; }
}
