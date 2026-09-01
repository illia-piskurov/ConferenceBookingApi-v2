using ConferenceBooking.Bll.Common.Rooms.Models;

namespace ConferenceBooking.Bll.Common.Bookings.Models;

public class BookingDetails
{
    public Booking Booking { get; set; } = null!;
    public Room Room { get; set; } = null!;
    public PricingResult? Pricing { get; set; }
}
