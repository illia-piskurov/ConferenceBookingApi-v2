using System.Collections.Concurrent;
using ConferenceBooking.LoadTest.Models;

namespace ConferenceBooking.LoadTest.Metrics;

public class MetricsCollector
{
    private readonly ConcurrentBag<RequestResult> _results = new();

    public void Record(RequestResult result)
    {
        _results.Add(result);
    }

    public TestRunReport BuildReport(int concurrency, TimeSpan totalExecutionTime)
    {
        var list = _results.ToList();
        var totalRequests = list.Count;

        if (totalRequests == 0)
        {
            return new TestRunReport
            {
                TotalRequests = 0,
                Concurrency = concurrency,
                TotalExecutionTime = totalExecutionTime,
                RequestsPerSecond = 0
            };
        }

        var durations = list.Select(r => r.DurationMs).OrderBy(d => d).ToArray();

        var successCount = list.Count(r => r.IsSuccess);
        var conflictCount = list.Count(r => r.StatusCode == 409);
        var errorCount = totalRequests - successCount - conflictCount;

        var min = durations[0];
        var max = durations[^1];
        var avg = durations.Average();

        var p50 = GetPercentile(durations, 0.50);
        var p95 = GetPercentile(durations, 0.95);
        var p99 = GetPercentile(durations, 0.99);

        var rps = totalExecutionTime.TotalSeconds > 0
            ? totalRequests / totalExecutionTime.TotalSeconds
            : 0;

        var statusCodes = list
            .GroupBy(r => r.StatusCode)
            .ToDictionary(g => g.Key, g => g.Count());

        var methods = list
            .GroupBy(r => r.Method)
            .ToDictionary(g => g.Key, g => g.Count());

        return new TestRunReport
        {
            TotalRequests = totalRequests,
            Concurrency = concurrency,
            TotalExecutionTime = totalExecutionTime,
            RequestsPerSecond = rps,
            SuccessCount = successCount,
            ConflictCount = conflictCount,
            ErrorCount = errorCount,
            MinResponseTimeMs = min,
            MaxResponseTimeMs = max,
            AvgResponseTimeMs = avg,
            P50ResponseTimeMs = p50,
            P95ResponseTimeMs = p95,
            P99ResponseTimeMs = p99,
            StatusCodeCounts = statusCodes,
            MethodCounts = methods
        };
    }

    private static double GetPercentile(double[] sortedArray, double percentile)
    {
        if (sortedArray.Length == 0) return 0;
        var index = (int)Math.Ceiling(percentile * sortedArray.Length) - 1;
        index = Math.Clamp(index, 0, sortedArray.Length - 1);
        return sortedArray[index];
    }
}
