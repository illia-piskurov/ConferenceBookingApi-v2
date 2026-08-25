using ConferenceBooking.Bll.Common.Bookings;
using ConferenceBooking.Bll.Common.Bookings.Models;
using ConferenceBooking.Bll.Common.Rooms;
using ConferenceBooking.Bll.Common.Rooms.Models;
using ConferenceBooking.Bll.Common.Shared.Exceptions;

namespace ConferenceBooking.Bll.Bookings;

public class BookingManager : IBookingManager
{
    private readonly IRoomRepository _roomRepository;
    private readonly IBookingRepository _bookingRepository;
    private readonly IPricingManager _pricingManager;

    public BookingManager(
        IRoomRepository roomRepository,
        IBookingRepository bookingRepository,
        IPricingManager pricingManager)
    {
        _roomRepository = roomRepository;
        _bookingRepository = bookingRepository;
        _pricingManager = pricingManager;
    }

    public async Task<BookingDetails> CreateBookingAsync(Guid roomId, DateTime startTime, DateTime endTime, List<Guid> selectedServiceIds)
    {
        var room = await _roomRepository.GetByIdAsync(roomId);
        if (room is null || room.IsDeleted)
        {
            throw new RoomNotFoundException(roomId);
        }

        ValidateBookingTime(startTime, endTime);

        var selectedServices = GetSelectedServices(room, selectedServiceIds);

        var conflicts = await _bookingRepository.GetOverlappingAsync(roomId, startTime, endTime);
        if (conflicts.Any())
        {
            throw new BookingConflictException(roomId, startTime, endTime);
        }

        var pricing = _pricingManager.Calculate(room, startTime, endTime, selectedServices);

        var booking = new Booking
        {
            Id = Guid.NewGuid(),
            RoomId = roomId,
            StartTime = startTime,
            EndTime = endTime,
            SelectedServiceIds = selectedServiceIds,
            TotalCost = pricing.TotalCost,
            CreatedAt = DateTime.UtcNow
        };

        await _bookingRepository.AddAsync(booking);

        return new BookingDetails
        {
            Booking = booking,
            Room = room,
            Pricing = pricing
        };
    }

    public async Task<BookingDetails> GetBookingByIdAsync(Guid id)
    {
        var booking = await _bookingRepository.GetByIdAsync(id);
        if (booking is null)
        {
            throw new InvalidBookingTimeException($"Бронювання із ID '{id}' не знайдено.");
        }

        var room = await _roomRepository.GetByIdAsync(booking.RoomId);
        if (room is null)
        {
            throw new RoomNotFoundException(booking.RoomId);
        }

        return new BookingDetails
        {
            Booking = booking,
            Room = room
        };
    }

    public async Task<IEnumerable<BookingDetails>> GetBookingsByRoomAsync(Guid roomId)
    {
        var room = await _roomRepository.GetByIdAsync(roomId);
        if (room is null)
        {
            throw new RoomNotFoundException(roomId);
        }

        var bookings = await _bookingRepository.GetByRoomIdAsync(roomId);
        return bookings.Select(b => new BookingDetails
        {
            Booking = b,
            Room = room
        });
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
        {
            throw new InvalidBookingTimeException(
                $"Послуги {string.Join(", ", unavailableIds)} недоступні у цьому залі.");
        }

        return selectedServices;
    }
}
