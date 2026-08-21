using System.Collections.Concurrent;
using AutoMapper;
using ConferenceBookingApi.Data.Repositories.Interfaces;
using ConferenceBookingApi.DTOs.Bookings;
using ConferenceBookingApi.Exceptions;
using ConferenceBookingApi.Models;
using ConferenceBookingApi.Services.Interfaces;

namespace ConferenceBookingApi.Services;

public class BookingService : IBookingService
{
    private readonly IRoomRepository _roomRepository;
    private readonly IBookingRepository _bookingRepository;
    private readonly IPricingService _pricingService;
    private readonly IMapper _mapper;

    private static readonly ConcurrentDictionary<Guid, SemaphoreSlim> _roomLocks = new();

    public BookingService(
        IRoomRepository roomRepository,
        IBookingRepository bookingRepository,
        IPricingService pricingService,
        IMapper mapper)
    {
        _roomRepository = roomRepository;
        _bookingRepository = bookingRepository;
        _pricingService = pricingService;
        _mapper = mapper;
    }

    public async Task<BookingResponseDto> CreateBookingAsync(CreateBookingDto dto)
    {
        var room = await _roomRepository.GetByIdAsync(dto.RoomId);
        if (room is null || room.IsDeleted) throw new RoomNotFoundException(dto.RoomId);

        ValidateBookingTime(dto.StartTime, dto.EndTime);

        var selectedServices = GetSelectedServices(room, dto.SelectedServiceIds);

        var roomLock = _roomLocks.GetOrAdd(dto.RoomId, _ => new SemaphoreSlim(1, 1));

        await roomLock.WaitAsync();
        try
        {
            var conflicts = await _bookingRepository.GetOverlappingAsync(
                dto.RoomId, dto.StartTime, dto.EndTime);

            if (conflicts.Any())
                throw new BookingConflictException(dto.RoomId, dto.StartTime, dto.EndTime);

            var pricing = _pricingService.Calculate(room, dto.StartTime, dto.EndTime, selectedServices);

            var booking = new Booking
            {
                Id = Guid.NewGuid(),
                RoomId = dto.RoomId,
                StartTime = dto.StartTime,
                EndTime = dto.EndTime,
                SelectedServiceIds = dto.SelectedServiceIds,
                TotalCost = pricing.TotalCost,
                CreatedAt = DateTime.UtcNow
            };

            await _bookingRepository.AddAsync(booking);

            var response = _mapper.Map<BookingResponseDto>((booking, room));
            response.RoomCost = pricing.RoomCost;
            response.ServicesCost = pricing.ServicesCost;
            response.PriceBreakdown = pricing.Breakdown;
            return response;
        }
        finally
        {
            roomLock.Release();
        }
    }

    public async Task<BookingResponseDto> GetBookingByIdAsync(Guid id)
    {
        var booking = await _bookingRepository.GetByIdAsync(id);
        if (booking is null)
            throw new InvalidBookingTimeException($"Бронювання із ID '{id}' не знайдено.");

        var room = await _roomRepository.GetByIdAsync(booking.RoomId);
        return _mapper.Map<BookingResponseDto>((booking, room));
    }

    public async Task<IEnumerable<BookingResponseDto>> GetBookingsByRoomAsync(Guid roomId)
    {
        var room = await _roomRepository.GetByIdAsync(roomId);
        if (room is null) throw new RoomNotFoundException(roomId);

        var bookings = await _bookingRepository.GetByRoomIdAsync(roomId);
        return _mapper.Map<IEnumerable<BookingResponseDto>>(bookings.Select(b => (b, room)));
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
}
