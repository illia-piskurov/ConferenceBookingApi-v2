using System.Data;
using Microsoft.Data.SqlClient;
using ConferenceBookingApi.Data.Repositories.Interfaces;
using ConferenceBookingApi.Models;

namespace ConferenceBookingApi.Data.Repositories;

public class SqlRoomRepository(IDbConnectionFactory connectionFactory) : IRoomRepository
{
    public async Task<IEnumerable<Room>> GetAllAsync()
    {
        await using var connection = await connectionFactory.CreateConnectionAsync();
        await using var command = new SqlCommand("IPiskurovSchema.sp_Rooms_GetAll", connection)
        {
            CommandType = CommandType.StoredProcedure
        };

        await using var reader = await command.ExecuteReaderAsync();

        var rooms = new Dictionary<Guid, Room>();

        while (await reader.ReadAsync())
        {
            var room = MapRoom(reader);
            rooms[room.Id] = room;
        }

        if (await reader.NextResultAsync())
        {
            while (await reader.ReadAsync())
            {
                var roomId = reader.GetGuid(reader.GetOrdinal("RoomId"));
                if (rooms.TryGetValue(roomId, out var room))
                {
                    room.AvailableServices.Add(new Service
                    {
                        Id = reader.GetGuid(reader.GetOrdinal("ServiceId")),
                        Name = reader.GetString(reader.GetOrdinal("Name")),
                        Price = reader.GetDecimal(reader.GetOrdinal("Price"))
                    });
                }
            }
        }

        return rooms.Values;
    }

    public async Task<Room?> GetByIdAsync(Guid id)
    {
        await using var connection = await connectionFactory.CreateConnectionAsync();
        await using var command = new SqlCommand("IPiskurovSchema.sp_Rooms_GetById", connection)
        {
            CommandType = CommandType.StoredProcedure
        };

        command.Parameters.Add(new SqlParameter("@Id", SqlDbType.UniqueIdentifier) { Value = id });

        await using var reader = await command.ExecuteReaderAsync();

        if (!await reader.ReadAsync())
            return null;

        var room = MapRoom(reader);

        if (await reader.NextResultAsync())
        {
            while (await reader.ReadAsync())
            {
                room.AvailableServices.Add(MapService(reader));
            }
        }

        return room;
    }

    public async Task<Room> AddAsync(Room room)
    {
        await using var connection = await connectionFactory.CreateConnectionAsync();
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
        await using var connection = await connectionFactory.CreateConnectionAsync();
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
        await using var connection = await connectionFactory.CreateConnectionAsync();
        await using var command = new SqlCommand("IPiskurovSchema.sp_Rooms_Delete", connection)
        {
            CommandType = CommandType.StoredProcedure
        };

        command.Parameters.Add(new SqlParameter("@Id", SqlDbType.UniqueIdentifier) { Value = id });
        await command.ExecuteNonQueryAsync();
    }

    // Хелпер для передачи TVP (Table-Valued Parameter)
    private static void AddServiceIdsParameter(SqlCommand command, IEnumerable<Guid> serviceIds)
    {
        var table = new DataTable();
        table.Columns.Add("Id", typeof(Guid));

        foreach (var serviceId in serviceIds)
        {
            table.Rows.Add(serviceId);
        }

        var param = command.Parameters.Add(new SqlParameter("@ServiceIds", SqlDbType.Structured)
        {
            TypeName = "IPiskurovSchema.GuidListType",
            Value = table
        });
    }

    private static Room MapRoom(SqlDataReader reader) => new()
    {
        Id = reader.GetGuid(reader.GetOrdinal("Id")),
        Name = reader.GetString(reader.GetOrdinal("Name")),
        Capacity = reader.GetInt32(reader.GetOrdinal("Capacity")),
        BaseHourlyRate = reader.GetDecimal(reader.GetOrdinal("BaseHourlyRate")),
        IsDeleted = reader.GetBoolean(reader.GetOrdinal("IsDeleted")),
        AvailableServices = new List<Service>()
    };

    private static Service MapService(SqlDataReader reader) => new()
    {
        Id = reader.GetGuid(reader.GetOrdinal("Id")),
        Name = reader.GetString(reader.GetOrdinal("Name")),
        Price = reader.GetDecimal(reader.GetOrdinal("Price"))
    };
}