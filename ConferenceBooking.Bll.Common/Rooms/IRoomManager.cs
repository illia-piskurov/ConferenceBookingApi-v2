using ConferenceBooking.Bll.Common.Rooms.Models;

namespace ConferenceBooking.Bll.Common.Rooms;

/// <summary>
/// Інтерфейс сервісу управління конференц-залами.
/// </summary>
public interface IRoomManager
{
    /// <summary>
    /// Отримати список усіх залів.
    /// </summary>
    /// <param name="cancellationToken">Токен скасування операції.</param>
    /// <returns>Колекція залів.</returns>
    Task<IEnumerable<Room>> GetAllRoomsAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Отримати зал за унікальним ідентифікатором.
    /// </summary>
    /// <param name="id">Ідентифікатор залу.</param>
    /// <param name="includeDeleted">Ознака включення видалених залів.</param>
    /// <param name="cancellationToken">Токен скасування операції.</param>
    /// <returns>Сутність залу.</returns>
    Task<Room> GetRoomByIdAsync(Guid id, bool includeDeleted = false, CancellationToken cancellationToken = default);

    /// <summary>
    /// Створити новий конференц-зал.
    /// </summary>
    /// <param name="request">Дані для створення залу.</param>
    /// <param name="cancellationToken">Токен скасування операції.</param>
    /// <returns>Створений зал із присвоєним ідентифікатором.</returns>
    Task<Room> CreateRoomAsync(CreateRoomRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Оновити дані існуючого конференц-залу.
    /// </summary>
    /// <param name="id">Ідентифікатор залу.</param>
    /// <param name="request">Оновлені дані залу.</param>
    /// <param name="cancellationToken">Токен скасування операції.</param>
    /// <returns>Оновлена сутність залу.</returns>
    Task<Room> UpdateRoomAsync(Guid id, UpdateRoomRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Видалити конференц-зал (м'яке видалення).
    /// </summary>
    /// <param name="id">Ідентифікатор залу.</param>
    /// <param name="cancellationToken">Токен скасування операції.</param>
    Task DeleteRoomAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Пошук доступних для бронювання залів за часовим інтервалом та місткістю.
    /// </summary>
    /// <param name="start">Початок бажаного інтервалу.</param>
    /// <param name="end">Кінець бажаного інтервалу.</param>
    /// <param name="capacity">Мінімальна необхідна місткість.</param>
    /// <param name="cancellationToken">Токен скасування операції.</param>
    /// <returns>Колекція доступних залів.</returns>
    Task<IEnumerable<Room>> SearchAvailableRoomsAsync(DateTime start, DateTime end, int capacity, CancellationToken cancellationToken = default);
}
