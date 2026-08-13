using ConferenceBookingApi.DTOs.Rooms;
using ConferenceBookingApi.Models;

namespace ConferenceBookingApi.Mappings;

public static class RoomMappingExtensions
{
    public static RoomResponseDto ToDto(this Room room) => new()
    {
        Id = room.Id,
        Name = room.Name,
        Capacity = room.Capacity,
        BaseHourlyRate = room.BaseHourlyRate,
        AvailableServices = room.AvailableServices.Select(s => s.ToDto()).ToList()
    };

    public static ServiceResponseDto ToDto(this Service service) => new()
    {
        Id = service.Id,
        Name = service.Name,
        Price = service.Price
    };
}
