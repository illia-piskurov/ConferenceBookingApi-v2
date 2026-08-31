using ConferenceBooking.Dal.SqlRepositories.Configuration;
using ConferenceBooking.Dal.SqlRepositories.Extensions;
using Microsoft.Data.SqlClient;

namespace ConferenceBooking.Dal.SqlRepositories.Connection;

public class SqlConnectionFactory : IDbConnectionFactory
{
    private readonly string _connectionString;

    public SqlDatabaseOptions Options { get; }

    public SqlConnectionFactory(string connectionString, SqlDatabaseOptions? options = null)
    {
        _connectionString = connectionString ?? throw new ArgumentNullException(nameof(connectionString));
        Options = options ?? new SqlDatabaseOptions();
    }

    public async Task<SqlConnection> CreateConnectionAsync(CancellationToken cancellationToken = default)
    {
        var connection = new SqlConnection(_connectionString);
        connection.RegisterConnectionOptions(Options);
        await connection.OpenAsync(cancellationToken);
        return connection;
    }
}
