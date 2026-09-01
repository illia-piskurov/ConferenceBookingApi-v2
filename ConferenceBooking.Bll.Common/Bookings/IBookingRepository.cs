using ConferenceBooking.Bll.Common.Bookings.Models;

namespace ConferenceBooking.Bll.Common.Bookings;

/// <summary>
/// Інтерфейс репозиторію доступу до даних бронювань.
/// </summary>
public interface IBookingRepository
{
    /// <summary>
    /// Отримати всі бронювання зі сховища даних.
    /// </summary>
    /// <param name="cancellationToken">Токен скасування операції.</param>
    /// <returns>Колекція всіх бронювань.</returns>
    Task<IEnumerable<Booking>> GetAllAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Отримати бронювання за унікальним ідентифікатором.
    /// </summary>
    /// <param name="id">Ідентифікатор бронювання.</param>
    /// <param name="cancellationToken">Токен скасування операції.</param>
    /// <returns>Сутність бронювання або null, якщо не знайдено.</returns>
    Task<Booking?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Отримати всі бронювання для конкретного залу.
    /// </summary>
    /// <param name="roomId">Ідентифікатор залу.</param>
    /// <param name="cancellationToken">Токен скасування операції.</param>
    /// <returns>Колекція бронювань залу.</returns>
    Task<IEnumerable<Booking>> GetByRoomIdAsync(Guid roomId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Отримати бронювання, що перетинаються із заданим часовим інтервалом для вказаного залу.
    /// </summary>
    /// <param name="roomId">Ідентифікатор залу.</param>
    /// <param name="start">Початок часового проміжку.</param>
    /// <param name="end">Кінець часового проміжку.</param>
    /// <param name="cancellationToken">Токен скасування операції.</param>
    /// <returns>Колекція конфліктних бронювань.</returns>
    Task<IEnumerable<Booking>> GetOverlappingAsync(Guid roomId, DateTime start, DateTime end, CancellationToken cancellationToken = default);

    /// <summary>
    /// Отримати бронювання за діапазоном дат.
    /// </summary>
    /// <param name="from">Початкова дата.</param>
    /// <param name="to">Кінцева дата.</param>
    /// <param name="cancellationToken">Токен скасування операції.</param>
    /// <returns>Колекція бронювань у заданому інтервалі.</returns>
    Task<IEnumerable<Booking>> GetByDateRangeAsync(DateTime from, DateTime to, CancellationToken cancellationToken = default);

    /// <summary>
    /// Зберегти нове бронювання у сховищі даних.
    /// </summary>
    /// <param name="booking">Сутність бронювання для збереження.</param>
    /// <param name="cancellationToken">Токен скасування операції.</param>
    /// <returns>Збережене бронювання.</returns>
    Task<Booking> AddAsync(Booking booking, CancellationToken cancellationToken = default);
}
