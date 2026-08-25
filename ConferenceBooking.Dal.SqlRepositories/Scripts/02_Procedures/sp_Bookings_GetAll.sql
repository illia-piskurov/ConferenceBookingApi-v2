CREATE OR ALTER PROCEDURE IPiskurovSchema.sp_Bookings_GetAll
AS
BEGIN
    SET NOCOUNT ON;

    SELECT b.Id, b.RoomId, b.StartTime, b.EndTime, b.TotalCost, b.CreatedAt
    FROM IPiskurovSchema.Bookings b;

    SELECT bs.BookingId, bs.ServiceId
    FROM IPiskurovSchema.BookingServices bs;
END;
GO
