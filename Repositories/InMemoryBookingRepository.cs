using System.Collections.Concurrent;
using ConferenceBookingApi.Models;
using ConferenceBookingApi.Repositories.Interfaces;

namespace ConferenceBookingApi.Repositories;

public class InMemoryBookingRepository : IBookingRepository
{
    private readonly ConcurrentDictionary<Guid, Booking> _bookings = new();

    public Task<IEnumerable<Booking>> GetAllAsync()
        => Task.FromResult(_bookings.Values.AsEnumerable());

    public Task<Booking?> GetByIdAsync(Guid id)
        => Task.FromResult(_bookings.TryGetValue(id, out var b) ? b : null);

    public Task<IEnumerable<Booking>> GetByRoomIdAsync(Guid roomId)
        => Task.FromResult(_bookings.Values.Where(b => b.RoomId == roomId));

    public Task<IEnumerable<Booking>> GetOverlappingAsync(Guid roomId, DateTime start, DateTime end)
    {
        var overlapping = _bookings.Values.Where(b =>
            b.RoomId == roomId &&
            b.StartTime < end &&
            b.EndTime > start);

        return Task.FromResult(overlapping);
    }

    public Task<Booking> AddAsync(Booking booking)
    {
        _bookings[booking.Id] = booking;
        return Task.FromResult(booking);
    }

    public Task<IEnumerable<Booking>> GetByDateRangeAsync(DateTime from, DateTime to)
    {
        var bookings = _bookings.Values.Where(b =>
            b.StartTime >= from && b.StartTime < to);
        return Task.FromResult(bookings);
    }
}
