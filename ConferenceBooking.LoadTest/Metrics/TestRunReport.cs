namespace ConferenceBooking.LoadTest.Metrics;

public class TestRunReport
{
    public int TotalRequests { get; init; }
    public int Concurrency { get; init; }
    public TimeSpan TotalExecutionTime { get; init; }
    public double RequestsPerSecond { get; init; }

    public int SuccessCount { get; init; }
    public int FailureCount { get; init; }
    public double SuccessRate => TotalRequests > 0 ? (double)SuccessCount / TotalRequests * 100 : 0;

    public double MinResponseTimeMs { get; init; }
    public double MaxResponseTimeMs { get; init; }
    public double AvgResponseTimeMs { get; init; }
    public double P50ResponseTimeMs { get; init; }
    public double P95ResponseTimeMs { get; init; }
    public double P99ResponseTimeMs { get; init; }

    public IReadOnlyDictionary<int, int> StatusCodeCounts { get; init; } = new Dictionary<int, int>();
    public IReadOnlyDictionary<string, int> MethodCounts { get; init; } = new Dictionary<string, int>();

    public void PrintToConsole()
    {
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine($"\n=======================================================");
        Console.WriteLine($"   РЕЗУЛЬТАТИ ТЕСТУ: {TotalRequests} задач / {Concurrency} concurrent");
        Console.WriteLine($"=======================================================");
        Console.ResetColor();

        Console.WriteLine($"Загальний час виконання:    {TotalExecutionTime.TotalSeconds:F2} c");
        Console.WriteLine($"Пропускна здатність (RPS):  {RequestsPerSecond:F2} req/sec");
        
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine($"Успішних запитів (2xx):     {SuccessCount} ({SuccessRate:F1}%)");
        Console.ResetColor();

        if (FailureCount > 0)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($"Запитів з помилками/кодами: {FailureCount} ({100 - SuccessRate:F1}%)");
            Console.ResetColor();
        }
        else
        {
            Console.WriteLine($"Запитів з помилками:        0 (0.0%)");
        }

        Console.WriteLine("\nЧас відповіді (мс):");
        Console.WriteLine($"  Мін:      {MinResponseTimeMs:F1} мс");
        Console.WriteLine($"  Середній: {AvgResponseTimeMs:F1} мс");
        Console.WriteLine($"  P50 (мед):{P50ResponseTimeMs:F1} мс");
        Console.WriteLine($"  P95:      {P95ResponseTimeMs:F1} мс");
        Console.WriteLine($"  P99:      {P99ResponseTimeMs:F1} мс");
        Console.WriteLine($"  Макс:     {MaxResponseTimeMs:F1} мс");

        Console.WriteLine("\nРозподіл HTTP-статусів:");
        foreach (var (code, count) in StatusCodeCounts.OrderBy(kv => kv.Key))
        {
            var statusName = code switch
            {
                0 => "Connection Error / Timeout",
                200 => "200 OK",
                201 => "201 Created",
                204 => "204 NoContent",
                400 => "400 BadRequest",
                404 => "404 NotFound",
                409 => "409 Conflict (накладення бронювань)",
                499 => "499 Client Closed",
                500 => "500 InternalServerError",
                _ => $"{code}"
            };
            Console.WriteLine($"  {statusName,-35}: {count,5} запитів");
        }

        Console.WriteLine("\nРозподіл методів:");
        foreach (var (method, count) in MethodCounts.OrderBy(kv => kv.Key))
        {
            Console.WriteLine($"  {method,-10}: {count,5} запитів");
        }
        Console.WriteLine("-------------------------------------------------------\n");
    }
}
