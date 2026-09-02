using Spectre.Console;

namespace ConferenceBooking.LoadTest.Reporting;

/// <summary>
/// Презентаційний контекст для анімованого відображення прогресу виконання завдань у консолі.
/// </summary>
public static class ConsoleProgressScope
{
    /// <summary>
    /// Виконує асинхронну операцію з відображенням анімованого прогрес-бару в консолі.
    /// </summary>
    /// <typeparam name="T">Тип результату операції.</typeparam>
    /// <param name="description">Опис задачі для відображення поряд із прогрес-баром.</param>
    /// <param name="totalUnits">Загальна кількість кроків / одиниць роботи.</param>
    /// <param name="action">Дія, що виконується і приймає провайдер прогресу <see cref="IProgress{Int32}"/>.</param>
    /// <returns>Результат виконання операції.</returns>
    public static async Task<T> ExecuteAsync<T>(
        string description,
        int totalUnits,
        Func<IProgress<int>, Task<T>> action)
    {
        T result = default!;

        await AnsiConsole.Progress()
            .StartAsync(async ctx =>
            {
                var progressTask = ctx.AddTask(description, maxValue: totalUnits);
                var progress = new Progress<int>(_ => progressTask.Increment(1));

                result = await action(progress);
            });

        return result;
    }
}
