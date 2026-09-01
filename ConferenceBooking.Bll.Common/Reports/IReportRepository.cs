using ConferenceBooking.Bll.Common.Reports.Models;

namespace ConferenceBooking.Bll.Common.Reports;

public interface IReportRepository
{
    Task<IReadOnlyList<DailyRevenue>> GetRevenueByDayAsync(DateTime from, DateTime to, CancellationToken cancellationToken = default);
    Task<IEnumerable<RoomPopularity>> GetRoomPopularityAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<RoomLoad>> GetRoomLoadRawAsync(DateTime from, DateTime to, CancellationToken cancellationToken = default);
}
