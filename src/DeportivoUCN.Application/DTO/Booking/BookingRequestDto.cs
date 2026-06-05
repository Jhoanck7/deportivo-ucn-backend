namespace DeportivoUCN.Application.DTO.Booking;

public class BookingRequestDto
{
    public int CourtId { get; set; }
    public DateOnly Date { get; set; }
    public int StartHour { get; set; }
    public decimal DepositAmount { get; set; }
}
