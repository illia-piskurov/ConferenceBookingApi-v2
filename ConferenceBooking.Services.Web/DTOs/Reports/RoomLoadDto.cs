namespace ConferenceBooking.Services.Web.DTOs.Reports;

public class RoomLoadDto
{
    public Guid RoomId { get; set; }
    public string RoomName { get; set; } = string.Empty;
    public double BookedHours { get; set; }
    public double TotalAvailableHours { get; set; }
    public double LoadPercentage { get; set; }
}
