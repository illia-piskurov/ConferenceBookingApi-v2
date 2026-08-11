using ConferenceBookingApi.DTOs.Bookings;

namespace ConferenceBookingApi.Services.Interfaces;

public interface IBookingService
{
    Task<BookingResponseDto> CreateBookingAsync(CreateBookingDto dto);
    Task<BookingResponseDto> GetBookingByIdAsync(Guid id);
    Task<IEnumerable<BookingResponseDto>> GetBookingsByRoomAsync(Guid roomId);
}
