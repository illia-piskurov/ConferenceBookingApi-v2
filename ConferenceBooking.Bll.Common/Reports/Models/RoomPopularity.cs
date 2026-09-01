namespace ConferenceBooking.Bll.Common.Reports.Models;

public class RoomPopularity
{
    public Guid RoomId { get; set; }
    public string RoomName { get; set; } = string.Empty;
    public int TotalBookings { get; set; }
    public decimal TotalRevenue { get; set; }
    public double AverageBookingDurationHours { get; set; }
}
