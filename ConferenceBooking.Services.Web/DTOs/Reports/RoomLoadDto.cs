namespace ConferenceBooking.Services.Web.DTOs.Reports;

/// <summary>
/// Звіт про відсоток завантаженості конкретного конференц-залу за період.
/// </summary>
public class RoomLoadDto
{
    /// <summary>
    /// Унікальний ідентифікатор конференц-залу.
    /// </summary>
    public Guid RoomId { get; set; }

    /// <summary>
    /// Назва конференц-залу.
    /// </summary>
    public string RoomName { get; set; } = string.Empty;

    /// <summary>
    /// Фактична кількість заброньованих годин у вказаному періоді.
    /// </summary>
    public double BookedHours { get; set; }

    /// <summary>
    /// Загальна кількість доступних робочих годин у цьому періоді.
    /// </summary>
    public double TotalAvailableHours { get; set; }

    /// <summary>
    /// Відсоток завантаженості залу (від 0.0% до 100.0%).
    /// </summary>
    public double LoadPercentage { get; set; }
}
