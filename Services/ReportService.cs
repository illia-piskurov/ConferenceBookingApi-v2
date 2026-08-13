using ConferenceBookingApi.DTOs.Reports;
using ConferenceBookingApi.Exceptions;
using ConferenceBookingApi.Repositories.Interfaces;
using ConferenceBookingApi.Services.Interfaces;

namespace ConferenceBookingApi.Services;

public class ReportService : IReportService
{
    private readonly IBookingRepository _bookingRepository;
    private readonly IRoomRepository _roomRepository;

    private const double DailyWorkingHours = 17.0;

    public ReportService(IBookingRepository bookingRepository, IRoomRepository roomRepository)
    {
        _bookingRepository = bookingRepository;
        _roomRepository = roomRepository;
    }

    public async Task<RevenueReportDto> GetRevenueReportAsync(DateTime from, DateTime to)
    {
        ValidateDateRange(from, to);

        var bookings = (await _bookingRepository.GetByDateRangeAsync(from, to)).ToList();

        var byDay = bookings
            .GroupBy(b => b.StartTime.Date)
            .Select(g => new DailyRevenueDto
            {
                Date = g.Key,
                Bookings = g.Count(),
                Revenue = g.Sum(b => b.TotalCost)
            })
            .OrderBy(d => d.Date)
            .ToList();

        return new RevenueReportDto
        {
            From = from,
            To = to,
            TotalBookings = bookings.Count,
            TotalRevenue = bookings.Sum(b => b.TotalCost),
            ByDay = byDay
        };
    }

    public async Task<IEnumerable<RoomPopularityDto>> GetRoomPopularityAsync()
    {
        var allBookings = (await _bookingRepository.GetAllAsync()).ToList();
        var allRooms = (await _roomRepository.GetAllAsync()).ToList();

        return allRooms
            .Select(room =>
            {
                var roomBookings = allBookings.Where(b => b.RoomId == room.Id).ToList();
                return new RoomPopularityDto
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

    public async Task<IEnumerable<RoomLoadDto>> GetRoomLoadAsync(DateTime from, DateTime to)
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

            return new RoomLoadDto
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
