using DeportivoUCN.Models.Enums;

namespace DeportivoUCN.Models.Entities;

public class Booking
{
    public int Id { get; set; }
    
    public int CourtId { get; set; }
    public Court? Court { get; set; }

    public int UserId { get; set; }
    public User? User { get; set; }

    public DateOnly Date { get; set; }
    public int StartHour { get; set; } 
    public int EndHour { get; set; } 
    
    public BookingStatus Status { get; set; } = BookingStatus.Pending;
    public decimal DepositAmount { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public string? AdminNotes { get; set; }
}
