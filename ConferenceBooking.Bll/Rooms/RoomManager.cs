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

    public async Task<IEnumerable<Room>> GetAllRoomsAsync(CancellationToken cancellationToken = default)
    {
        return await _roomRepository.GetAllAsync(cancellationToken);
    }

    public async Task<Room> GetRoomByIdAsync(Guid id, bool includeDeleted = false, CancellationToken cancellationToken = default)
    {
        var room = await _roomRepository.GetByIdAsync(id, cancellationToken);
        if (room is null || (!includeDeleted && room.IsDeleted))
        {
            throw new RoomNotFoundException(id);
        }

        return room;
    }

    public async Task<Room> CreateRoomAsync(CreateRoomRequest request, CancellationToken cancellationToken = default)
    {
        var room = new Room
        {
            Id = Guid.NewGuid(),
            Name = request.Name,
            Capacity = request.Capacity,
            BaseHourlyRate = request.BaseHourlyRate,
            AvailableServices = request.AvailableServices.ToList()
        };

        return await _roomRepository.AddAsync(room, cancellationToken);
    }

    public async Task<Room> UpdateRoomAsync(Guid id, UpdateRoomRequest request, CancellationToken cancellationToken = default)
    {
        var existing = await GetRoomByIdAsync(id, cancellationToken: cancellationToken);

        existing.Name = request.Name;
        existing.Capacity = request.Capacity;
        existing.BaseHourlyRate = request.BaseHourlyRate;
        existing.AvailableServices = request.AvailableServices.ToList();

        return await _roomRepository.UpdateAsync(existing, cancellationToken);
    }

    public async Task DeleteRoomAsync(Guid id, CancellationToken cancellationToken = default)
    {
        await GetRoomByIdAsync(id, cancellationToken: cancellationToken);
        await _roomRepository.DeleteAsync(id, cancellationToken);
    }

    public async Task<IEnumerable<Room>> SearchAvailableRoomsAsync(DateTime start, DateTime end, int capacity, CancellationToken cancellationToken = default)
    {
        if (start >= end)
            throw new InvalidBookingTimeException("Час початку повинен бути раніше часу закінчення.");

        return await _roomRepository.SearchAvailableAsync(start, end, capacity, cancellationToken);
    }
}
