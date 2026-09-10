using System.ComponentModel.DataAnnotations;

namespace ConferenceBooking.Services.Web.DTOs.Bookings;

/// <summary>
/// Модель для створення нового бронювання конференц-залу.
/// </summary>
public class CreateBookingDto
{
    /// <summary>
    /// Унікальний ідентифікатор залу, що бронюється.
    /// </summary>
    [Required(ErrorMessage = "ID залу є обов'язковим.")]
    public Guid RoomId { get; set; }

    /// <summary>
    /// Дата та час початку періоду бронювання.
    /// </summary>
    [Required(ErrorMessage = "Час початку є обов'язковим.")]
    public DateTime StartTime { get; set; }

    /// <summary>
    /// Дата та час завершення періоду бронювання.
    /// </summary>
    [Required(ErrorMessage = "Час закінчення є обов'язковим.")]
    public DateTime EndTime { get; set; }

    /// <summary>
    /// Список унікальних ідентифікаторів замовлених додаткових послуг.
    /// </summary>
    public List<Guid> SelectedServiceIds { get; set; } = [];
}
