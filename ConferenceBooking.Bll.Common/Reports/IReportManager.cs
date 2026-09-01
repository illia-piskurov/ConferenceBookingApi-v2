using ConferenceBooking.Bll.Common.Reports.Models;

namespace ConferenceBooking.Bll.Common.Reports;

/// <summary>
/// Інтерфейс сервісу формування бізнес-звітів та аналітики.
/// </summary>
public interface IReportManager
{
    /// <summary>
    /// Сформувати звіт про доходи за вказаний період із поденною деталізацією.
    /// </summary>
    /// <param name="from">Початкова дата періоду звітності.</param>
    /// <param name="to">Кінцева дата періоду звітності.</param>
    /// <param name="cancellationToken">Токен скасування операції.</param>
    /// <returns>Зведений звіт про доходи.</returns>
    Task<RevenueReport> GetRevenueReportAsync(DateTime from, DateTime to, CancellationToken cancellationToken = default);

    /// <summary>
    /// Отримати рейтинг популярності залів на основі кількості бронювань та виручки.
    /// </summary>
    /// <param name="cancellationToken">Токен скасування операції.</param>
    /// <returns>Колекція показників популярності кожного залу.</returns>
    Task<IEnumerable<RoomPopularity>> GetRoomPopularityAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Отримати розрахунок відсотка завантаженості залів за вказаний період часу.
    /// </summary>
    /// <param name="from">Початок періоду аналізу.</param>
    /// <param name="to">Кінець періоду аналізу.</param>
    /// <param name="cancellationToken">Токен скасування операції.</param>
    /// <returns>Колекція показників завантаженості залів.</returns>
    Task<IEnumerable<RoomLoad>> GetRoomLoadAsync(DateTime from, DateTime to, CancellationToken cancellationToken = default);
}
