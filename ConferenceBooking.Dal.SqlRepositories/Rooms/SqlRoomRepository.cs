using System.Data;
using AutoMapper;
using Microsoft.Data.SqlClient;
using ConferenceBooking.Bll.Common.Rooms;
using ConferenceBooking.Bll.Common.Rooms.Models;
using ConferenceBooking.Dal.SqlRepositories.Connection;
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
        await using var command = new SqlCommand("IPiskurovSchema.sp_Rooms_GetAll", connection)
        {
            CommandType = CommandType.StoredProcedure
        };

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
                var roomId = reader.GetGuid(reader.GetOrdinal("RoomId"));
                if (rooms.TryGetValue(roomId, out var room))
                {
                    var serviceEntity = new ServiceEntity
                    {
                        Id = reader.GetGuid(reader.GetOrdinal("ServiceId")),
                        Name = reader.GetString(reader.GetOrdinal("Name")),
                        Price = reader.GetDecimal(reader.GetOrdinal("Price"))
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
        await using var command = new SqlCommand("IPiskurovSchema.sp_Rooms_GetById", connection)
        {
            CommandType = CommandType.StoredProcedure
        };

        command.Parameters.Add(new SqlParameter("@Id", SqlDbType.UniqueIdentifier) { Value = id });

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
        await using var command = new SqlCommand("IPiskurovSchema.sp_Rooms_Insert", connection)
        {
            CommandType = CommandType.StoredProcedure
        };

        var idParam = new SqlParameter("@Id", SqlDbType.UniqueIdentifier)
        {
            Direction = ParameterDirection.InputOutput,
            Value = room.Id == Guid.Empty ? DBNull.Value : room.Id
        };
        command.Parameters.Add(idParam);
        command.Parameters.Add(new SqlParameter("@Name", SqlDbType.NVarChar, 150) { Value = room.Name });
        command.Parameters.Add(new SqlParameter("@Capacity", SqlDbType.Int) { Value = room.Capacity });
        command.Parameters.Add(new SqlParameter("@BaseHourlyRate", SqlDbType.Decimal) { Value = room.BaseHourlyRate, Precision = 18, Scale = 2 });

        AddServiceIdsParameter(command, room.AvailableServices.Select(s => s.Id));

        await command.ExecuteNonQueryAsync();

        room.Id = (Guid)idParam.Value;
        return room;
    }

    public async Task<Room> UpdateAsync(Room room)
    {
        await using var connection = await _connectionFactory.CreateConnectionAsync();
        await using var command = new SqlCommand("IPiskurovSchema.sp_Rooms_Update", connection)
        {
            CommandType = CommandType.StoredProcedure
        };

        command.Parameters.Add(new SqlParameter("@Id", SqlDbType.UniqueIdentifier) { Value = room.Id });
        command.Parameters.Add(new SqlParameter("@Name", SqlDbType.NVarChar, 150) { Value = room.Name });
        command.Parameters.Add(new SqlParameter("@Capacity", SqlDbType.Int) { Value = room.Capacity });
        command.Parameters.Add(new SqlParameter("@BaseHourlyRate", SqlDbType.Decimal) { Value = room.BaseHourlyRate, Precision = 18, Scale = 2 });

        AddServiceIdsParameter(command, room.AvailableServices.Select(s => s.Id));

        await command.ExecuteNonQueryAsync();
        return room;
    }

    public async Task DeleteAsync(Guid id)
    {
        await using var connection = await _connectionFactory.CreateConnectionAsync();
        await using var command = new SqlCommand("IPiskurovSchema.sp_Rooms_Delete", connection)
        {
            CommandType = CommandType.StoredProcedure
        };

        command.Parameters.Add(new SqlParameter("@Id", SqlDbType.UniqueIdentifier) { Value = id });
        await command.ExecuteNonQueryAsync();
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

    private static RoomEntity MapRoomEntity(SqlDataReader reader) => new()
    {
        Id = reader.GetGuid(reader.GetOrdinal("Id")),
        Name = reader.GetString(reader.GetOrdinal("Name")),
        Capacity = reader.GetInt32(reader.GetOrdinal("Capacity")),
        BaseHourlyRate = reader.GetDecimal(reader.GetOrdinal("BaseHourlyRate")),
        IsDeleted = reader.GetBoolean(reader.GetOrdinal("IsDeleted"))
    };

    private static ServiceEntity MapServiceEntity(SqlDataReader reader) => new()
    {
        Id = reader.GetGuid(reader.GetOrdinal("Id")),
        Name = reader.GetString(reader.GetOrdinal("Name")),
        Price = reader.GetDecimal(reader.GetOrdinal("Price"))
    };
}
