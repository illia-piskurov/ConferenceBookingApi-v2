using ConferenceBooking.Bll.Common.Bookings.Models;
using ConferenceBooking.Bll.Common.Rooms.Models;

namespace ConferenceBooking.Bll.Common.Bookings;

/// <summary>
/// Інтерфейс калькулятора вартості бронювання залів та послуг.
/// </summary>
public interface IPricingManager
{
    /// <summary>
    /// Розрахувати вартість оренди залу за тарифними зонами та додаткових послуг.
    /// </summary>
    /// <param name="room">Дані конференц-залу з базовою ставкою.</param>
    /// <param name="startTime">Дата та час початку оренди.</param>
    /// <param name="endTime">Дата та час завершення оренди.</param>
    /// <param name="selectedServices">Перелік замовлених додаткових послуг.</param>
    /// <returns>Результат та деталізація розрахунку вартості.</returns>
    PricingResult Calculate(Room room, DateTime startTime, DateTime endTime, IEnumerable<Service> selectedServices);
}
