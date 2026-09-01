using ConferenceBooking.Bll.Common.Reports.Models;

namespace ConferenceBooking.Bll.Common.Reports;

public interface IReportManager
{
    Task<RevenueReport> GetRevenueReportAsync(DateTime from, DateTime to);
    Task<IEnumerable<RoomPopularity>> GetRoomPopularityAsync();
    Task<IEnumerable<RoomLoad>> GetRoomLoadAsync(DateTime from, DateTime to);
}
