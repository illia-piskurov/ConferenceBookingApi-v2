using ConferenceBooking.Bll.Common.Bookings.Models;

namespace ConferenceBooking.Bll.Common.Bookings;

public interface IBookingManager
{
    Task<BookingDetails> CreateBookingAsync(Guid roomId, DateTime startTime, DateTime endTime, IEnumerable<Guid> selectedServiceIds);
    Task<BookingDetails> GetBookingByIdAsync(Guid id);
    Task<IEnumerable<BookingDetails>> GetBookingsByRoomAsync(Guid roomId);
}
