CREATE OR ALTER PROCEDURE IPiskurovSchema.sp_Bookings_GetByRoomId
    @RoomId UNIQUEIDENTIFIER
AS
BEGIN
    SET NOCOUNT ON;

    SELECT b.Id, b.RoomId, b.StartTime, b.EndTime, b.TotalCost, b.CreatedAt
    FROM IPiskurovSchema.Bookings b
    WHERE b.RoomId = @RoomId;

    SELECT bs.BookingId, bs.ServiceId
    FROM IPiskurovSchema.BookingServices bs
    INNER JOIN IPiskurovSchema.Bookings b ON b.Id = bs.BookingId
    WHERE b.RoomId = @RoomId;
END;
GO
