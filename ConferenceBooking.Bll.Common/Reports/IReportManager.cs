using ConferenceBooking.Bll.Common.Reports.Models;

namespace ConferenceBooking.Bll.Common.Reports;

public interface IReportManager
{
    Task<RevenueReport> GetRevenueReportAsync(DateTime from, DateTime to, CancellationToken cancellationToken = default);
    Task<IEnumerable<RoomPopularity>> GetRoomPopularityAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<RoomLoad>> GetRoomLoadAsync(DateTime from, DateTime to, CancellationToken cancellationToken = default);
}
