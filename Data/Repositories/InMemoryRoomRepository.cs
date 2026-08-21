using System.Collections.Concurrent;
using ConferenceBookingApi.Data.Repositories.Interfaces;
using ConferenceBookingApi.Models;

namespace ConferenceBookingApi.Data.Repositories;

public class InMemoryRoomRepository : IRoomRepository
{
    private readonly ConcurrentDictionary<Guid, Room> _rooms = new();

    public Task<IEnumerable<Room>> GetAllAsync()
        => Task.FromResult(_rooms.Values.AsEnumerable());

    public Task<Room?> GetByIdAsync(Guid id)
        => Task.FromResult(_rooms.TryGetValue(id, out var room) ? room : null);

    public Task<Room> AddAsync(Room room)
    {
        _rooms[room.Id] = room;
        return Task.FromResult(room);
    }

    public Task<Room> UpdateAsync(Room room)
    {
        _rooms[room.Id] = room;
        return Task.FromResult(room);
    }

    public Task DeleteAsync(Guid id)
    {
        _rooms.TryRemove(id, out _);
        return Task.CompletedTask;
    }
}
