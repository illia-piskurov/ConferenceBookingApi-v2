using ConferenceBooking.LoadTest.Models;
using ConferenceBooking.LoadTest.Reporting;
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
        ConsoleReportPrinter.PrintIndividualReport(report);
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
            ConsoleReportPrinter.PrintIndividualReport(report);
            reports.Add(report);

            // Пауза 2 секунди між прогонами для стабілізації ресурсів
            await Task.Delay(2000);
        }

        ConsoleReportPrinter.PrintComparativeTable(reports);
    }
}
