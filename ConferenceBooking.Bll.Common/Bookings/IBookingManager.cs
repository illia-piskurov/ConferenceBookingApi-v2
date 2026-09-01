using ConferenceBooking.Bll.Common.Bookings.Models;

namespace ConferenceBooking.Bll.Common.Bookings;

/// <summary>
/// Інтерфейс сервісу управління бронюваннями конференц-залів.
/// </summary>
public interface IBookingManager
{
    /// <summary>
    /// Створити нове бронювання конференц-залу з перевіркою конфліктів та розрахунком вартості.
    /// </summary>
    /// <param name="roomId">Унікальний ідентифікатор залу.</param>
    /// <param name="startTime">Дата та час початку бронювання.</param>
    /// <param name="endTime">Дата та час завершення бронювання.</param>
    /// <param name="selectedServiceIds">Список ідентифікаторів додаткових послуг.</param>
    /// <param name="cancellationToken">Токен скасування операції.</param>
    /// <returns>Деталі створеного бронювання з фінансовим розрахунком.</returns>
    Task<BookingDetails> CreateBookingAsync(
        Guid roomId,
        DateTime startTime,
        DateTime endTime,
        IEnumerable<Guid> selectedServiceIds,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Отримати детальну інформацію про бронювання за його ідентифікатором.
    /// </summary>
    /// <param name="id">Унікальний ідентифікатор бронювання.</param>
    /// <param name="cancellationToken">Токен скасування операції.</param>
    /// <returns>Деталі бронювання.</returns>
    Task<BookingDetails> GetBookingByIdAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Отримати список усіх бронювань для конкретного конференц-залу.
    /// </summary>
    /// <param name="roomId">Унікальний ідентифікатор залу.</param>
    /// <param name="cancellationToken">Токен скасування операції.</param>
    /// <returns>Колекція бронювань вказаного залу.</returns>
    Task<IEnumerable<BookingDetails>> GetBookingsByRoomAsync(Guid roomId, CancellationToken cancellationToken = default);
}
