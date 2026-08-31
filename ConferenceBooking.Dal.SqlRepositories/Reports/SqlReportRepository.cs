using System.Data;
using ConferenceBooking.Bll.Common.Reports;
using ConferenceBooking.Bll.Common.Reports.Models;
using ConferenceBooking.Dal.SqlRepositories.Connection;
using ConferenceBooking.Dal.SqlRepositories.Constants;
using ConferenceBooking.Dal.SqlRepositories.Extensions;

namespace ConferenceBooking.Dal.SqlRepositories.Reports;

public class SqlReportRepository : IReportRepository
{
    private readonly IDbConnectionFactory _connectionFactory;

    public SqlReportRepository(IDbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<IEnumerable<DailyRevenue>> GetRevenueByDayAsync(DateTime from, DateTime to)
    {
        await using var connection = await _connectionFactory.CreateConnectionAsync();
        await using var command = connection
            .Procedure(SqlProcedures.Reports.GetRevenue)
            .AddParam("@From", SqlDbType.DateTime2, from)
            .AddParam("@To", SqlDbType.DateTime2, to);

        await using var reader = await command.ExecuteReaderAsync();

        var result = new List<DailyRevenue>();
        while (await reader.ReadAsync())
        {
            result.Add(new DailyRevenue
            {
                Date = reader.Get<DateTime>("Date"),
                Bookings = reader.Get<int>("Bookings"),
                Revenue = reader.Get<decimal>("Revenue")
            });
        }

        return result;
    }

    public async Task<IEnumerable<RoomPopularity>> GetRoomPopularityAsync()
    {
        await using var connection = await _connectionFactory.CreateConnectionAsync();
        await using var command = connection.Procedure(SqlProcedures.Reports.GetRoomPopularity);

        await using var reader = await command.ExecuteReaderAsync();

        var result = new List<RoomPopularity>();
        while (await reader.ReadAsync())
        {
            result.Add(new RoomPopularity
            {
                RoomId = reader.Get<Guid>("RoomId"),
                RoomName = reader.Get<string>("RoomName"),
                TotalBookings = reader.Get<int>("TotalBookings"),
                TotalRevenue = reader.Get<decimal>("TotalRevenue"),
                AverageBookingDurationHours = Convert.ToDouble(reader["AverageBookingDurationHours"])
            });
        }

        return result;
    }

    public async Task<IEnumerable<RoomLoad>> GetRoomLoadRawAsync(DateTime from, DateTime to)
    {
        await using var connection = await _connectionFactory.CreateConnectionAsync();
        await using var command = connection
            .Procedure(SqlProcedures.Reports.GetRoomLoad)
            .AddParam("@From", SqlDbType.DateTime2, from)
            .AddParam("@To", SqlDbType.DateTime2, to);

        await using var reader = await command.ExecuteReaderAsync();

        var result = new List<RoomLoad>();
        while (await reader.ReadAsync())
        {
            result.Add(new RoomLoad
            {
                RoomId = reader.Get<Guid>("RoomId"),
                RoomName = reader.Get<string>("RoomName"),
                BookedHours = Convert.ToDouble(reader["BookedHours"])
            });
        }

        return result;
    }
}
