using ConferenceBooking.Bll.Common.Rooms.Models;

namespace ConferenceBooking.Bll.Common.Rooms;

/// <summary>
/// Інтерфейс репозиторію доступу до даних конференц-залів.
/// </summary>
public interface IRoomRepository
{
    /// <summary>
    /// Отримати всі зали зі сховища.
    /// </summary>
    /// <param name="cancellationToken">Токен скасування операції.</param>
    /// <returns>Колекція залів.</returns>
    Task<IEnumerable<Room>> GetAllAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Отримати зал за ідентифікатором.
    /// </summary>
    /// <param name="id">Ідентифікатор залу.</param>
    /// <param name="cancellationToken">Токен скасування операції.</param>
    /// <returns>Сутність залу або null, якщо не знайдено.</returns>
    Task<Room?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Додати новий зал до сховища даних.
    /// </summary>
    /// <param name="room">Сутність залу для збереження.</param>
    /// <param name="cancellationToken">Токен скасування операції.</param>
    /// <returns>Збережений зал із призначеними ідентифікаторами.</returns>
    Task<Room> AddAsync(Room room, CancellationToken cancellationToken = default);

    /// <summary>
    /// Оновити інформацію про зал та його послуги.
    /// </summary>
    /// <param name="room">Сутність залу з оновленими даними.</param>
    /// <param name="cancellationToken">Токен скасування операції.</param>
    /// <returns>Оновлена сутність залу.</returns>
    Task<Room> UpdateAsync(Room room, CancellationToken cancellationToken = default);

    /// <summary>
    /// Видалити зал за ідентифікатором (м'яке видалення).
    /// </summary>
    /// <param name="id">Ідентифікатор залу.</param>
    /// <param name="cancellationToken">Токен скасування операції.</param>
    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Пошук доступних залів у заданому інтервалі часу з необхідною місткістю.
    /// </summary>
    /// <param name="start">Початок часового проміжку.</param>
    /// <param name="end">Кінець часового проміжку.</param>
    /// <param name="capacity">Мінімальна місткість.</param>
    /// <param name="cancellationToken">Токен скасування операції.</param>
    /// <returns>Колекція вільних залів.</returns>
    Task<IEnumerable<Room>> SearchAvailableAsync(DateTime start, DateTime end, int capacity, CancellationToken cancellationToken = default);
}
