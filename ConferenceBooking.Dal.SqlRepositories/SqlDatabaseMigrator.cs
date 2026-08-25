using ConferenceBooking.Utils.DbUp;

namespace ConferenceBooking.Dal.SqlRepositories;

public static class SqlDatabaseMigrator
{
    public static void Migrate(string connectionString)
    {
        DatabaseMigrator.MigrateSqlServer(
            connectionString,
            typeof(SqlDatabaseMigrator).Assembly,
            journalSchema: "IPiskurovSchema",
            journalTable: "SchemaVersions"
        );
    }
}
