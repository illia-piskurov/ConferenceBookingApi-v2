namespace ConferenceBookingApi.DTOs.Bookings;

public class PricingResultDto
{
    public decimal RoomCost { get; set; }
    public decimal ServicesCost { get; set; }
    public decimal TotalCost { get; set; }
    public List<PriceBreakdownItemDto> Breakdown { get; set; } = [];
}
