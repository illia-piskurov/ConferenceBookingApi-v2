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

    public async Task<Room> CreateRoomAsync(Room room)
    {
        ValidateRoom(room);
        return await _roomRepository.AddAsync(room);
    }

    public async Task<Room> UpdateRoomAsync(Guid id, Room room)
    {
        await GetRoomByIdAsync(id);
        ValidateRoom(room);

        room.Id = id;
        return await _roomRepository.UpdateAsync(room);
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

    private static void ValidateRoom(Room room)
    {
        if (string.IsNullOrWhiteSpace(room.Name))
            throw new ArgumentException("Назва залу є обов'язковою.", nameof(room.Name));

        if (room.Capacity <= 0)
            throw new ArgumentException("Місткість повинна бути більшою за 0.", nameof(room.Capacity));

        if (room.BaseHourlyRate < 0)
            throw new ArgumentException("Погодинна ставка не може бути від'ємною.", nameof(room.BaseHourlyRate));
    }
}
