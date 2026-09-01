using ConferenceBooking.Bll.Common.Bookings;
using ConferenceBooking.Bll.Common.Bookings.Models;
using ConferenceBooking.Bll.Common.Rooms;
using ConferenceBooking.Bll.Common.Rooms.Models;
using ConferenceBooking.Bll.Common.Shared.Exceptions;

namespace ConferenceBooking.Bll.Bookings;

public class BookingManager : IBookingManager
{
    private readonly IRoomManager _roomManager;
    private readonly IBookingRepository _bookingRepository;
    private readonly IPricingManager _pricingManager;

    public BookingManager(
        IRoomManager roomManager,
        IBookingRepository bookingRepository,
        IPricingManager pricingManager)
    {
        _roomManager = roomManager;
        _bookingRepository = bookingRepository;
        _pricingManager = pricingManager;
    }

    public async Task<BookingDetails> CreateBookingAsync(Guid roomId, DateTime startTime, DateTime endTime, IEnumerable<Guid> selectedServiceIds, CancellationToken cancellationToken = default)
    {
        var room = await _roomManager.GetRoomByIdAsync(roomId, cancellationToken: cancellationToken);

        ValidateBookingTime(startTime, endTime);

        var serviceIdList = selectedServiceIds as IReadOnlyCollection<Guid> ?? selectedServiceIds.ToList();
        var selectedServices = GetSelectedServices(room, serviceIdList);

        var conflicts = await _bookingRepository.GetOverlappingAsync(roomId, startTime, endTime, cancellationToken);
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
            SelectedServiceIds = serviceIdList.ToList(),
            TotalCost = pricing.TotalCost,
            CreatedAt = DateTime.UtcNow
        };

        await _bookingRepository.AddAsync(booking, cancellationToken);

        return new BookingDetails
        {
            Booking = booking,
            Room = room,
            Pricing = pricing
        };
    }

    public async Task<BookingDetails> GetBookingByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var booking = await _bookingRepository.GetByIdAsync(id, cancellationToken);
        if (booking is null)
        {
            throw new BookingNotFoundException(id);
        }

        var room = await _roomManager.GetRoomByIdAsync(booking.RoomId, includeDeleted: true, cancellationToken: cancellationToken);

        return new BookingDetails
        {
            Booking = booking,
            Room = room
        };
    }

    public async Task<IEnumerable<BookingDetails>> GetBookingsByRoomAsync(Guid roomId, CancellationToken cancellationToken = default)
    {
        var room = await _roomManager.GetRoomByIdAsync(roomId, includeDeleted: true, cancellationToken: cancellationToken);

        var bookings = await _bookingRepository.GetByRoomIdAsync(roomId, cancellationToken);
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

    private static IReadOnlyCollection<Service> GetSelectedServices(Room room, IEnumerable<Guid> serviceIds)
    {
        var requestedIds = serviceIds as IReadOnlyCollection<Guid> ?? serviceIds.ToList();
        if (requestedIds.Count == 0)
            return Array.Empty<Service>();

        var availableServicesMap = room.AvailableServices.ToDictionary(s => s.Id);
        var unavailableIds = requestedIds.Where(id => !availableServicesMap.ContainsKey(id)).ToList();

        if (unavailableIds.Count > 0)
        {
            throw new InvalidBookingTimeException(
                $"Послуги {string.Join(", ", unavailableIds)} недоступні у цьому залі.");
        }

        return requestedIds.Select(id => availableServicesMap[id]).ToList();
    }
}
