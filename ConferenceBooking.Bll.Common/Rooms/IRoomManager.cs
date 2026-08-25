using ConferenceBooking.Bll.Common.Rooms.Models;

namespace ConferenceBooking.Bll.Common.Rooms;

public interface IRoomManager
{
    Task<IEnumerable<Room>> GetAllRoomsAsync();
    Task<Room> GetRoomByIdAsync(Guid id);
    Task<Room> CreateRoomAsync(Room room);
    Task<Room> UpdateRoomAsync(Guid id, Room room);
    Task DeleteRoomAsync(Guid id);
    Task<IEnumerable<Room>> SearchAvailableRoomsAsync(DateTime start, DateTime end, int capacity);
}
