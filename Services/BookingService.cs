using ConferenceBookingApi.DTOs.Bookings;
using ConferenceBookingApi.Exceptions;
using ConferenceBookingApi.Models;
using ConferenceBookingApi.Repositories.Interfaces;
using ConferenceBookingApi.Services.Interfaces;

namespace ConferenceBookingApi.Services;

public class BookingService : IBookingService
{
    private readonly IRoomRepository _roomRepository;
    private readonly IBookingRepository _bookingRepository;
    private readonly PricingService _pricingService;

    private static readonly SemaphoreSlim _bookingLock = new(1, 1);

    public BookingService(
        IRoomRepository roomRepository,
        IBookingRepository bookingRepository,
        PricingService pricingService)
    {
        _roomRepository = roomRepository;
        _bookingRepository = bookingRepository;
        _pricingService = pricingService;
    }

    public async Task<BookingResponseDto> CreateBookingAsync(CreateBookingDto dto)
    {
        var room = await _roomRepository.GetByIdAsync(dto.RoomId);
        if (room is null) throw new RoomNotFoundException(dto.RoomId);

        ValidateBookingTime(dto.StartTime, dto.EndTime);

        var selectedServices = GetSelectedServices(room, dto.SelectedServiceIds);

        await _bookingLock.WaitAsync();
        try
        {
            var conflicts = await _bookingRepository.GetOverlappingAsync(
                dto.RoomId, dto.StartTime, dto.EndTime);

            if (conflicts.Any())
                throw new BookingConflictException(dto.RoomId, dto.StartTime, dto.EndTime);

            var (roomCost, servicesCost, totalCost, breakdown) =
                _pricingService.Calculate(room, dto.StartTime, dto.EndTime, selectedServices);

            var booking = new Booking
            {
                Id = Guid.NewGuid(),
                RoomId = dto.RoomId,
                StartTime = dto.StartTime,
                EndTime = dto.EndTime,
                SelectedServiceIds = dto.SelectedServiceIds,
                TotalCost = totalCost,
                CreatedAt = DateTime.UtcNow
            };

            await _bookingRepository.AddAsync(booking);

            return new BookingResponseDto
            {
                Id = booking.Id,
                RoomId = room.Id,
                RoomName = room.Name,
                StartTime = booking.StartTime,
                EndTime = booking.EndTime,
                DurationHours = (booking.EndTime - booking.StartTime).TotalHours,
                SelectedServices = selectedServices.Select(s => s.Name).ToList(),
                RoomCost = roomCost,
                ServicesCost = servicesCost,
                TotalCost = totalCost,
                PriceBreakdown = breakdown
            };
        }
        finally
        {
            _bookingLock.Release();
        }
    }

    public async Task<BookingResponseDto> GetBookingByIdAsync(Guid id)
    {
        var booking = await _bookingRepository.GetByIdAsync(id);
        if (booking is null)
            throw new InvalidBookingTimeException($"Бронювання із ID '{id}' не знайдено.");

        var room = await _roomRepository.GetByIdAsync(booking.RoomId);
        return MapToDto(booking, room);
    }

    public async Task<IEnumerable<BookingResponseDto>> GetBookingsByRoomAsync(Guid roomId)
    {
        var room = await _roomRepository.GetByIdAsync(roomId);
        if (room is null) throw new RoomNotFoundException(roomId);

        var bookings = await _bookingRepository.GetByRoomIdAsync(roomId);
        return bookings.Select(b => MapToDto(b, room));
    }

    private static void ValidateBookingTime(DateTime start, DateTime end)
    {
        if (start >= end)
            throw new InvalidBookingTimeException("Час початку повинен бути раніше часу закінчення.");

        if (start < DateTime.UtcNow)
            throw new InvalidBookingTimeException("Неможливо забронювати зал у минулому.");

        if ((end - start).TotalHours > 24)
            throw new InvalidBookingTimeException("Бронювання не може тривати більше 24 годин.");

        if (start.Hour < 6 || end.Hour > 23 || (end.Hour == 23 && end.Minute > 0))
            throw new InvalidBookingTimeException("Зали доступні з 06:00 до 23:00.");
    }

    private static List<Service> GetSelectedServices(Room room, List<Guid> serviceIds)
    {
        var selectedServices = room.AvailableServices
            .Where(s => serviceIds.Contains(s.Id))
            .ToList();

        var unavailableIds = serviceIds
            .Where(id => !room.AvailableServices.Any(s => s.Id == id))
            .ToList();

        if (unavailableIds.Any())
            throw new InvalidBookingTimeException(
                $"Послуги {string.Join(", ", unavailableIds)} недоступні у цьому залі.");

        return selectedServices;
    }

    private static BookingResponseDto MapToDto(Booking booking, Room? room) => new()
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
