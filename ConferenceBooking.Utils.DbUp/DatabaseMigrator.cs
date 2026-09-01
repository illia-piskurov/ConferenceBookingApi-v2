using System.Reflection;
using DbUp;
using DbUp.Engine;

namespace ConferenceBooking.Utils.DbUp;

public static class DatabaseMigrator
{
    public static DatabaseUpgradeResult MigrateSqlServer(
        string connectionString,
        Assembly scriptsAssembly,
        string journalSchema = "dbo",
        string journalTable = "SchemaVersions")
    {
        if (!connectionString.Contains("database.windows.net", StringComparison.OrdinalIgnoreCase))
        {
            try
            {
                EnsureDatabase.For.SqlDatabase(connectionString);
            }
            catch
            {
                // If user doesn't have master DB permissions, proceed to migrate existing DB
            }
        }

        var upgrader = DeployChanges.To
            .SqlDatabase(connectionString)
            .WithScriptsEmbeddedInAssembly(scriptsAssembly)
            .JournalToSqlTable(journalSchema, journalTable)
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
