namespace ConferenceBookingApi.Exceptions;

public class BookingConflictException : Exception
{
    public Guid RoomId { get; }
    public DateTime RequestedStart { get; }
    public DateTime RequestedEnd { get; }

    public BookingConflictException(Guid roomId, DateTime start, DateTime end)
        : base($"Зал '{roomId}' вже заброньований на час з {start:HH:mm} до {end:HH:mm}.")
    {
        RoomId = roomId;
        RequestedStart = start;
        RequestedEnd = end;
    }
}
