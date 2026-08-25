namespace ConferenceBooking.Services.Web.DTOs.Reports;

public class RevenueReportDto
{
    public DateTime From { get; set; }
    public DateTime To { get; set; }
    public int TotalBookings { get; set; }
    public decimal TotalRevenue { get; set; }
    public List<DailyRevenueDto> ByDay { get; set; } = [];
}
