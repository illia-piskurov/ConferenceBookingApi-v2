namespace ConferenceBooking.Services.Web.DTOs.Bookings;

/// <summary>
/// Елемент деталізації розрахунку вартості за окремим інтервалом тарифної зони.
/// </summary>
public class PriceBreakdownItemDto
{
    /// <summary>
    /// Назва тарифної зони (наприклад: "Ранок", "День", "Вечір", "Ніч").
    /// </summary>
    public string ZoneName { get; set; } = string.Empty;

    /// <summary>
    /// Початок тарифного інтервалу.
    /// </summary>
    public DateTime From { get; set; }

    /// <summary>
    /// Кінець тарифного інтервалу.
    /// </summary>
    public DateTime To { get; set; }

    /// <summary>
    /// Тривалість у годинах.
    /// </summary>
    public double Hours { get; set; }

    /// <summary>
    /// Базова погодинна ставка залу.
    /// </summary>
    public decimal Rate { get; set; }

    /// <summary>
    /// Коефіцієнт тарифної зони.
    /// </summary>
    public decimal Multiplier { get; set; }

    /// <summary>
    /// Проміжна вартість для даного інтервалу.
    /// </summary>
    public decimal Subtotal { get; set; }
}

/// <summary>
/// Результат розрахунку вартості оренди та додаткових послуг.
/// </summary>
public class PricingResultDto
{
    /// <summary>
    /// Вартість оренди конференц-залу.
    /// </summary>
    public decimal RoomCost { get; set; }

    /// <summary>
    /// Загальна вартість обраних додаткових послуг.
    /// </summary>
    public decimal ServicesCost { get; set; }

    /// <summary>
    /// Загальна сума до сплати.
    /// </summary>
    public decimal TotalCost { get; set; }

    /// <summary>
    /// Погодинна деталізація тарифів.
    /// </summary>
    public List<PriceBreakdownItemDto> Breakdown { get; set; } = [];
}
