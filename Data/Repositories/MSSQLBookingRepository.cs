using System.Data;
using Microsoft.Data.SqlClient;
using ConferenceBookingApi.Data.Repositories.Interfaces;
using ConferenceBookingApi.Exceptions;
using ConferenceBookingApi.Models;

namespace ConferenceBookingApi.Data.Repositories;

public class SqlBookingRepository(IDbConnectionFactory connectionFactory) : IBookingRepository
{
    public async Task<IEnumerable<Booking>> GetAllAsync()
    {
        await using var connection = await connectionFactory.CreateConnectionAsync();
        await using var command = new SqlCommand("IPiskurovSchema.sp_Bookings_GetAll", connection)
        {
            CommandType = CommandType.StoredProcedure
        };

        return await ReadBookingsWithServicesAsync(command);
    }

    public async Task<Booking?> GetByIdAsync(Guid id)
    {
        await using var connection = await connectionFactory.CreateConnectionAsync();
        await using var command = new SqlCommand("IPiskurovSchema.sp_Bookings_GetById", connection)
        {
            CommandType = CommandType.StoredProcedure
        };

        command.Parameters.Add(new SqlParameter("@Id", SqlDbType.UniqueIdentifier) { Value = id });

        await using var reader = await command.ExecuteReaderAsync();

        if (!await reader.ReadAsync())
            return null;

        var booking = MapBooking(reader);

        if (await reader.NextResultAsync())
        {
            while (await reader.ReadAsync())
            {
                booking.SelectedServiceIds.Add(reader.GetGuid(reader.GetOrdinal("ServiceId")));
            }
        }

        return booking;
    }

    public async Task<IEnumerable<Booking>> GetByRoomIdAsync(Guid roomId)
    {
        await using var connection = await connectionFactory.CreateConnectionAsync();
        await using var command = new SqlCommand("IPiskurovSchema.sp_Bookings_GetByRoomId", connection)
        {
            CommandType = CommandType.StoredProcedure
        };

        command.Parameters.Add(new SqlParameter("@RoomId", SqlDbType.UniqueIdentifier) { Value = roomId });

        return await ReadBookingsWithServicesAsync(command);
    }

    public async Task<IEnumerable<Booking>> GetOverlappingAsync(Guid roomId, DateTime start, DateTime end)
    {
        await using var connection = await connectionFactory.CreateConnectionAsync();
        await using var command = new SqlCommand("IPiskurovSchema.sp_Bookings_GetOverlapping", connection)
        {
            CommandType = CommandType.StoredProcedure
        };

        command.Parameters.Add(new SqlParameter("@RoomId", SqlDbType.UniqueIdentifier) { Value = roomId });
        command.Parameters.Add(new SqlParameter("@StartTime", SqlDbType.DateTime2) { Value = start });
        command.Parameters.Add(new SqlParameter("@EndTime", SqlDbType.DateTime2) { Value = end });

        return await ReadBookingsWithServicesAsync(command);
    }

    public async Task<IEnumerable<Booking>> GetByDateRangeAsync(DateTime from, DateTime to)
    {
        await using var connection = await connectionFactory.CreateConnectionAsync();
        await using var command = new SqlCommand("IPiskurovSchema.sp_Bookings_GetByDateRange", connection)
        {
            CommandType = CommandType.StoredProcedure
        };

        command.Parameters.Add(new SqlParameter("@From", SqlDbType.DateTime2) { Value = from });
        command.Parameters.Add(new SqlParameter("@To", SqlDbType.DateTime2) { Value = to });

        return await ReadBookingsWithServicesAsync(command);
    }

    public async Task<Booking> AddAsync(Booking booking)
    {
        await using var connection = await connectionFactory.CreateConnectionAsync();
        await using var command = new SqlCommand("IPiskurovSchema.sp_Bookings_Insert", connection)
        {
            CommandType = CommandType.StoredProcedure
        };

        var idParam = new SqlParameter("@Id", SqlDbType.UniqueIdentifier)
        {
            Direction = ParameterDirection.InputOutput,
            Value = booking.Id == Guid.Empty ? DBNull.Value : booking.Id
        };
        var createdAtParam = new SqlParameter("@CreatedAt", SqlDbType.DateTime2)
        {
            Direction = ParameterDirection.Output
        };

        command.Parameters.Add(idParam);
        command.Parameters.Add(new SqlParameter("@RoomId", SqlDbType.UniqueIdentifier) { Value = booking.RoomId });
        command.Parameters.Add(new SqlParameter("@StartTime", SqlDbType.DateTime2) { Value = booking.StartTime });
        command.Parameters.Add(new SqlParameter("@EndTime", SqlDbType.DateTime2) { Value = booking.EndTime });
        command.Parameters.Add(new SqlParameter("@TotalCost", SqlDbType.Decimal) { Value = booking.TotalCost, Precision = 18, Scale = 2 });
        command.Parameters.Add(createdAtParam);

        AddServiceIdsParameter(command, booking.SelectedServiceIds);

        try
        {
            await command.ExecuteNonQueryAsync();
        }
        catch (SqlException ex) when (ex.Number == 50001)
        {
            throw new BookingConflictException(booking.RoomId, booking.StartTime, booking.EndTime);
        }

        booking.Id = (Guid)idParam.Value;
        booking.CreatedAt = (DateTime)createdAtParam.Value;

        return booking;
    }

    // Хелпер для параллельного маппинга двух рекордсетов
    private static async Task<List<Booking>> ReadBookingsWithServicesAsync(SqlCommand command)
    {
        await using var reader = await command.ExecuteReaderAsync();

        var bookings = new Dictionary<Guid, Booking>();

        while (await reader.ReadAsync())
        {
            var booking = MapBooking(reader);
            bookings[booking.Id] = booking;
        }

        if (await reader.NextResultAsync())
        {
            while (await reader.ReadAsync())
            {
                var bookingId = reader.GetGuid(reader.GetOrdinal("BookingId"));
                var serviceId = reader.GetGuid(reader.GetOrdinal("ServiceId"));

                if (bookings.TryGetValue(bookingId, out var booking))
                {
                    booking.SelectedServiceIds.Add(serviceId);
                }
            }
        }

        return bookings.Values.ToList();
    }

    private static void AddServiceIdsParameter(SqlCommand command, IEnumerable<Guid> serviceIds)
    {
        var table = new DataTable();
        table.Columns.Add("Id", typeof(Guid));

        foreach (var serviceId in serviceIds)
        {
            table.Rows.Add(serviceId);
        }

        command.Parameters.Add(new SqlParameter("@ServiceIds", SqlDbType.Structured)
        {
            TypeName = "IPiskurovSchema.GuidListType",
            Value = table
        });
    }

    private static Booking MapBooking(SqlDataReader reader) => new()
    {
        Id = reader.GetGuid(reader.GetOrdinal("Id")),
        RoomId = reader.GetGuid(reader.GetOrdinal("RoomId")),
        StartTime = reader.GetDateTime(reader.GetOrdinal("StartTime")),
        EndTime = reader.GetDateTime(reader.GetOrdinal("EndTime")),
        TotalCost = reader.GetDecimal(reader.GetOrdinal("TotalCost")),
        CreatedAt = reader.GetDateTime(reader.GetOrdinal("CreatedAt")),
        SelectedServiceIds = new List<Guid>()
    };
}