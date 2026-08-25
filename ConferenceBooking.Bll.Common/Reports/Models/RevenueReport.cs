namespace ConferenceBooking.Bll.Common.Reports.Models;

public class RevenueReport
{
    public DateTime From { get; set; }
    public DateTime To { get; set; }
    public int TotalBookings { get; set; }
    public decimal TotalRevenue { get; set; }
    public List<DailyRevenue> ByDay { get; set; } = [];
}
