namespace ConferenceBookingApi.DTOs.Reports;

public class RoomLoadDto
{
    public Guid RoomId { get; set; }
    public string RoomName { get; set; } = string.Empty;
    public double LoadPercentage { get; set; }
    public double BookedHours { get; set; }
    public double TotalAvailableHours { get; set; }
}
