namespace ConferenceBooking.Bll.Common.Rooms.Models;

public class CreateRoomRequest
{
    public string Name { get; set; } = string.Empty;
    public int Capacity { get; set; }
    public decimal BaseHourlyRate { get; set; }
    public List<Service> AvailableServices { get; set; } = [];
}
