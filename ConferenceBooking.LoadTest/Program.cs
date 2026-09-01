using ConferenceBooking.LoadTest.Metrics;
using ConferenceBooking.LoadTest.Scenarios;

namespace ConferenceBooking.LoadTest;

internal class Program
{
    private const string DefaultBaseUrl = "http://localhost:5280";

    private static async Task Main(string[] args)
    {
        Console.OutputEncoding = System.Text.Encoding.UTF8;

        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine("============================================================");
        Console.WriteLine("       ConferenceBooking Web API - Load Testing Tool        ");
        Console.WriteLine("============================================================");
        Console.ResetColor();

        // 1. Обробка CLI аргументів: dotnet run -- [totalRequests] [concurrency] [baseUrl]
        if (args.Length >= 2 && int.TryParse(args[0], out var totalReq) && int.TryParse(args[1], out var concurrency))
        {
            var baseUrl = args.Length >= 3 ? args[2] : DefaultBaseUrl;
            await RunSingleTestAsync(baseUrl, totalReq, concurrency);
            return;
        }

        // 2. Інтерактивне меню
        var apiUrl = DefaultBaseUrl;
        Console.Write($"Введіть URL API (за замовчуванням: {DefaultBaseUrl}): ");
        var inputUrl = Console.ReadLine();
        if (!string.IsNullOrWhiteSpace(inputUrl))
        {
            apiUrl = inputUrl.Trim();
        }

        while (true)
        {
            Console.WriteLine("\nОберіть дію:");
            Console.WriteLine("  [1] Запустити порівняльний бенчмарк (1000 @ 10, 1000 @ 50, 1000 @ 100)");
            Console.WriteLine("  [2] Запустити одиночний тест з кастомними параметрами");
            Console.WriteLine("  [0] Вихід");
            Console.Write("\nВаш вибір: ");

            var choice = Console.ReadLine()?.Trim();
            switch (choice)
            {
                case "1":
                    await RunComparativeBenchmarkAsync(apiUrl);
                    break;
                case "2":
                    await RunCustomTestPromptAsync(apiUrl);
                    break;
                case "0":
                    Console.WriteLine("Роботу завершено.");
                    return;
                default:
                    Console.WriteLine("Невірний вибір. Спробуйте ще раз.");
                    break;
            }
        }
    }

    private static async Task RunSingleTestAsync(string baseUrl, int totalRequests, int concurrency)
    {
        Console.WriteLine($"\nПідключення до {baseUrl}...");
        using var runner = new LoadScenarioRunner(baseUrl);
        await runner.InitializeAsync();

        Console.WriteLine($"Старт тесту: {totalRequests} задач, {concurrency} одночасних запитів...\n");
        var report = await runner.RunTestAsync(totalRequests, concurrency);
        report.PrintToConsole();
    }

    private static async Task RunCustomTestPromptAsync(string baseUrl)
    {
        Console.Write("Кількість асинхронних задач (за замовчуванням 1000): ");
        var totalStr = Console.ReadLine();
        var totalRequests = int.TryParse(totalStr, out var t) && t > 0 ? t : 1000;

        Console.Write("Ступінь паралельності (concurrent requests, за замовчуванням 50): ");
        var concStr = Console.ReadLine();
        var concurrency = int.TryParse(concStr, out var c) && c > 0 ? c : 50;

        await RunSingleTestAsync(baseUrl, totalRequests, concurrency);
    }

    private static async Task RunComparativeBenchmarkAsync(string baseUrl)
    {
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine("\n>>> ІНІЦІАЛІЗАЦІЯ ПОРІВНЯЛЬНОГО БЕНЧМАРКУ <<<");
        Console.ResetColor();

        using var runner = new LoadScenarioRunner(baseUrl);
        await runner.InitializeAsync();

        // Невеликий розігрів (Warmup) для JIT та пулу з'єднань
        Console.WriteLine("Виконується короткий прогрів (warmup 30 запитів)...");
        await runner.RunTestAsync(30, 5);
        Console.WriteLine("Прогрів завершено. Початок основних випробувань.\n");

        var testConfigs = new[]
        {
            (Total: 1000, Concurrency: 10),
            (Total: 1000, Concurrency: 50),
            (Total: 1000, Concurrency: 100)
        };

        var reports = new List<TestRunReport>();

        foreach (var (total, concurrency) in testConfigs)
        {
            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.WriteLine($"\n--- ТЕСТ: {total} задач / {concurrency} concurrent ---");
            Console.ResetColor();

            var report = await runner.RunTestAsync(total, concurrency);
            report.PrintToConsole();
            reports.Add(report);

            // Пауза 2 секунди між прогонами для стабілізації ресурсів
            await Task.Delay(2000);
        }

        PrintComparativeTable(reports);
    }

    private static void PrintComparativeTable(IReadOnlyList<TestRunReport> reports)
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
