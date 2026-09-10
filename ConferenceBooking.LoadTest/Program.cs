using ConferenceBooking.LoadTest.Reporting;
using ConferenceBooking.LoadTest.Scenarios;
using Spectre.Console;

namespace ConferenceBooking.LoadTest;

internal class Program
{
    private const string DefaultBaseUrl = "http://localhost:5280";

    private static async Task Main()
    {
        Console.OutputEncoding = System.Text.Encoding.UTF8;
        ConsoleUi.ShowBanner();

        var apiUrl = ConsoleUi.AskApiUrl(DefaultBaseUrl);
        var orchestrator = new BenchmarkOrchestrator();

        while (true)
        {
            var choice = ConsoleUi.SelectMainMenuAction();

            if (choice.StartsWith("1"))
            {
                await orchestrator.RunComparativeBenchmarkAsync(apiUrl);
            }
            else if (choice.StartsWith("2"))
            {
                var (total, conc) = ConsoleUi.AskCustomTestParameters();
                await orchestrator.RunSingleTestAsync(apiUrl, total, conc);
            }
            else
            {
                AnsiConsole.MarkupLine("[grey]Роботу завершено.[/]");
                return;
            }
        }
    }
}
