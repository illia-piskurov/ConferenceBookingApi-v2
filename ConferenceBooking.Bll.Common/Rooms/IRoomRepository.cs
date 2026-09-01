using ConferenceBooking.Bll.Common.Rooms.Models;

namespace ConferenceBooking.Bll.Common.Rooms;

public interface IRoomRepository
{
    Task<IEnumerable<Room>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<Room?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Room> AddAsync(Room room, CancellationToken cancellationToken = default);
    Task<Room> UpdateAsync(Room room, CancellationToken cancellationToken = default);
    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IEnumerable<Room>> SearchAvailableAsync(DateTime start, DateTime end, int capacity, CancellationToken cancellationToken = default);
}
