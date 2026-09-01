using System.Data;
using System.Runtime.CompilerServices;
using ConferenceBooking.Dal.SqlRepositories.Configuration;
using Microsoft.Data.SqlClient;

namespace ConferenceBooking.Dal.SqlRepositories.Extensions;

public static class SqlCommandExtensions
{
    private static readonly ConditionalWeakTable<SqlConnection, SqlDatabaseOptions> ConnectionOptionsMap = new();
    private static readonly ConditionalWeakTable<SqlCommand, SqlDatabaseOptions> CommandOptionsMap = new();

    /// <summary>
    /// Реєструє налаштування схеми БД для конкретного підключення
    /// </summary>
    public static void RegisterConnectionOptions(this SqlConnection connection, SqlDatabaseOptions options)
    {
        ConnectionOptionsMap.AddOrUpdate(connection, options);
    }

    /// <summary>
    /// Створює SqlCommand для збереженої процедури з автоматичним підставленням схеми
    /// </summary>
    public static SqlCommand Procedure(this SqlConnection connection, string procedureName)
    {
        ConnectionOptionsMap.TryGetValue(connection, out var options);

        var qualifiedName = options != null ? options.QualifyProcedure(procedureName) : procedureName;

        var command = new SqlCommand(qualifiedName, connection)
        {
            CommandType = CommandType.StoredProcedure
        };

        if (options != null)
        {
            CommandOptionsMap.AddOrUpdate(command, options);
        }

        return command;
    }

    /// <summary>
    /// Додає параметр з автоматичним визначенням типу та обробкою null -> DBNull
    /// </summary>
    public static SqlCommand AddParam(this SqlCommand command, string name, object? value)
    {
        command.Parameters.AddWithValue(name, value ?? DBNull.Value);
        return command;
    }

    /// <summary>
    /// Додає строго типізований параметр
    /// </summary>
    public static SqlCommand AddParam(this SqlCommand command, string name, SqlDbType dbType, object? value)
    {
        var parameter = new SqlParameter(name, dbType)
        {
            Value = value ?? DBNull.Value
        };
        command.Parameters.Add(parameter);
        return command;
    }

    /// <summary>
    /// Додає параметр з точністю та масштабом (наприклад, для Decimal)
    /// </summary>
    public static SqlCommand AddParam(this SqlCommand command, string name, SqlDbType dbType, byte precision, byte scale, object? value)
    {
        var parameter = new SqlParameter(name, dbType)
        {
            Precision = precision,
            Scale = scale,
            Value = value ?? DBNull.Value
        };
        command.Parameters.Add(parameter);
        return command;
    }

    /// <summary>
    /// Додає InputOutput параметр
    /// </summary>
    public static SqlCommand AddInputOutputParam(
        this SqlCommand command,
        string name,
        SqlDbType dbType,
        object? value,
        out SqlParameter paramRef)
    {
        paramRef = new SqlParameter(name, dbType)
        {
            Direction = ParameterDirection.InputOutput,
            Value = (value == null || (value is Guid g && g == Guid.Empty)) ? DBNull.Value : value
        };
        command.Parameters.Add(paramRef);
        return command;
    }

    /// <summary>
    /// Додає Output параметр
    /// </summary>
    public static SqlCommand AddOutputParam(
        this SqlCommand command,
        string name,
        SqlDbType dbType,
        out SqlParameter paramRef)
    {
        paramRef = new SqlParameter(name, dbType)
        {
            Direction = ParameterDirection.Output
        };
        command.Parameters.Add(paramRef);
        return command;
    }

    /// <summary>
    /// Додає TVP параметр списку Guid з автоматичним визначенням типу з конфігурації схеми
    /// </summary>
    public static SqlCommand AddGuidTvpParam(
        this SqlCommand command,
        string name,
        IEnumerable<Guid>? ids)
    {
        CommandOptionsMap.TryGetValue(command, out var options);
        var typeName = options?.GuidListType ?? "dbo.GuidListType";

        return command.AddTvpParam(name, typeName, ids);
    }

    /// <summary>
    /// Додає Table-Valued Parameter (TVP) з явним ім'ям типу
    /// </summary>
    public static SqlCommand AddTvpParam(
        this SqlCommand command,
        string name,
        string typeName,
        IEnumerable<Guid>? ids)
    {
        var table = new DataTable();
        table.Columns.Add("Id", typeof(Guid));

        if (ids != null)
        {
            foreach (var id in ids)
            {
                table.Rows.Add(id);
            }
        }

        command.Parameters.Add(new SqlParameter(name, SqlDbType.Structured)
        {
            TypeName = typeName,
            Value = table
        });

        return command;
    }
}

public static class SqlDataReaderExtensions
{
    /// <summary>
    /// Читає значення колонки за ім'ям
    /// </summary>
    public static T Get<T>(this SqlDataReader reader, string columnName)
    {
        var ordinal = reader.GetOrdinal(columnName);
        return reader.GetFieldValue<T>(ordinal);
    }

    /// <summary>
    /// Безпечно читає nullable значення колонки за ім'ям
    /// </summary>
    public static T? GetNullable<T>(this SqlDataReader reader, string columnName) where T : struct
    {
        var ordinal = reader.GetOrdinal(columnName);
        return reader.IsDBNull(ordinal) ? null : reader.GetFieldValue<T>(ordinal);
    }

    /// <summary>
    /// Перевіряє наявність колонки у вибірці
    /// </summary>
    public static bool HasColumn(this SqlDataReader reader, string columnName)
    {
        for (var i = 0; i < reader.FieldCount; i++)
        {
            if (reader.GetName(i).Equals(columnName, StringComparison.OrdinalIgnoreCase))
                return true;
        }
        return false;
    }
}
