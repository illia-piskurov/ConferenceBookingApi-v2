namespace ConferenceBookingApi.Exceptions;

public class RoomNotFoundException : Exception
{
    public Guid RoomId { get; }

    public RoomNotFoundException(Guid roomId)
        : base($"Зал із ID '{roomId}' не знайдено.")
    {
        RoomId = roomId;
    }
}
