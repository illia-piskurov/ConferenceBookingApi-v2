namespace ConferenceBooking.Bll.Common.Shared.Exceptions;

public class RoomNotFoundException : Exception
{
    public Guid RoomId { get; }

    public RoomNotFoundException(Guid roomId)
        : base($"Зал з ID '{roomId}' не знайдено.")
    {
        RoomId = roomId;
    }
}
