using ConferenceBookingApi.Models;

namespace ConferenceBookingApi.Repositories.Interfaces;

public interface IRoomRepository
{
    Task<IEnumerable<Room>> GetAllAsync();
    Task<Room?> GetByIdAsync(Guid id);
    Task<Room> AddAsync(Room room);
    Task<Room> UpdateAsync(Room room);
    Task DeleteAsync(Guid id);
}
