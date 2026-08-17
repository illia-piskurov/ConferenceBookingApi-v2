namespace ConferenceBookingApi.Models;

public class Booking
{
    public Guid Id { get; set; }
    public Guid RoomId { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public List<Guid> SelectedServiceIds { get; set; } = new();
    public decimal TotalCost { get; set; }
    public DateTime CreatedAt { get; set; }
}
