namespace ConferenceBooking.Bll.Common.Rooms.Models;

/// <summary>
/// Запит бізнес-рівня на створення нового конференц-залу.
/// </summary>
public class CreateRoomRequest
{
    /// <summary>
    /// Назва залу.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Максимальна місткість залу (кількість осіб).
    /// </summary>
    public int Capacity { get; set; }

    /// <summary>
    /// Базова погодинна ставка оренди.
    /// </summary>
    public decimal BaseHourlyRate { get; set; }

    /// <summary>
    /// Колекція супутніх послуг, доступних у залі.
    /// </summary>
    public IReadOnlyCollection<Service> AvailableServices { get; set; } = [];
}
