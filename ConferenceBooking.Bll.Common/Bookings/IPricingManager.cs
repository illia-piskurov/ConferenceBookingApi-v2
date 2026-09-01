using ConferenceBooking.Bll.Common.Bookings.Models;
using ConferenceBooking.Bll.Common.Rooms.Models;

namespace ConferenceBooking.Bll.Common.Bookings;

public interface IPricingManager
{
    PricingResult Calculate(Room room, DateTime startTime, DateTime endTime, IEnumerable<Service> selectedServices);
}
