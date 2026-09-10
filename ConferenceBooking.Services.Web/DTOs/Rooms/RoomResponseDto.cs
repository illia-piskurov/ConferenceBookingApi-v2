namespace ConferenceBooking.Services.Web.DTOs.Rooms;

/// <summary>
/// Відповідь з інформацією про супутню послугу конференц-залу.
/// </summary>
public class ServiceResponseDto
{
    /// <summary>
    /// Унікальний ідентифікатор послуги.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Назва додаткової послуги.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Вартість послуги в гривнях.
    /// </summary>
    public decimal Price { get; set; }
}

/// <summary>
/// Відповідь з інформацією про конференц-зал.
/// </summary>
public class RoomResponseDto
{
    /// <summary>
    /// Унікальний ідентифікатор конференц-залу.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Назва залу.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Місткість залу (кількість осіб).
    /// </summary>
    public int Capacity { get; set; }

    /// <summary>
    /// Базова погодинна ставка оренди залу.
    /// </summary>
    public decimal BaseHourlyRate { get; set; }

    /// <summary>
    /// Перелік доступних послуг у даному залі.
    /// </summary>
    public List<ServiceResponseDto> AvailableServices { get; set; } = new();
}
