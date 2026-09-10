using System.ComponentModel.DataAnnotations;

namespace ConferenceBooking.Services.Web.DTOs.Rooms;

/// <summary>
/// Модель для оновлення параметрів існуючого конференц-залу.
/// </summary>
public class UpdateRoomDto
{
    /// <summary>
    /// Оновлена назва конференц-залу.
    /// </summary>
    [Required(ErrorMessage = "Назва залу є обов'язковою.")]
    [StringLength(100, MinimumLength = 2, ErrorMessage = "Назва залу має містити від 2 до 100 символів.")]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Оновлена місткість залу (кількість осіб).
    /// </summary>
    [Range(1, 1000, ErrorMessage = "Місткість залу має бути від 1 до 1000 осіб.")]
    public int Capacity { get; set; }

    /// <summary>
    /// Оновлена базова погодинна ставка оренди.
    /// </summary>
    [Range(0.01, 1000000, ErrorMessage = "Погодинна ставка має бути більшою за 0.")]
    public decimal BaseHourlyRate { get; set; }

    /// <summary>
    /// Оновлений список доступних послуг залу.
    /// </summary>
    public List<UpdateServiceDto> AvailableServices { get; set; } = [];
}
