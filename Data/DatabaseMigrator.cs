using System.Reflection;
using DbUp;
using DbUp.Engine;

namespace ConferenceBookingApi.Data;

public static class DatabaseMigrator
{
    public static DatabaseUpgradeResult MigrateDatabase(string connectionString)
    {
        if (!connectionString.Contains("database.windows.net", StringComparison.OrdinalIgnoreCase))
        {
            try
            {
                EnsureDatabase.For.SqlDatabase(connectionString);
            }
            catch
            {
                // In case user doesn't have permissions to master DB, continue to migrate existing DB
            }
        }

        var upgrader = DeployChanges.To
            .SqlDatabase(connectionString)
            .WithScriptsEmbeddedInAssembly(Assembly.GetExecutingAssembly())
            .JournalToSqlTable("IPiskurovSchema", "SchemaVersions")
            .WithTransactionPerScript()
            .LogToConsole()
            .Build();

        var result = upgrader.PerformUpgrade();

        if (!result.Successful)
        {
            throw new InvalidOperationException($"DbUp migration failed: {result.Error.Message}", result.Error);
        }

        return result;
    }
}
