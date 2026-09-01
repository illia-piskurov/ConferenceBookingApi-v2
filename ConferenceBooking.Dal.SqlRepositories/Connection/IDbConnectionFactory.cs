using ConferenceBooking.Dal.SqlRepositories.Configuration;
using Microsoft.Data.SqlClient;

namespace ConferenceBooking.Dal.SqlRepositories.Connection;

/// <summary>
/// Інтерфейс фабрики асинхронного створення підключень до бази даних SQL Server.
/// </summary>
public interface IDbConnectionFactory
{
    /// <summary>
    /// Конфігураційні параметри бази даних (наприклад, назва схеми).
    /// </summary>
    SqlDatabaseOptions Options { get; }

    /// <summary>
    /// Створити та асинхронно відкрити нове підключення до бази даних.
    /// </summary>
    /// <param name="cancellationToken">Токен скасування операції.</param>
    /// <returns>Відкритий екземпляр <see cref="SqlConnection"/>.</returns>
    Task<SqlConnection> CreateConnectionAsync(CancellationToken cancellationToken = default);
}
