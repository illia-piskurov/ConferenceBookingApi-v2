namespace ConferenceBooking.Dal.SqlRepositories.Constants;

public static class SqlProcedures
{
    public static class Rooms
    {
        public const string GetAll = "sp_Rooms_GetAll";
        public const string GetById = "sp_Rooms_GetById";
        public const string Insert = "sp_Rooms_Insert";
        public const string Update = "sp_Rooms_Update";
        public const string Delete = "sp_Rooms_Delete";
    }

    public static class Bookings
    {
        public const string GetAll = "sp_Bookings_GetAll";
        public const string GetById = "sp_Bookings_GetById";
        public const string GetByRoomId = "sp_Bookings_GetByRoomId";
        public const string GetOverlapping = "sp_Bookings_GetOverlapping";
        public const string GetByDateRange = "sp_Bookings_GetByDateRange";
        public const string Insert = "sp_Bookings_Insert";
    }
}
