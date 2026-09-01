namespace ConferenceBooking.Services.Web.DTOs.Reports;

public class DailyRevenueDto
{
    public DateTime Date { get; set; }
    public int Bookings { get; set; }
    public decimal Revenue { get; set; }
}
