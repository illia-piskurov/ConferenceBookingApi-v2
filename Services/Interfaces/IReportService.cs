using ConferenceBookingApi.DTOs.Reports;

namespace ConferenceBookingApi.Services.Interfaces;

public interface IReportService
{
    Task<RevenueReportDto> GetRevenueReportAsync(DateTime from, DateTime to);
    Task<IEnumerable<RoomPopularityDto>> GetRoomPopularityAsync();
    Task<IEnumerable<RoomLoadDto>> GetRoomLoadAsync(DateTime from, DateTime to);
}
