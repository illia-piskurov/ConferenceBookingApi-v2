namespace ConferenceBooking.Dal.SqlRepositories.Rooms.Entities;

public class ServiceEntity
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal Price { get; set; }
}
