using ConferenceBookingApi.DTOs.Rooms;

namespace ConferenceBookingApi.Services.Interfaces;

public interface IRoomService
{
    Task<IEnumerable<RoomResponseDto>> GetAllRoomsAsync();
    Task<RoomResponseDto> GetRoomByIdAsync(Guid id);
    Task<RoomResponseDto> CreateRoomAsync(CreateRoomDto dto);
    Task<RoomResponseDto> UpdateRoomAsync(Guid id, UpdateRoomDto dto);
    Task DeleteRoomAsync(Guid id);
    Task<IEnumerable<RoomResponseDto>> SearchAvailableRoomsAsync(DateTime start, DateTime end, int capacity);
}
