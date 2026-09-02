using Spectre.Console;

namespace ConferenceBooking.LoadTest.Reporting;

/// <summary>
/// Презентаційний клас для взаємодії з користувачем через меню та запити введення.
/// </summary>
public static class ConsoleUi
{
    /// <summary>
    /// Відображає стилізований банер програми.
    /// </summary>
    public static void ShowBanner()
    {
        AnsiConsole.Write(
            new Panel(new Markup("[bold cyan]Conference Booking Web API[/] [yellow]- Навантажувальне тестування[/]"))
            {
                Border = BoxBorder.Double,
                BorderStyle = Style.Parse("cyan"),
                Padding = new Padding(2, 0, 2, 0)
            });
    }

    /// <summary>
    /// Відображає розділювальну лінію початку бенчмарку.
    /// </summary>
    public static void ShowBenchmarkHeader()
    {
        AnsiConsole.WriteLine();
        var rule = new Rule("[bold yellow]ІНІЦІАЛІЗАЦІЯ ПОРІВНЯЛЬНОГО БЕНЧМАРКУ[/]")
            .Centered()
            .RuleStyle("yellow");
        AnsiConsole.Write(rule);
    }

    /// <summary>
    /// Запитує в користувача адресу Web API із значенням за замовчуванням.
    /// </summary>
    /// <param name="defaultUrl">Базовий URL за замовчуванням.</param>
    /// <returns>Введений URL.</returns>
    public static string AskApiUrl(string defaultUrl)
    {
        return AnsiConsole.Ask("[green]Введіть URL API[/]:", defaultUrl).Trim();
    }

    /// <summary>
    /// Відображає головне меню дій для вибору за допомогою стрілок клавіатури.
    /// </summary>
    /// <returns>Рядок обраного пункту меню.</returns>
    public static string SelectMainMenuAction()
    {
        return AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title("[yellow]Оберіть режим роботи:[/]")
                .PageSize(4)
                .AddChoices([
                    "1. Запустити порівняльний бенчмарк (1000 @ 10, 50, 100)",
                    "2. Запустити одиночний тест з кастомними параметрами",
                    "0. Вихід"
                ]));
    }

    /// <summary>
    /// Запитує параметри для кастомного одиночного тесту.
    /// </summary>
    /// <returns>Кортеж із кількістю задач та ступенем паралельності.</returns>
    public static (int totalRequests, int concurrency) AskCustomTestParameters()
    {
        var totalRequests = AnsiConsole.Ask("[green]Кількість асинхронних задач[/]:", 1000);
        var concurrency = AnsiConsole.Ask("[green]Ступінь паралельності (concurrent requests)[/]:", 50);
        return (totalRequests, concurrency);
    }
}
