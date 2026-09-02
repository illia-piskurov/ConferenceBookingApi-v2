using ConferenceBooking.LoadTest.Models;
using Spectre.Console;

namespace ConferenceBooking.LoadTest.Reporting;

public static class ConsoleReportPrinter
{
    public static void PrintIndividualReport(TestRunReport report)
    {
        AnsiConsole.WriteLine();
        var rule = new Rule($"[bold cyan]РЕЗУЛЬТАТИ ТЕСТУ: {report.TotalRequests} задач / {report.Concurrency} concurrent[/]")
            .LeftJustified()
            .RuleStyle("cyan");
        AnsiConsole.Write(rule);

        // 1. Загальні метрики у вигляді картки
        var summaryTable = new Table()
            .Border(TableBorder.Rounded)
            .BorderColor(Color.Cyan1)
            .Title("[bold]Загальні показники[/]")
            .AddColumn(new TableColumn("[bold]Параметр[/]").LeftAligned())
            .AddColumn(new TableColumn("[bold]Значення[/]").RightAligned());

        summaryTable.AddRow("Час виконання", $"[bold yellow]{report.TotalExecutionTime.TotalSeconds:F2} c[/]");
        summaryTable.AddRow("Пропускна здатність", $"[bold green]{report.RequestsPerSecond:F2} req/sec[/]");
        summaryTable.AddRow("Успішних (2xx)", $"[green]{report.SuccessCount} ({report.SuccessRate:F1}%)[/]");

        if (report.ConflictCount > 0)
        {
            summaryTable.AddRow("Конфліктів часу (409)", $"[yellow]{report.ConflictCount} ({report.ConflictRate:F1}%)[/]");
        }

        if (report.ErrorCount > 0)
        {
            summaryTable.AddRow("Помилок/таймаутів", $"[bold red]{report.ErrorCount} ({report.ErrorRate:F1}%)[/]");
        }
        else
        {
            summaryTable.AddRow("Помилок (5xx/Timeout)", "[green]0 (0.0%)[/]");
        }

        // 2. Перцентилі часу відповіді
        var latencyTable = new Table()
            .Border(TableBorder.Rounded)
            .BorderColor(Color.Blue)
            .Title("[bold]Час відповіді (Latency)[/]")
            .AddColumn(new TableColumn("[bold]Метрика[/]").LeftAligned())
            .AddColumn(new TableColumn("[bold]Значення[/]").RightAligned());

        latencyTable.AddRow("Мінімальний", $"{report.MinResponseTimeMs:F1} мс");
        latencyTable.AddRow("Середній", $"{report.AvgResponseTimeMs:F1} мс");
        latencyTable.AddRow("P50 (медіана)", $"[bold]{report.P50ResponseTimeMs:F1} мс[/]");
        latencyTable.AddRow("P95 (95-й %)", $"{report.P95ResponseTimeMs:F1} мс");
        latencyTable.AddRow("P99 (99-й %)", $"{report.P99ResponseTimeMs:F1} мс");
        latencyTable.AddRow("Максимальний", $"{report.MaxResponseTimeMs:F1} мс");

        // Відображаємо side-by-side у сітці
        var grid = new Grid();
        grid.AddColumn();
        grid.AddColumn();
        grid.AddRow(summaryTable, latencyTable);
        AnsiConsole.Write(grid);

        // 3. Розподіл HTTP-статусів
        var statusTable = new Table()
            .Border(TableBorder.Rounded)
            .BorderColor(Color.Grey)
            .Title("[bold]Розподіл HTTP-статусів[/]")
            .AddColumn(new TableColumn("[bold]Статус[/]").LeftAligned())
            .AddColumn(new TableColumn("[bold]Кількість запитів[/]").RightAligned())
            .AddColumn(new TableColumn("[bold]Частка[/]").RightAligned());

        foreach (var (code, count) in report.StatusCodeCounts.OrderBy(kv => kv.Key))
        {
            var percent = (double)count / report.TotalRequests * 100;
            var (name, color) = code switch
            {
                0 => ("Connection Error / Timeout", "red"),
                200 => ("200 OK", "green"),
                201 => ("201 Created", "green"),
                204 => ("204 NoContent", "green"),
                400 => ("400 BadRequest", "yellow"),
                404 => ("404 NotFound", "yellow"),
                409 => ("409 Conflict (накладення)", "yellow"),
                499 => ("499 Client Closed", "grey"),
                500 => ("500 InternalServerError", "red"),
                _ => ($"{code}", "white")
            };
            statusTable.AddRow($"[{color}]{name}[/]", $"[{color}]{count}[/]", $"[{color}]{percent:F1}%[/]");
        }

        AnsiConsole.Write(statusTable);
        AnsiConsole.WriteLine();
    }

    public static void PrintComparativeTable(IReadOnlyList<TestRunReport> reports)
    {
        AnsiConsole.WriteLine();
        var rule = new Rule("[bold cyan]ПОРІВНЯЛЬНА ТАБЛИЦЯ НАВАНТАЖУВАЛЬНОГО ТЕСТУВАННЯ[/]")
            .Centered()
            .RuleStyle("cyan");
        AnsiConsole.Write(rule);
        AnsiConsole.WriteLine();

        var table = new Table()
            .Border(TableBorder.Rounded)
            .BorderColor(Color.Cyan1);

        table.AddColumn(new TableColumn("[bold]Паралельність[/]").Centered());
        table.AddColumn(new TableColumn("[bold]Час (сек)[/]").RightAligned());
        table.AddColumn(new TableColumn("[bold]RPS[/]").RightAligned());
        table.AddColumn(new TableColumn("[bold]Успіх (2xx)[/]").RightAligned());
        table.AddColumn(new TableColumn("[bold]Конфлікт (409)[/]").RightAligned());
        table.AddColumn(new TableColumn("[bold]Помилки (5xx)[/]").RightAligned());
        table.AddColumn(new TableColumn("[bold]Мін (мс)[/]").RightAligned());
        table.AddColumn(new TableColumn("[bold]Сер (мс)[/]").RightAligned());
        table.AddColumn(new TableColumn("[bold]P50 (мед)[/]").RightAligned());
        table.AddColumn(new TableColumn("[bold]P95 (мс)[/]").RightAligned());

        foreach (var r in reports)
        {
            var successMarkup = $"[green]{r.SuccessCount} ({r.SuccessRate:F0}%)[/]";
            var conflictMarkup = r.ConflictCount > 0
                ? $"[yellow]{r.ConflictCount} ({r.ConflictRate:F0}%)[/]"
                : $"[grey]{r.ConflictCount} ({r.ConflictRate:F0}%)[/]";
            var errorMarkup = r.ErrorCount > 0
                ? $"[bold red]{r.ErrorCount} ({r.ErrorRate:F0}%)[/]"
                : $"[green]{r.ErrorCount} ({r.ErrorRate:F0}%)[/]";

            table.AddRow(
                $"[bold cyan]{r.Concurrency} conn[/]",
                $"{r.TotalExecutionTime.TotalSeconds:F2} c",
                $"[bold green]{r.RequestsPerSecond:F1}[/]",
                successMarkup,
                conflictMarkup,
                errorMarkup,
                $"{r.MinResponseTimeMs:F1}",
                $"{r.AvgResponseTimeMs:F1}",
                $"{r.P50ResponseTimeMs:F1}",
                $"{r.P95ResponseTimeMs:F1}"
            );
        }

        AnsiConsole.Write(table);
        AnsiConsole.WriteLine();
    }
}
