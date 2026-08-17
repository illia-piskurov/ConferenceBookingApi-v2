namespace ConferenceBookingApi.Models;

public class Service
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal Price { get; set; }
}
