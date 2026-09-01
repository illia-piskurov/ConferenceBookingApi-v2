namespace ConferenceBooking.Services.Web.DTOs.Reports;

/// <summary>
/// Статистика популярності конференц-залу в рейтингу.
/// </summary>
public class RoomPopularityDto
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
    /// Загальна кількість бронювань цього залу за весь час.
    /// </summary>
    public int TotalBookings { get; set; }

    /// <summary>
    /// Загальна виручка, принесена цим залом (у гривнях).
    /// </summary>
    public decimal TotalRevenue { get; set; }

    /// <summary>
    /// Середня тривалість одного бронювання (у годинах).
    /// </summary>
    public double AverageBookingDurationHours { get; set; }
}
