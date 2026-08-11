namespace ConferenceBookingApi.Models;

public class Room
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int Capacity { get; set; }
    public decimal BaseHourlyRate { get; set; }
    public List<Service> AvailableServices { get; set; } = new();
}
