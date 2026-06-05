namespace DeportivoUCN.Application.DTO.Court;

public class CourtResponseDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public decimal PricePerHour { get; set; }
}
