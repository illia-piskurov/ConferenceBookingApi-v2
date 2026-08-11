namespace ConferenceBookingApi.DTOs.Reports;

public class RoomPopularityDto
{
    public Guid RoomId { get; set; }
    public string RoomName { get; set; } = string.Empty;
    public int TotalBookings { get; set; }
    public decimal TotalRevenue { get; set; }
    public double AverageBookingDurationHours { get; set; }
}
