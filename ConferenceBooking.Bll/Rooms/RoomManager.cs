using ConferenceBooking.Bll.Common.Rooms;
using ConferenceBooking.Bll.Common.Rooms.Models;
using ConferenceBooking.Bll.Common.Shared.Exceptions;

namespace ConferenceBooking.Bll.Rooms;

public class RoomManager : IRoomManager
{
    private readonly IRoomRepository _roomRepository;

    public RoomManager(IRoomRepository roomRepository)
    {
        _roomRepository = roomRepository;
    }

    public async Task<IEnumerable<Room>> GetAllRoomsAsync()
    {
        return await _roomRepository.GetAllAsync();
    }

    public async Task<Room> GetRoomByIdAsync(Guid id)
    {
        var room = await _roomRepository.GetByIdAsync(id);
        if (room is null || room.IsDeleted)
        {
            throw new RoomNotFoundException(id);
        }

        return room;
    }

    public async Task<Room> CreateRoomAsync(CreateRoomRequest request)
    {
        var room = new Room
        {
            Id = Guid.NewGuid(),
            Name = request.Name,
            Capacity = request.Capacity,
            BaseHourlyRate = request.BaseHourlyRate,
            AvailableServices = request.AvailableServices.ToList()
        };

        return await _roomRepository.AddAsync(room);
    }

    public async Task<Room> UpdateRoomAsync(Guid id, UpdateRoomRequest request)
    {
        var existing = await GetRoomByIdAsync(id);

        existing.Name = request.Name;
        existing.Capacity = request.Capacity;
        existing.BaseHourlyRate = request.BaseHourlyRate;
        existing.AvailableServices = request.AvailableServices.ToList();

        return await _roomRepository.UpdateAsync(existing);
    }

    public async Task DeleteRoomAsync(Guid id)
    {
        await GetRoomByIdAsync(id);
        await _roomRepository.DeleteAsync(id);
    }

    public async Task<IEnumerable<Room>> SearchAvailableRoomsAsync(DateTime start, DateTime end, int capacity)
    {
        if (start >= end)
            throw new InvalidBookingTimeException("Час початку повинен бути раніше часу закінчення.");

        return await _roomRepository.SearchAvailableAsync(start, end, capacity);
    }
}
