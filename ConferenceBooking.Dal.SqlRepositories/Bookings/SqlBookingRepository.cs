using System.Data;
using AutoMapper;
using Microsoft.Data.SqlClient;
using ConferenceBooking.Bll.Common.Bookings;
using ConferenceBooking.Bll.Common.Bookings.Models;
using ConferenceBooking.Bll.Common.Shared.Exceptions;
using ConferenceBooking.Dal.SqlRepositories.Bookings.Entities;
using ConferenceBooking.Dal.SqlRepositories.Connection;
using ConferenceBooking.Dal.SqlRepositories.Constants;
using ConferenceBooking.Dal.SqlRepositories.Extensions;

namespace ConferenceBooking.Dal.SqlRepositories.Bookings;

public class SqlBookingRepository : IBookingRepository
{
    private readonly IDbConnectionFactory _connectionFactory;
    private readonly IMapper _mapper;

    public SqlBookingRepository(IDbConnectionFactory connectionFactory, IMapper mapper)
    {
        _connectionFactory = connectionFactory;
        _mapper = mapper;
    }

    public async Task<IEnumerable<Booking>> GetAllAsync()
    {
        await using var connection = await _connectionFactory.CreateConnectionAsync();
        await using var command = connection.Procedure(SqlProcedures.Bookings.GetAll);

        return await ReadBookingsWithServicesAsync(command);
    }

    public async Task<Booking?> GetByIdAsync(Guid id)
    {
        await using var connection = await _connectionFactory.CreateConnectionAsync();
        await using var command = connection
            .Procedure(SqlProcedures.Bookings.GetById)
            .AddParam("@Id", SqlDbType.UniqueIdentifier, id);

        await using var reader = await command.ExecuteReaderAsync();

        if (!await reader.ReadAsync())
            return null;

        var entity = MapBookingEntity(reader);
        var booking = _mapper.Map<Booking>(entity);

        if (await reader.NextResultAsync())
        {
            while (await reader.ReadAsync())
            {
                booking.SelectedServiceIds.Add(reader.Get<Guid>("ServiceId"));
            }
        }

        return booking;
    }

    public async Task<IEnumerable<Booking>> GetByRoomIdAsync(Guid roomId)
    {
        await using var connection = await _connectionFactory.CreateConnectionAsync();
        await using var command = connection
            .Procedure(SqlProcedures.Bookings.GetByRoomId)
            .AddParam("@RoomId", SqlDbType.UniqueIdentifier, roomId);

        return await ReadBookingsWithServicesAsync(command);
    }

    public async Task<IEnumerable<Booking>> GetOverlappingAsync(Guid roomId, DateTime start, DateTime end)
    {
        await using var connection = await _connectionFactory.CreateConnectionAsync();
        await using var command = connection
            .Procedure(SqlProcedures.Bookings.GetOverlapping)
            .AddParam("@RoomId", SqlDbType.UniqueIdentifier, roomId)
            .AddParam("@StartTime", SqlDbType.DateTime2, start)
            .AddParam("@EndTime", SqlDbType.DateTime2, end);

        return await ReadBookingsWithServicesAsync(command);
    }

    public async Task<IEnumerable<Booking>> GetByDateRangeAsync(DateTime from, DateTime to)
    {
        await using var connection = await _connectionFactory.CreateConnectionAsync();
        await using var command = connection
            .Procedure(SqlProcedures.Bookings.GetByDateRange)
            .AddParam("@From", SqlDbType.DateTime2, from)
            .AddParam("@To", SqlDbType.DateTime2, to);

        return await ReadBookingsWithServicesAsync(command);
    }

    public async Task<Booking> AddAsync(Booking booking)
    {
        await using var connection = await _connectionFactory.CreateConnectionAsync();
        await using var command = connection
            .Procedure(SqlProcedures.Bookings.Insert)
            .AddInputOutputParam("@Id", SqlDbType.UniqueIdentifier, booking.Id, out var idParam)
            .AddParam("@RoomId", SqlDbType.UniqueIdentifier, booking.RoomId)
            .AddParam("@StartTime", SqlDbType.DateTime2, booking.StartTime)
            .AddParam("@EndTime", SqlDbType.DateTime2, booking.EndTime)
            .AddParam("@TotalCost", SqlDbType.Decimal, 18, 2, booking.TotalCost)
            .AddOutputParam("@CreatedAt", SqlDbType.DateTime2, out var createdAtParam)
            .AddGuidTvpParam("@ServiceIds", booking.SelectedServiceIds);

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

    private async Task<List<Booking>> ReadBookingsWithServicesAsync(SqlCommand command)
    {
        await using var reader = await command.ExecuteReaderAsync();

        var bookings = new Dictionary<Guid, Booking>();

        while (await reader.ReadAsync())
        {
            var entity = MapBookingEntity(reader);
            var booking = _mapper.Map<Booking>(entity);
            bookings[booking.Id] = booking;
        }

        if (await reader.NextResultAsync())
        {
            while (await reader.ReadAsync())
            {
                var bookingId = reader.Get<Guid>("BookingId");
                var serviceId = reader.Get<Guid>("ServiceId");

                if (bookings.TryGetValue(bookingId, out var booking))
                {
                    booking.SelectedServiceIds.Add(serviceId);
                }
            }
        }

        return bookings.Values.ToList();
    }

    private static BookingEntity MapBookingEntity(SqlDataReader reader) => new()
    {
        Id = reader.Get<Guid>("Id"),
        RoomId = reader.Get<Guid>("RoomId"),
        StartTime = reader.Get<DateTime>("StartTime"),
        EndTime = reader.Get<DateTime>("EndTime"),
        TotalCost = reader.Get<decimal>("TotalCost"),
        CreatedAt = reader.Get<DateTime>("CreatedAt")
    };
}
