using ConferenceBookingApi.DTOs.Bookings;
using ConferenceBookingApi.Models;

namespace ConferenceBookingApi.Mappings;

public static class BookingMappingExtensions
{
    public static BookingResponseDto ToDto(this Booking booking, Room? room) => new()
    {
        Id = booking.Id,
        RoomId = booking.RoomId,
        RoomName = room?.Name ?? "Невідомо",
        StartTime = booking.StartTime,
        EndTime = booking.EndTime,
        DurationHours = (booking.EndTime - booking.StartTime).TotalHours,
        TotalCost = booking.TotalCost,
        SelectedServices = room?.AvailableServices
            .Where(s => booking.SelectedServiceIds.Contains(s.Id))
            .Select(s => s.Name)
            .ToList() ?? new List<string>()
    };
}
