using ConferenceBooking.Bll.Common.Rooms.Models;

namespace ConferenceBooking.Bll.Common.Rooms;

public interface IRoomRepository
{
    Task<IEnumerable<Room>> GetAllAsync();
    Task<Room?> GetByIdAsync(Guid id);
    Task<Room> AddAsync(Room room);
    Task<Room> UpdateAsync(Room room);
    Task DeleteAsync(Guid id);
    Task<IEnumerable<Room>> SearchAvailableAsync(DateTime start, DateTime end, int capacity);
}
