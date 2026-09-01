namespace ConferenceBooking.Dal.SqlRepositories.Bookings.Entities;

public class BookingEntity
{
    public Guid Id { get; set; }
    public Guid RoomId { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public decimal TotalCost { get; set; }
    public DateTime CreatedAt { get; set; }
}
