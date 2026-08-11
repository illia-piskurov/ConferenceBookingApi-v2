using ConferenceBookingApi.Models;

namespace ConferenceBookingApi.Repositories.Interfaces;

public interface IBookingRepository
{
    Task<IEnumerable<Booking>> GetAllAsync();
    Task<Booking?> GetByIdAsync(Guid id);
    Task<IEnumerable<Booking>> GetByRoomIdAsync(Guid roomId);
    Task<IEnumerable<Booking>> GetOverlappingAsync(Guid roomId, DateTime start, DateTime end);
    Task<Booking> AddAsync(Booking booking);
    Task<IEnumerable<Booking>> GetByDateRangeAsync(DateTime from, DateTime to);
}
