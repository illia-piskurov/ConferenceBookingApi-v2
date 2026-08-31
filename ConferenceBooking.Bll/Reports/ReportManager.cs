using ConferenceBooking.Bll.Common.Reports;
using ConferenceBooking.Bll.Common.Reports.Models;
using ConferenceBooking.Bll.Common.Shared.Exceptions;

namespace ConferenceBooking.Bll.Reports;

public class ReportManager : IReportManager
{
    private readonly IReportRepository _reportRepository;

    private const double DailyWorkingHours = 17.0;

    public ReportManager(IReportRepository reportRepository)
    {
        _reportRepository = reportRepository;
    }

    public async Task<RevenueReport> GetRevenueReportAsync(DateTime from, DateTime to)
    {
        ValidateDateRange(from, to);

        var byDay = await _reportRepository.GetRevenueByDayAsync(from, to);

        return new RevenueReport
        {
            From = from,
            To = to,
            TotalBookings = byDay.Sum(d => d.Bookings),
            TotalRevenue = byDay.Sum(d => d.Revenue),
            ByDay = byDay
        };
    }

    public async Task<IEnumerable<RoomPopularity>> GetRoomPopularityAsync()
    {
        return await _reportRepository.GetRoomPopularityAsync();
    }

    public async Task<IEnumerable<RoomLoad>> GetRoomLoadAsync(DateTime from, DateTime to)
    {
        ValidateDateRange(from, to);

        var totalDays = (to - from).TotalDays;
        var totalAvailableHoursPerRoom = totalDays * DailyWorkingHours;

        var roomsLoad = await _reportRepository.GetRoomLoadRawAsync(from, to);

        foreach (var room in roomsLoad)
        {
            room.TotalAvailableHours = totalAvailableHoursPerRoom;
            room.LoadPercentage = totalAvailableHoursPerRoom > 0
                ? Math.Round(room.BookedHours / totalAvailableHoursPerRoom * 100, 2)
                : 0;
        }

        return roomsLoad;
    }

    private static void ValidateDateRange(DateTime from, DateTime to)
    {
        if (from >= to)
        {
            throw new InvalidBookingTimeException("Початкова дата періоду повинна бути раніше кінцевої дати.");
        }
    }
}
