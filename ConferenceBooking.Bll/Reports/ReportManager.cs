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

    public async Task<RevenueReport> GetRevenueReportAsync(DateTime from, DateTime to, CancellationToken cancellationToken = default)
    {
        ValidateDateRange(from, to);

        var byDay = await _reportRepository.GetRevenueByDayAsync(from, to, cancellationToken);

        return new RevenueReport
        {
            From = from,
            To = to,
            TotalBookings = byDay.Sum(d => d.Bookings),
            TotalRevenue = byDay.Sum(d => d.Revenue),
            ByDay = byDay
        };
    }

    public async Task<IEnumerable<RoomPopularity>> GetRoomPopularityAsync(CancellationToken cancellationToken = default)
    {
        return await _reportRepository.GetRoomPopularityAsync(cancellationToken);
    }

    public async Task<IEnumerable<RoomLoad>> GetRoomLoadAsync(DateTime from, DateTime to, CancellationToken cancellationToken = default)
    {
        ValidateDateRange(from, to);

        var totalDays = (to - from).TotalDays;
        var totalAvailableHoursPerRoom = totalDays * DailyWorkingHours;

        var roomsLoad = await _reportRepository.GetRoomLoadRawAsync(from, to, cancellationToken);

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
