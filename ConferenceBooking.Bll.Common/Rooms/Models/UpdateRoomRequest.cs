namespace ConferenceBooking.Bll.Common.Rooms.Models;

/// <summary>
/// Запит бізнес-рівня на оновлення існуючого конференц-залу.
/// </summary>
public class UpdateRoomRequest
{
    /// <summary>
    /// Оновлена назва залу.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Оновлена місткість залу (кількість осіб).
    /// </summary>
    public int Capacity { get; set; }

    /// <summary>
    /// Оновлена базова погодинна ставка оренди.
    /// </summary>
    public decimal BaseHourlyRate { get; set; }

    /// <summary>
    /// Оновлена колекція доступних послуг залу.
    /// </summary>
    public IReadOnlyCollection<Service> AvailableServices { get; set; } = [];
}
