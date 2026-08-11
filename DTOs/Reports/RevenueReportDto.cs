namespace ConferenceBookingApi.DTOs.Reports;

public class DailyRevenueDto
{
    public DateTime Date { get; set; }
    public int Bookings { get; set; }
    public decimal Revenue { get; set; }
}

public class RevenueReportDto
{
    public DateTime From { get; set; }
    public DateTime To { get; set; }
    public int TotalBookings { get; set; }
    public decimal TotalRevenue { get; set; }
    public List<DailyRevenueDto> ByDay { get; set; } = new();
}
