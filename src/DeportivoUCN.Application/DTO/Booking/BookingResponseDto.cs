namespace DeportivoUCN.Application.DTO.Booking;

public class BookingResponseDto
{
    public int Id { get; set; }
    public int CourtId { get; set; }
    public string CourtName { get; set; } = string.Empty;
    public int UserId { get; set; }
    public string UserFullName { get; set; } = string.Empty;
    public string UserPhone { get; set; } = string.Empty;
    public DateOnly Date { get; set; }
    public int StartHour { get; set; }
    public int EndHour { get; set; }
    public string Status { get; set; } = string.Empty;
    public decimal DepositAmount { get; set; }
    public decimal TotalPrice { get; set; }
    public string WhatsAppLink { get; set; } = string.Empty;
    public string? AdminNotes { get; set; }
    public DateTime CreatedAt { get; set; }
}
