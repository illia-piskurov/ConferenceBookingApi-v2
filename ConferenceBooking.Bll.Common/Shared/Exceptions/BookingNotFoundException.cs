namespace ConferenceBooking.Bll.Common.Shared.Exceptions;

public class BookingNotFoundException : Exception
{
    public BookingNotFoundException(Guid id)
        : base($"Бронювання із ID '{id}' не знайдено.")
    {
    }
}
