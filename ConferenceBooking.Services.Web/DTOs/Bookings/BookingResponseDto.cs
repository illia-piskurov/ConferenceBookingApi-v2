using ConferenceBooking.Services.Web.DTOs.Rooms;

namespace ConferenceBooking.Services.Web.DTOs.Bookings;

/// <summary>
/// Відповідь із повною інформацією про бронювання та розрахунок вартості.
/// </summary>
public class BookingResponseDto
{
    /// <summary>
    /// Унікальний ідентифікатор бронювання.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Ідентифікатор заброньованого залу.
    /// </summary>
    public Guid RoomId { get; set; }

    /// <summary>
    /// Назва заброньованого залу.
    /// </summary>
    public string RoomName { get; set; } = string.Empty;

    /// <summary>
    /// Дата та час початку оренди.
    /// </summary>
    public DateTime StartTime { get; set; }

    /// <summary>
    /// Дата та час завершення оренди.
    /// </summary>
    public DateTime EndTime { get; set; }

    /// <summary>
    /// Перелік підключених додаткових послуг.
    /// </summary>
    public List<ServiceResponseDto> SelectedServices { get; set; } = [];

    /// <summary>
    /// Вартість оренди залу з урахуванням погодинних тарифних зон.
    /// </summary>
    public decimal RoomCost { get; set; }

    /// <summary>
    /// Загальна вартість обраних додаткових послуг.
    /// </summary>
    public decimal ServicesCost { get; set; }

    /// <summary>
    /// Підсумкова вартість бронювання (зал + послуги).
    /// </summary>
    public decimal TotalCost { get; set; }

    /// <summary>
    /// Деталізація розрахунку вартості за кожним часовим та тарифним інтервалом.
    /// </summary>
    public List<PriceBreakdownItemDto> PriceBreakdown { get; set; } = [];

    /// <summary>
    /// Дата та час створення запису про бронювання.
    /// </summary>
    public DateTime CreatedAt { get; set; }
}
