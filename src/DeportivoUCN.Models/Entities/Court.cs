using DeportivoUCN.Models.Enums;

namespace DeportivoUCN.Models.Entities;

public class Court
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public CourtStatus Status { get; set; } = CourtStatus.Available;
    public decimal PricePerHour { get; set; }

    public ICollection<Booking> Bookings { get; set; } = new List<Booking>();
}
