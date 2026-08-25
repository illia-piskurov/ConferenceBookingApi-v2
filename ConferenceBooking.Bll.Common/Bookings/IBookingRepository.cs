using ConferenceBooking.Bll.Common.Bookings.Models;

namespace ConferenceBooking.Bll.Common.Bookings;

public interface IBookingRepository
{
    Task<IEnumerable<Booking>> GetAllAsync();
    Task<Booking?> GetByIdAsync(Guid id);
    Task<IEnumerable<Booking>> GetByRoomIdAsync(Guid roomId);
    Task<IEnumerable<Booking>> GetOverlappingAsync(Guid roomId, DateTime start, DateTime end);
    Task<IEnumerable<Booking>> GetByDateRangeAsync(DateTime from, DateTime to);
    Task<Booking> AddAsync(Booking booking);
}
