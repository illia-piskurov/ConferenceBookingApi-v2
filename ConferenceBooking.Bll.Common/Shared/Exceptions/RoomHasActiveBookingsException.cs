namespace ConferenceBooking.Bll.Common.Shared.Exceptions;

public class RoomHasActiveBookingsException : Exception
{
    public RoomHasActiveBookingsException(Guid roomId)
        : base("Неможливо видалити зал, оскільки на нього є активні або майбутні бронювання.")
    {
    }
}
