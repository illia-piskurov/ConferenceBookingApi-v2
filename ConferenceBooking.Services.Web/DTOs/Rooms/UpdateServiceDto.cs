using System.ComponentModel.DataAnnotations;

namespace ConferenceBooking.Services.Web.DTOs.Rooms;

/// <summary>
/// Модель для додавання або оновлення послуги при редагуванні залу.
/// </summary>
public class UpdateServiceDto
{
    /// <summary>
    /// Ідентифікатор існуючої послуги (якщо null — буде створено нову послугу).
    /// </summary>
    public Guid? Id { get; set; }

    /// <summary>
    /// Назва додаткової послуги.
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
