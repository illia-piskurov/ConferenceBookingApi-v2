namespace ConferenceBooking.Dal.SqlRepositories.Constants;

public static class SqlErrorCodes
{
    /// <summary>
    /// Код помилки при спробі забронювати зал, який вже заброньований на вказаний час (sp_Bookings_Insert).
    /// </summary>
    public const int BookingConflict = 50001;

    /// <summary>
    /// Код помилки при спробі видалити зал, на який є активні або майбутні бронювання (sp_Rooms_Delete).
    /// </summary>
    public const int RoomHasActiveBookings = 50002;
}
