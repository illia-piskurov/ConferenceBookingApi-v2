namespace ConferenceBooking.Dal.SqlRepositories.Configuration;

public class SqlDatabaseOptions
{
    public const string SectionName = "DatabaseOptions";

    public string Schema { get; set; } = "dbo";

    public string GuidListType => $"{Schema}.GuidListType";

    public string QualifyProcedure(string procedureName)
    {
        return procedureName.Contains('.') ? procedureName : $"{Schema}.{procedureName}";
    }
}
