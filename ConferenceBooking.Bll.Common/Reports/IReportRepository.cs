using ConferenceBooking.Bll.Common.Reports.Models;

namespace ConferenceBooking.Bll.Common.Reports;

public interface IReportRepository
{
    Task<IEnumerable<DailyRevenue>> GetRevenueByDayAsync(DateTime from, DateTime to);
    Task<IEnumerable<RoomPopularity>> GetRoomPopularityAsync();
    Task<IEnumerable<RoomLoad>> GetRoomLoadRawAsync(DateTime from, DateTime to);
}
