using ConferenceBooking.Dal.SqlRepositories.Configuration;
using Microsoft.Data.SqlClient;

namespace ConferenceBooking.Dal.SqlRepositories.Connection;

public interface IDbConnectionFactory
{
    SqlDatabaseOptions Options { get; }
    Task<SqlConnection> CreateConnectionAsync(CancellationToken cancellationToken = default);
}
