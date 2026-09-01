using System.ComponentModel.DataAnnotations;

namespace ConferenceBooking.Services.Web.DTOs.Rooms;

/// <summary>
/// Модель для створення нової супутньої послуги конференц-залу.
/// </summary>
public class CreateServiceDto
{
    /// <summary>
    /// Назва додаткової послуги (наприклад: "Проєктор", "Кава-брейк").
    /// </summary>
    [Required(ErrorMessage = "Назва послуги є обов'язковою.")]
    [StringLength(100, MinimumLength = 2, ErrorMessage = "Назва послуги має містити від 2 до 100 символів.")]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Вартість послуги в гривнях.
    /// </summary>
    [Range(0, 100000, ErrorMessage = "Вартість послуги не може бути меншою за 0.")]
    public decimal Price { get; set; }
}

/// <summary>
/// Модель для створення нового конференц-залу.
/// </summary>
public class CreateRoomDto
{
    /// <summary>
    /// Назва конференц-залу.
    /// </summary>
    [Required(ErrorMessage = "Назва залу є обов'язковою.")]
    [StringLength(100, MinimumLength = 2, ErrorMessage = "Назва залу має містити від 2 до 100 символів.")]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Максимальна місткість залу (кількість осіб).
    /// </summary>
    [Range(1, 1000, ErrorMessage = "Місткість залу має бути від 1 до 1000 осіб.")]
    public int Capacity { get; set; }

    /// <summary>
    /// Базова погодинна ставка оренди залу.
    /// </summary>
    [Range(0.01, 1000000, ErrorMessage = "Погодинна ставка має бути більшою за 0.")]
    public decimal BaseHourlyRate { get; set; }

    /// <summary>
    /// Список супутніх послуг, доступних у цьому залі.
    /// </summary>
    public List<CreateServiceDto> AvailableServices { get; set; } = [];
}
