namespace ConferenceBooking.Bll.Common.Reports.Models;

public class RoomLoad
{
    public Guid RoomId { get; set; }
    public string RoomName { get; set; } = string.Empty;
    public double BookedHours { get; set; }
    public double TotalAvailableHours { get; set; }
    public double LoadPercentage { get; set; }
}
