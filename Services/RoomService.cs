using ConferenceBookingApi.DTOs.Rooms;
using ConferenceBookingApi.Exceptions;
using ConferenceBookingApi.Models;
using ConferenceBookingApi.Repositories.Interfaces;
using ConferenceBookingApi.Services.Interfaces;

namespace ConferenceBookingApi.Services;

public class RoomService : IRoomService
{
    private readonly IRoomRepository _roomRepository;
    private readonly IBookingRepository _bookingRepository;

    public RoomService(IRoomRepository roomRepository, IBookingRepository bookingRepository)
    {
        _roomRepository = roomRepository;
        _bookingRepository = bookingRepository;
    }

    public async Task<IEnumerable<RoomResponseDto>> GetAllRoomsAsync()
    {
        var rooms = await _roomRepository.GetAllAsync();
        return rooms.Select(MapToDto);
    }

    public async Task<RoomResponseDto> GetRoomByIdAsync(Guid id)
    {
        var room = await _roomRepository.GetByIdAsync(id);
        if (room is null) throw new RoomNotFoundException(id);
        return MapToDto(room);
    }

    public async Task<RoomResponseDto> CreateRoomAsync(CreateRoomDto dto)
    {
        var room = new Room
        {
            Id = Guid.NewGuid(),
            Name = dto.Name,
            Capacity = dto.Capacity,
            BaseHourlyRate = dto.BaseHourlyRate,
            AvailableServices = dto.Services.Select(s => new Service
            {
                Id = Guid.NewGuid(),
                Name = s.Name,
                Price = s.Price
            }).ToList()
        };

        await _roomRepository.AddAsync(room);
        return MapToDto(room);
    }

    public async Task<RoomResponseDto> UpdateRoomAsync(Guid id, UpdateRoomDto dto)
    {
        var existing = await _roomRepository.GetByIdAsync(id);
        if (existing is null) throw new RoomNotFoundException(id);

        existing.Name = dto.Name;
        existing.Capacity = dto.Capacity;
        existing.BaseHourlyRate = dto.BaseHourlyRate;
        existing.AvailableServices = dto.Services.Select(s => new Service
        {
            Id = Guid.NewGuid(),
            Name = s.Name,
            Price = s.Price
        }).ToList();

        await _roomRepository.UpdateAsync(existing);
        return MapToDto(existing);
    }

    public async Task DeleteRoomAsync(Guid id)
    {
        var existing = await _roomRepository.GetByIdAsync(id);
        if (existing is null) throw new RoomNotFoundException(id);
        await _roomRepository.DeleteAsync(id);
    }

    public async Task<IEnumerable<RoomResponseDto>> SearchAvailableRoomsAsync(
        DateTime start, DateTime end, int capacity)
    {
        var allRooms = await _roomRepository.GetAllAsync();

        var suitableRooms = allRooms.Where(r => r.Capacity >= capacity);

        var availableRooms = new List<Room>();
        foreach (var room in suitableRooms)
        {
            var conflicts = await _bookingRepository.GetOverlappingAsync(room.Id, start, end);
            if (!conflicts.Any())
                availableRooms.Add(room);
        }

        return availableRooms.Select(MapToDto);
    }

    private static RoomResponseDto MapToDto(Room room) => new()
    {
        Id = room.Id,
        Name = room.Name,
        Capacity = room.Capacity,
        BaseHourlyRate = room.BaseHourlyRate,
        AvailableServices = room.AvailableServices.Select(s => new ServiceResponseDto
        {
            Id = s.Id,
            Name = s.Name,
            Price = s.Price
        }).ToList()
    };
}
