namespace ConferenceBooking.Services.Web.DTOs.Reports;

/// <summary>
/// Фінансові показники виручки за конкретний календарний день.
/// </summary>
public class DailyRevenueDto
{
    /// <summary>
    /// Дата (день).
    /// </summary>
    public DateTime Date { get; set; }

    /// <summary>
    /// Кількість оформлених бронювань у цей день.
    /// </summary>
    public int Bookings { get; set; }

    /// <summary>
    /// Загальна виручка в гривнях за цей день.
    /// </summary>
    public decimal Revenue { get; set; }
}
