namespace ConferenceBooking.Bll.Common.Reports.Models;

public class DailyRevenue
{
    public DateTime Date { get; set; }
    public int Bookings { get; set; }
    public decimal Revenue { get; set; }
}
