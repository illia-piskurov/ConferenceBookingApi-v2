namespace ConferenceBooking.Services.Web.DTOs.Reports;

/// <summary>
/// Зведений звіт про доходи за обраний період.
/// </summary>
public class RevenueReportDto
{
    /// <summary>
    /// Початкова дата звітного періоду.
    /// </summary>
    public DateTime From { get; set; }

    /// <summary>
    /// Кінцева дата звітного періоду.
    /// </summary>
    public DateTime To { get; set; }

    /// <summary>
    /// Загальна кількість бронювань за весь період.
    /// </summary>
    public int TotalBookings { get; set; }

    /// <summary>
    /// Загальна сума виручки за весь період.
    /// </summary>
    public decimal TotalRevenue { get; set; }

    /// <summary>
    /// Щоденна розбивка фінансових показників.
    /// </summary>
    public List<DailyRevenueDto> ByDay { get; set; } = [];
}
