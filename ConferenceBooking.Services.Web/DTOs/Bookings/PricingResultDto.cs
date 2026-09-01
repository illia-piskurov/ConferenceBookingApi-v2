namespace ConferenceBooking.Services.Web.DTOs.Bookings;

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

public class PricingResultDto
{
    public decimal RoomCost { get; set; }
    public decimal ServicesCost { get; set; }
    public decimal TotalCost { get; set; }
    public List<PriceBreakdownItemDto> Breakdown { get; set; } = [];
}
