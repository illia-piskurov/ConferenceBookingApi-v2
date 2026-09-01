namespace ConferenceBooking.LoadTest.Models;

public class TestRunReport
{
    public int TotalRequests { get; init; }
    public int Concurrency { get; init; }
    public TimeSpan TotalExecutionTime { get; init; }
    public double RequestsPerSecond { get; init; }

    public int SuccessCount { get; init; }
    public int ConflictCount { get; init; }
    public int ErrorCount { get; init; }
    public int FailureCount => ConflictCount + ErrorCount;

    public double SuccessRate => TotalRequests > 0 ? (double)SuccessCount / TotalRequests * 100 : 0;
    public double ConflictRate => TotalRequests > 0 ? (double)ConflictCount / TotalRequests * 100 : 0;
    public double ErrorRate => TotalRequests > 0 ? (double)ErrorCount / TotalRequests * 100 : 0;

    public double MinResponseTimeMs { get; init; }
    public double MaxResponseTimeMs { get; init; }
    public double AvgResponseTimeMs { get; init; }
    public double P50ResponseTimeMs { get; init; }
    public double P95ResponseTimeMs { get; init; }
    public double P99ResponseTimeMs { get; init; }

    public IReadOnlyDictionary<int, int> StatusCodeCounts { get; init; } = new Dictionary<int, int>();
    public IReadOnlyDictionary<string, int> MethodCounts { get; init; } = new Dictionary<string, int>();
}
