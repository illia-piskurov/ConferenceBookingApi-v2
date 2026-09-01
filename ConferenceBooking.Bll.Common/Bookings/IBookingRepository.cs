using ConferenceBooking.Bll.Common.Bookings.Models;

namespace ConferenceBooking.Bll.Common.Bookings;

public interface IBookingRepository
{
    Task<IEnumerable<Booking>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<Booking?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IEnumerable<Booking>> GetByRoomIdAsync(Guid roomId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Booking>> GetOverlappingAsync(Guid roomId, DateTime start, DateTime end, CancellationToken cancellationToken = default);
    Task<IEnumerable<Booking>> GetByDateRangeAsync(DateTime from, DateTime to, CancellationToken cancellationToken = default);
    Task<Booking> AddAsync(Booking booking, CancellationToken cancellationToken = default);
}
