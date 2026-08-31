using System.Data;
using AutoMapper;
using Microsoft.Data.SqlClient;
using ConferenceBooking.Bll.Common.Rooms;
using ConferenceBooking.Bll.Common.Rooms.Models;
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

    public async Task<IEnumerable<Room>> GetAllAsync()
    {
        await using var connection = await _connectionFactory.CreateConnectionAsync();
        await using var command = connection.Procedure(SqlProcedures.Rooms.GetAll);
        await using var reader = await command.ExecuteReaderAsync();

        var rooms = new Dictionary<Guid, Room>();

        while (await reader.ReadAsync())
        {
            var entity = MapRoomEntity(reader);
            var room = _mapper.Map<Room>(entity);
            rooms[room.Id] = room;
        }

        if (await reader.NextResultAsync())
        {
            while (await reader.ReadAsync())
            {
                var roomId = reader.Get<Guid>("RoomId");
                if (rooms.TryGetValue(roomId, out var room))
                {
                    var serviceEntity = new ServiceEntity
                    {
                        Id = reader.Get<Guid>("ServiceId"),
                        Name = reader.Get<string>("Name"),
                        Price = reader.Get<decimal>("Price")
                    };
                    room.AvailableServices.Add(_mapper.Map<Service>(serviceEntity));
                }
            }
        }

        return rooms.Values;
    }

    public async Task<Room?> GetByIdAsync(Guid id)
    {
        await using var connection = await _connectionFactory.CreateConnectionAsync();
        await using var command = connection
            .Procedure(SqlProcedures.Rooms.GetById)
            .AddParam("@Id", SqlDbType.UniqueIdentifier, id);

        await using var reader = await command.ExecuteReaderAsync();

        if (!await reader.ReadAsync())
            return null;

        var entity = MapRoomEntity(reader);
        var room = _mapper.Map<Room>(entity);

        if (await reader.NextResultAsync())
        {
            while (await reader.ReadAsync())
            {
                var serviceEntity = MapServiceEntity(reader);
                room.AvailableServices.Add(_mapper.Map<Service>(serviceEntity));
            }
        }

        return room;
    }

    public async Task<Room> AddAsync(Room room)
    {
        await using var connection = await _connectionFactory.CreateConnectionAsync();
        await using var command = connection
            .Procedure(SqlProcedures.Rooms.Insert)
            .AddInputOutputParam("@Id", SqlDbType.UniqueIdentifier, room.Id, out var idParam)
            .AddParam("@Name", SqlDbType.NVarChar, room.Name)
            .AddParam("@Capacity", SqlDbType.Int, room.Capacity)
            .AddParam("@BaseHourlyRate", SqlDbType.Decimal, 18, 2, room.BaseHourlyRate)
            .AddGuidTvpParam("@ServiceIds", room.AvailableServices.Select(s => s.Id));

        await command.ExecuteNonQueryAsync();

        room.Id = (Guid)idParam.Value;
        return room;
    }

    public async Task<Room> UpdateAsync(Room room)
    {
        await using var connection = await _connectionFactory.CreateConnectionAsync();
        await using var command = connection
            .Procedure(SqlProcedures.Rooms.Update)
            .AddParam("@Id", SqlDbType.UniqueIdentifier, room.Id)
            .AddParam("@Name", SqlDbType.NVarChar, room.Name)
            .AddParam("@Capacity", SqlDbType.Int, room.Capacity)
            .AddParam("@BaseHourlyRate", SqlDbType.Decimal, 18, 2, room.BaseHourlyRate)
            .AddGuidTvpParam("@ServiceIds", room.AvailableServices.Select(s => s.Id));

        await command.ExecuteNonQueryAsync();
        return room;
    }

    public async Task DeleteAsync(Guid id)
    {
        await using var connection = await _connectionFactory.CreateConnectionAsync();
        await using var command = connection
            .Procedure(SqlProcedures.Rooms.Delete)
            .AddParam("@Id", SqlDbType.UniqueIdentifier, id);

        await command.ExecuteNonQueryAsync();
    }

    private static RoomEntity MapRoomEntity(SqlDataReader reader) => new()
    {
        Id = reader.Get<Guid>("Id"),
        Name = reader.Get<string>("Name"),
        Capacity = reader.Get<int>("Capacity"),
        BaseHourlyRate = reader.Get<decimal>("BaseHourlyRate"),
        IsDeleted = reader.Get<bool>("IsDeleted")
    };

    private static ServiceEntity MapServiceEntity(SqlDataReader reader) => new()
    {
        Id = reader.Get<Guid>("Id"),
        Name = reader.Get<string>("Name"),
        Price = reader.Get<decimal>("Price")
    };
}
