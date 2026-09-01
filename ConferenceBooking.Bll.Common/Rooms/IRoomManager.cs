using ConferenceBooking.Bll.Common.Rooms.Models;

namespace ConferenceBooking.Bll.Common.Rooms;

public interface IRoomManager
{
    Task<IEnumerable<Room>> GetAllRoomsAsync(CancellationToken cancellationToken = default);
    Task<Room> GetRoomByIdAsync(Guid id, bool includeDeleted = false, CancellationToken cancellationToken = default);
    Task<Room> CreateRoomAsync(CreateRoomRequest request, CancellationToken cancellationToken = default);
    Task<Room> UpdateRoomAsync(Guid id, UpdateRoomRequest request, CancellationToken cancellationToken = default);
    Task DeleteRoomAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IEnumerable<Room>> SearchAvailableRoomsAsync(DateTime start, DateTime end, int capacity, CancellationToken cancellationToken = default);
}
