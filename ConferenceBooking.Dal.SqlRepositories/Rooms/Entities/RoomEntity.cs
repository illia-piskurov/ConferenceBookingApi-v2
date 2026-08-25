namespace ConferenceBooking.Dal.SqlRepositories.Rooms.Entities;

public class RoomEntity
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int Capacity { get; set; }
    public decimal BaseHourlyRate { get; set; }
    public bool IsDeleted { get; set; }
}
