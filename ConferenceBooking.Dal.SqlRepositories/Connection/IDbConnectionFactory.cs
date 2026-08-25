using Microsoft.Data.SqlClient;

namespace ConferenceBooking.Dal.SqlRepositories.Connection;

public interface IDbConnectionFactory
{
    Task<SqlConnection> CreateConnectionAsync(CancellationToken cancellationToken = default);
}
