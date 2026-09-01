using System.Data;
using AutoMapper;
using Microsoft.Data.SqlClient;
using ConferenceBooking.Bll.Common.Rooms;
using ConferenceBooking.Bll.Common.Rooms.Models;
using ConferenceBooking.Bll.Common.Shared.Exceptions;
using ConferenceBooking.Dal.SqlRepositories.Connection;
using ConferenceBooking.Dal.SqlRepositories.Constants;
using ConferenceBooking.Dal.SqlRepositories.Extensions;
using ConferenceBooking.Dal.SqlRepositories.Rooms.Entities;

namespace ConferenceBooking.Dal.SqlRepositories.Rooms;

public class SqlRoomRepository : IRoomRepository
{
    private readonly IDbConnectionFactory _connectionFactory;
    private readonly IMapper _mapper;

    public SqlRoomRepository(IDbConnectionFactory connectionFactory, IMapper mapper)
    {
        _connectionFactory = connectionFactory;
        _mapper = mapper;
    }

    public async Task<IEnumerable<Room>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        await using var connection = await _connectionFactory.CreateConnectionAsync(cancellationToken);
        await using var command = connection.Procedure(SqlProcedures.Rooms.GetAll);

        return await ReadRoomsWithServicesAsync(command, cancellationToken);
    }

    public async Task<IEnumerable<Room>> SearchAvailableAsync(DateTime start, DateTime end, int capacity, CancellationToken cancellationToken = default)
    {
        await using var connection = await _connectionFactory.CreateConnectionAsync(cancellationToken);
        await using var command = connection
            .Procedure(SqlProcedures.Rooms.SearchAvailable)
            .AddParam("@StartTime", SqlDbType.DateTime2, start)
            .AddParam("@EndTime", SqlDbType.DateTime2, end)
            .AddParam("@Capacity", SqlDbType.Int, capacity);

        return await ReadRoomsWithServicesAsync(command, cancellationToken);
    }

    public async Task<Room?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        await using var connection = await _connectionFactory.CreateConnectionAsync(cancellationToken);
        await using var command = connection
            .Procedure(SqlProcedures.Rooms.GetById)
            .AddParam("@Id", SqlDbType.UniqueIdentifier, id);

        await using var reader = await command.ExecuteReaderAsync(cancellationToken);

        if (!await reader.ReadAsync(cancellationToken))
            return null;

        var entity = MapRoomEntity(reader);
        var room = _mapper.Map<Room>(entity);

        if (await reader.NextResultAsync(cancellationToken))
        {
            while (await reader.ReadAsync(cancellationToken))
            {
                var serviceEntity = MapServiceEntity(reader);
                room.AvailableServices.Add(_mapper.Map<Service>(serviceEntity));
            }
        }

        return room;
    }

    public async Task<Room> AddAsync(Room room, CancellationToken cancellationToken = default)
    {
        await using var connection = await _connectionFactory.CreateConnectionAsync(cancellationToken);
        await using var command = connection
            .Procedure(SqlProcedures.Rooms.Insert)
            .AddInputOutputParam("@Id", SqlDbType.UniqueIdentifier, room.Id, out var idParam)
            .AddParam("@Name", SqlDbType.NVarChar, room.Name)
            .AddParam("@Capacity", SqlDbType.Int, room.Capacity)
            .AddParam("@BaseHourlyRate", SqlDbType.Decimal, 18, 2, room.BaseHourlyRate)
            .AddGuidTvpParam("@ServiceIds", room.AvailableServices.Select(s => s.Id));

        await command.ExecuteNonQueryAsync(cancellationToken);

        room.Id = (Guid)idParam.Value;
        return room;
    }

    public async Task<Room> UpdateAsync(Room room, CancellationToken cancellationToken = default)
    {
        await using var connection = await _connectionFactory.CreateConnectionAsync(cancellationToken);
        await using var command = connection
            .Procedure(SqlProcedures.Rooms.Update)
            .AddParam("@Id", SqlDbType.UniqueIdentifier, room.Id)
            .AddParam("@Name", SqlDbType.NVarChar, room.Name)
            .AddParam("@Capacity", SqlDbType.Int, room.Capacity)
            .AddParam("@BaseHourlyRate", SqlDbType.Decimal, 18, 2, room.BaseHourlyRate)
            .AddGuidTvpParam("@ServiceIds", room.AvailableServices.Select(s => s.Id));

        await command.ExecuteNonQueryAsync(cancellationToken);
        return room;
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        await using var connection = await _connectionFactory.CreateConnectionAsync(cancellationToken);
        await using var command = connection
            .Procedure(SqlProcedures.Rooms.Delete)
            .AddParam("@Id", SqlDbType.UniqueIdentifier, id);

        try
        {
            await command.ExecuteNonQueryAsync(cancellationToken);
        }
        catch (SqlException ex) when (ex.Number == 50002)
        {
            throw new RoomHasActiveBookingsException(id);
        }
    }

    private async Task<List<Room>> ReadRoomsWithServicesAsync(SqlCommand command, CancellationToken cancellationToken)
    {
        await using var reader = await command.ExecuteReaderAsync(cancellationToken);

        var rooms = new Dictionary<Guid, Room>();

        while (await reader.ReadAsync(cancellationToken))
        {
            var entity = MapRoomEntity(reader);
            var room = _mapper.Map<Room>(entity);
            rooms[room.Id] = room;
        }

        if (await reader.NextResultAsync(cancellationToken))
        {
            while (await reader.ReadAsync(cancellationToken))
            {
                var roomId = reader.Get<Guid>("RoomId");
                if (rooms.TryGetValue(roomId, out var room))
                {
                    var serviceEntity = MapServiceEntity(reader);
                    room.AvailableServices.Add(_mapper.Map<Service>(serviceEntity));
                }
            }
        }

        return rooms.Values.ToList();
    }

    private static RoomEntity MapRoomEntity(SqlDataReader reader) => new()
    {
        Id = reader.Get<Guid>("Id"),
        Name = reader.Get<string>("Name"),
        Capacity = reader.Get<int>("Capacity"),
        BaseHourlyRate = reader.Get<decimal>("BaseHourlyRate"),
        IsDeleted = reader.Get<bool>("IsDeleted")
    };

    private static ServiceEntity MapServiceEntity(SqlDataReader reader)
    {
        var id = reader.HasColumn("ServiceId")
            ? reader.Get<Guid>("ServiceId")
            : reader.Get<Guid>("Id");

        return new ServiceEntity
        {
            Id = id,
            Name = reader.Get<string>("Name"),
            Price = reader.Get<decimal>("Price")
        };
    }
}
