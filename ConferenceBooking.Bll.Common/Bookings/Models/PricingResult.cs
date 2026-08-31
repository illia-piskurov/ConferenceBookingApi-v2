namespace ConferenceBooking.Bll.Common.Bookings.Models;

public class PricingResult
{
    public decimal RoomCost { get; set; }
    public decimal ServicesCost { get; set; }
    public decimal TotalCost { get; set; }
    public IReadOnlyList<PriceBreakdownItem> Breakdown { get; set; } = [];
}
