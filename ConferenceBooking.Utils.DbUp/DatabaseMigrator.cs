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

        DatabaseUpgradeResult? result = null;
        for (var attempt = 1; attempt <= 5; attempt++)
        {
            try
            {
                result = upgrader.PerformUpgrade();
                if (result.Successful)
                {
                    return result;
                }

                if (attempt < 5)
                {
                    Console.WriteLine($"[WARN] DbUp upgrade attempt {attempt} failed ({result.Error?.Message}). Retrying in 2 seconds...");
                    Thread.Sleep(2000);
                }
            }
            catch (Exception ex) when (attempt < 5)
            {
                Console.WriteLine($"[WARN] DbUp upgrade attempt {attempt} threw exception ({ex.Message}). Retrying in 2 seconds...");
                Thread.Sleep(2000);
            }
        }

        if (result == null || !result.Successful)
        {
            throw new InvalidOperationException($"DbUp migration failed after retries: {result?.Error?.Message}", result?.Error);
        }

        return result;
    }
}
