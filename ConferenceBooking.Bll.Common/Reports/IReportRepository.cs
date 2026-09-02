using ConferenceBooking.Bll.Common.Reports.Models;

namespace ConferenceBooking.Bll.Common.Reports;

/// <summary>
/// Інтерфейс репозиторію отримання аналітичних даних для звітів.
/// </summary>
public interface IReportRepository
{
    /// <summary>
    /// Отримати дані про виручку та кількість бронювань з групуванням по днях.
    /// </summary>
    /// <param name="from">Початок інтервалу вибірки.</param>
    /// <param name="to">Кінець інтервалу вибірки.</param>
    /// <param name="cancellationToken">Токен скасування операції.</param>
    /// <returns>Список щоденних фінансових показників.</returns>
    Task<IReadOnlyList<DailyRevenue>> GetRevenueByDayAsync(DateTime from, DateTime to, CancellationToken cancellationToken = default);

    /// <summary>
    /// Отримати статистичні показники популярності залів із бази даних.
    /// </summary>
    /// <param name="cancellationToken">Токен скасування операції.</param>
    /// <returns>Колекція сутностей популярності залів.</returns>
    Task<IEnumerable<RoomPopularity>> GetRoomPopularityAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Отримати первинні дані про завантаженість залів (загальну кількість заброньованих годин).
    /// </summary>
    /// <param name="from">Початок інтервалу вибірки.</param>
    /// <param name="to">Кінець інтервалу вибірки.</param>
    /// <param name="cancellationToken">Токен скасування операції.</param>
    /// <returns>Колекція вихідних даних навантаження на зали.</returns>
    Task<IEnumerable<RoomLoad>> GetRoomLoadRawAsync(DateTime from, DateTime to, CancellationToken cancellationToken = default);
}
