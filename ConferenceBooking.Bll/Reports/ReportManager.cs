using ConferenceBooking.Bll.Common.Bookings;
using ConferenceBooking.Bll.Common.Reports;
using ConferenceBooking.Bll.Common.Reports.Models;
using ConferenceBooking.Bll.Common.Rooms;
using ConferenceBooking.Bll.Common.Shared.Exceptions;

namespace ConferenceBooking.Bll.Reports;

public class ReportManager : IReportManager
{
    private readonly IBookingRepository _bookingRepository;
    private readonly IRoomRepository _roomRepository;

    private const double DailyWorkingHours = 17.0;

    public ReportManager(IBookingRepository bookingRepository, IRoomRepository roomRepository)
    {
        _bookingRepository = bookingRepository;
        _roomRepository = roomRepository;
    }

    public async Task<RevenueReport> GetRevenueReportAsync(DateTime from, DateTime to)
    {
        ValidateDateRange(from, to);

        var bookings = (await _bookingRepository.GetByDateRangeAsync(from, to)).ToList();

        var byDay = bookings
            .GroupBy(b => b.StartTime.Date)
            .Select(g => new DailyRevenue
            {
                Date = g.Key,
                Bookings = g.Count(),
                Revenue = g.Sum(b => b.TotalCost)
            })
            .OrderBy(d => d.Date)
            .ToList();

        return new RevenueReport
        {
            From = from,
            To = to,
            TotalBookings = bookings.Count,
            TotalRevenue = bookings.Sum(b => b.TotalCost),
            ByDay = byDay
        };
    }

    public async Task<IEnumerable<RoomPopularity>> GetRoomPopularityAsync()
    {
        var allBookings = (await _bookingRepository.GetAllAsync()).ToList();
        var allRooms = (await _roomRepository.GetAllAsync()).ToList();

        return allRooms
            .Select(room =>
            {
                var roomBookings = allBookings.Where(b => b.RoomId == room.Id).ToList();
                return new RoomPopularity
                {
                    RoomId = room.Id,
                    RoomName = room.Name,
                    TotalBookings = roomBookings.Count,
                    TotalRevenue = roomBookings.Sum(b => b.TotalCost),
                    AverageBookingDurationHours = roomBookings.Any()
                        ? roomBookings.Average(b => (b.EndTime - b.StartTime).TotalHours)
                        : 0
                };
            })
            .OrderByDescending(r => r.TotalBookings);
    }

    public async Task<IEnumerable<RoomLoad>> GetRoomLoadAsync(DateTime from, DateTime to)
    {
        ValidateDateRange(from, to);

        var rangeBookings = (await _bookingRepository.GetByDateRangeAsync(from, to)).ToList();
        var allRooms = (await _roomRepository.GetAllAsync()).ToList();

        var totalDays = (to - from).TotalDays;
        var totalAvailableHoursPerRoom = totalDays * DailyWorkingHours;

        return allRooms.Select(room =>
        {
            var roomBookings = rangeBookings.Where(b => b.RoomId == room.Id);

            var bookedHours = roomBookings.Sum(b =>
            {
                var effectiveStart = b.StartTime < from ? from : b.StartTime;
                var effectiveEnd = b.EndTime > to ? to : b.EndTime;
                return (effectiveEnd - effectiveStart).TotalHours;
            });

            return new RoomLoad
            {
                RoomId = room.Id,
                RoomName = room.Name,
                BookedHours = bookedHours,
                TotalAvailableHours = totalAvailableHoursPerRoom,
                LoadPercentage = totalAvailableHoursPerRoom > 0
                    ? Math.Round(bookedHours / totalAvailableHoursPerRoom * 100, 2)
                    : 0
            };
        });
    }

    private static void ValidateDateRange(DateTime from, DateTime to)
    {
        if (from >= to)
        {
            throw new InvalidBookingTimeException("Початкова дата періоду повинна бути раніше кінцевої дати.");
        }
    }
}
