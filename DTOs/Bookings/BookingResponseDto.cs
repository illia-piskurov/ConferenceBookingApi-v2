namespace ConferenceBookingApi.DTOs.Bookings;

public class PriceBreakdownItemDto
{
    public string ZoneName { get; set; } = string.Empty;
    public DateTime From { get; set; }
    public DateTime To { get; set; }
    public double Hours { get; set; }
    public decimal Rate { get; set; }
    public decimal Multiplier { get; set; }
    public decimal Subtotal { get; set; }
}

public class BookingResponseDto
{
    public Guid Id { get; set; }
    public Guid RoomId { get; set; }
    public string RoomName { get; set; } = string.Empty;
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public double DurationHours { get; set; }
    public List<string> SelectedServices { get; set; } = new();
    public decimal RoomCost { get; set; }
    public decimal ServicesCost { get; set; }
    public decimal TotalCost { get; set; }
    public List<PriceBreakdownItemDto> PriceBreakdown { get; set; } = new();
}
