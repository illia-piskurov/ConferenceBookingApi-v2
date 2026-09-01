namespace ConferenceBooking.Bll.Common.Bookings.Models;

public class PriceBreakdownItem
{
    public string ZoneName { get; set; } = string.Empty;
    public DateTime From { get; set; }
    public DateTime To { get; set; }
    public double Hours { get; set; }
    public decimal Rate { get; set; }
    public decimal Multiplier { get; set; }
    public decimal Subtotal { get; set; }
}
