using Microsoft.Data.SqlClient;

namespace ConferenceBookingApi.Data.Repositories.Interfaces;

public interface IDbConnectionFactory
{
    Task<SqlConnection> CreateConnectionAsync(CancellationToken cancellationToken = default);
}
