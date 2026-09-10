using ConferenceBooking.LoadTest.Models;
using ConferenceBooking.LoadTest.Reporting;
using Spectre.Console;

namespace ConferenceBooking.LoadTest.Scenarios;

/// <summary>
/// Оркестратор для координації та виконання сценаріїв навантажувального тестування.
/// </summary>
public class BenchmarkOrchestrator
{
    /// <summary>
    /// Виконує одиночний тест із заданою кількістю запитів та ступенем паралельності.
    /// </summary>
    public async Task RunSingleTestAsync(string baseUrl, int totalRequests, int concurrency)
    {
        AnsiConsole.MarkupLine($"\n[grey]Підключення до[/] [cyan]{baseUrl}[/]...");
        using var runner = new LoadScenarioRunner(baseUrl);
        await runner.InitializeAsync();

        AnsiConsole.MarkupLine($"[green]Старт тесту:[/] [bold]{totalRequests}[/] задач, [bold]{concurrency}[/] одночасних запитів...\n");
        var report = await ConsoleProgressScope.ExecuteAsync(
            $"[cyan]Виконання {totalRequests} запитів ({concurrency} concurrent)[/]",
            totalRequests,
            progress => runner.RunTestAsync(totalRequests, concurrency, progress));

        ConsoleReportPrinter.PrintIndividualReport(report);
    }

    /// <summary>
    /// Виконує повний триетапний порівняльний бенчмарк (10, 50, 100 concurrent) із попереднім прогрівом.
    /// </summary>
    public async Task RunComparativeBenchmarkAsync(string baseUrl)
    {
        ConsoleUi.ShowBenchmarkHeader();

        using var runner = new LoadScenarioRunner(baseUrl);
        await runner.InitializeAsync();

        // Прогрів (Warmup) для JIT та пулу з'єднань
        AnsiConsole.MarkupLine("[grey]Виконується короткий прогрів (warmup 30 запитів)...[/]");
        await ConsoleProgressScope.ExecuteAsync(
            "[yellow]Прогрів (30 запитів)[/]",
            30,
            progress => runner.RunTestAsync(30, 5, progress));
        AnsiConsole.MarkupLine("[green]Прогрів успішно завершено. Початок основних випробувань.[/]\n");

        var testConfigs = new[]
        {
            (Total: 1000, Concurrency: 10),
            (Total: 1000, Concurrency: 50),
            (Total: 1000, Concurrency: 100)
        };

        var reports = new List<TestRunReport>();

        foreach (var (total, concurrency) in testConfigs)
        {
            AnsiConsole.MarkupLine($"\n[bold magenta]>>> ТЕСТ: {total} задач / {concurrency} concurrent <<<[/]");

            var report = await ConsoleProgressScope.ExecuteAsync(
                $"[cyan]{total} задач ({concurrency} concurrent)[/]",
                total,
                progress => runner.RunTestAsync(total, concurrency, progress));

            ConsoleReportPrinter.PrintIndividualReport(report);
            reports.Add(report);

            // Пауза 2 секунди між прогонами для стабілізації ресурсів
            await Task.Delay(2000);
        }

        ConsoleReportPrinter.PrintComparativeTable(reports);
    }
}
