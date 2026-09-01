using ConferenceBooking.Services.Web.DTOs.Rooms;

namespace ConferenceBooking.Services.Web.DTOs.Bookings;

public class BookingResponseDto
{
    public Guid Id { get; set; }
    public Guid RoomId { get; set; }
    public string RoomName { get; set; } = string.Empty;
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public List<ServiceResponseDto> SelectedServices { get; set; } = [];
    public decimal RoomCost { get; set; }
    public decimal ServicesCost { get; set; }
    public decimal TotalCost { get; set; }
    public List<PriceBreakdownItemDto> PriceBreakdown { get; set; } = [];
    public DateTime CreatedAt { get; set; }
}
