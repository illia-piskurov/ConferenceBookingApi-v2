using ConferenceBooking.Bll.Common.Rooms.Models;

namespace ConferenceBooking.Bll.Common.Rooms;

public interface IRoomManager
{
    Task<IEnumerable<Room>> GetAllRoomsAsync();
    Task<Room> GetRoomByIdAsync(Guid id, bool includeDeleted = false);
    Task<Room> CreateRoomAsync(CreateRoomRequest request);
    Task<Room> UpdateRoomAsync(Guid id, UpdateRoomRequest request);
    Task DeleteRoomAsync(Guid id);
    Task<IEnumerable<Room>> SearchAvailableRoomsAsync(DateTime start, DateTime end, int capacity);
}
