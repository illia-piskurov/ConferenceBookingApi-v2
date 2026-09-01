using System.ComponentModel.DataAnnotations;

namespace ConferenceBooking.Services.Web.DTOs.Bookings;

public class CreateBookingDto
{
    [Required(ErrorMessage = "ID залу є обов'язковим.")]
    public Guid RoomId { get; set; }

    [Required(ErrorMessage = "Час початку є обов'язковим.")]
    public DateTime StartTime { get; set; }

    [Required(ErrorMessage = "Час закінчення є обов'язковим.")]
    public DateTime EndTime { get; set; }

    public List<Guid> SelectedServiceIds { get; set; } = [];
}
