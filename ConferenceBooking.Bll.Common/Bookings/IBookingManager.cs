using ConferenceBooking.Bll.Common.Bookings.Models;

namespace ConferenceBooking.Bll.Common.Bookings;

public interface IBookingManager
{
    Task<BookingDetails> CreateBookingAsync(Guid roomId, DateTime startTime, DateTime endTime, IEnumerable<Guid> selectedServiceIds, CancellationToken cancellationToken = default);
    Task<BookingDetails> GetBookingByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IEnumerable<BookingDetails>> GetBookingsByRoomAsync(Guid roomId, CancellationToken cancellationToken = default);
}
