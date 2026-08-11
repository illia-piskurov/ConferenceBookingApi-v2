using System.ComponentModel.DataAnnotations;

namespace ConferenceBookingApi.DTOs.Bookings;

public class CreateBookingDto
{
    [Required]
    public Guid RoomId { get; set; }

    [Required]
    public DateTime StartTime { get; set; }

    [Required]
    public DateTime EndTime { get; set; }

    public List<Guid> SelectedServiceIds { get; set; } = new();
}
