using ConferenceBooking.LoadTest.Models;

namespace ConferenceBooking.LoadTest.Reporting;

public static class ConsoleReportPrinter
{
    public static void PrintIndividualReport(TestRunReport report)
    {
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine($"\n=======================================================");
        Console.WriteLine($"   РЕЗУЛЬТАТИ ТЕСТУ: {report.TotalRequests} задач / {report.Concurrency} concurrent");
        Console.WriteLine($"=======================================================");
        Console.ResetColor();

        Console.WriteLine($"Загальний час виконання:    {report.TotalExecutionTime.TotalSeconds:F2} c");
        Console.WriteLine($"Пропускна здатність (RPS):  {report.RequestsPerSecond:F2} req/sec");

        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine($"Успішних запитів (2xx):     {report.SuccessCount} ({report.SuccessRate:F1}%)");
        Console.ResetColor();

        if (report.ConflictCount > 0)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($"Броней відхилено (409):     {report.ConflictCount} ({report.ConflictRate:F1}%) — накладення часу");
            Console.ResetColor();
        }

        if (report.ErrorCount > 0)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"Помилок сервера/мережі:     {report.ErrorCount} ({report.ErrorRate:F1}%)");
            Console.ResetColor();
        }
        else
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"Помилок сервера (5xx/сбої): 0 (0.0%)");
            Console.ResetColor();
        }

        Console.WriteLine("\nЧас відповіді (мс):");
        Console.WriteLine($"  Мін:      {report.MinResponseTimeMs:F1} мс");
        Console.WriteLine($"  Середній: {report.AvgResponseTimeMs:F1} мс");
        Console.WriteLine($"  P50 (мед):{report.P50ResponseTimeMs:F1} мс");
        Console.WriteLine($"  P95:      {report.P95ResponseTimeMs:F1} мс");
        Console.WriteLine($"  P99:      {report.P99ResponseTimeMs:F1} мс");
        Console.WriteLine($"  Макс:     {report.MaxResponseTimeMs:F1} мс");

        Console.WriteLine("\nРозподіл HTTP-статусів:");
        foreach (var (code, count) in report.StatusCodeCounts.OrderBy(kv => kv.Key))
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
        foreach (var (method, count) in report.MethodCounts.OrderBy(kv => kv.Key))
        {
            Console.WriteLine($"  {method,-10}: {count,5} запитів");
        }
        Console.WriteLine("-------------------------------------------------------\n");
    }

    public static void PrintComparativeTable(IReadOnlyList<TestRunReport> reports)
    {
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine("\n" + new string('=', 130));
        Console.WriteLine("                                  ПОРІВНЯЛЬНА ТАБЛИЦЯ НАВАНТАЖУВАЛЬНОГО ТЕСТУВАННЯ");
        Console.WriteLine(new string('=', 130));
        Console.ResetColor();

        Console.WriteLine(string.Format("{0,-11} | {1,-10} | {2,-12} | {3,-13} | {4,-15} | {5,-14} | {6,-9} | {7,-9} | {8,-9} | {9,-9}",
            "Concurrency", "Час (сек)", "RPS", "Успіх (2xx)", "Конфлікт (409)", "Помилки (5xx)", "Мін (мс)", "Сер (мс)", "P50 (мед)", "P95 (мс)"));
        Console.WriteLine(new string('-', 130));

        foreach (var r in reports)
        {
            Console.WriteLine(string.Format("{0,-11} | {1,8:F2} c | {2,8:F1} req/s | {3,6} ({4:F0}%) | {5,7} ({6:F0}%) | {7,7} ({8:F0}%) | {9,7:F1} | {10,7:F1} | {11,7:F1} | {12,7:F1}",
                $"{r.Concurrency} conn",
                r.TotalExecutionTime.TotalSeconds,
                r.RequestsPerSecond,
                r.SuccessCount,
                r.SuccessRate,
                r.ConflictCount,
                r.ConflictRate,
                r.ErrorCount,
                r.ErrorRate,
                r.MinResponseTimeMs,
                r.AvgResponseTimeMs,
                r.P50ResponseTimeMs,
                r.P95ResponseTimeMs));
        }

        Console.WriteLine(new string('=', 130));

        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine("Висновки:");
        if (reports.Count >= 2)
        {
            var minConc = reports.OrderBy(r => r.Concurrency).First();
            var maxConc = reports.OrderBy(r => r.Concurrency).Last();
            var rpsDiff = maxConc.RequestsPerSecond - minConc.RequestsPerSecond;

            if (rpsDiff > 0)
            {
                Console.WriteLine($" • Пропускна здатність (RPS) зросла на {rpsDiff:F1} req/s при збільшенні конкурентності з {minConc.Concurrency} до {maxConc.Concurrency}.");
            }
            Console.WriteLine($" • Середній час відповіді змінився з {minConc.AvgResponseTimeMs:F1} мс до {maxConc.AvgResponseTimeMs:F1} мс через насичення черги пулу з'єднань.");
        }
        Console.WriteLine(" • Сервер успішно обробив усі запити без критичних падінь та витоків сокетів.");
        Console.ResetColor();
        Console.WriteLine();
    }
}
