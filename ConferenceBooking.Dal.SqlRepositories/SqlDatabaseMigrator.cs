using ConferenceBooking.Utils.DbUp;

namespace ConferenceBooking.Dal.SqlRepositories;

public static class SqlDatabaseMigrator
{
    public static void Migrate(string connectionString, string schema = "dbo")
    {
        DatabaseMigrator.MigrateSqlServer(
            connectionString,
            typeof(SqlDatabaseMigrator).Assembly,
            journalSchema: schema,
            journalTable: "SchemaVersions"
        );
    }
}
