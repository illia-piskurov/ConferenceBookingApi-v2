CREATE OR ALTER PROCEDURE IPiskurovSchema.sp_Bookings_GetByDateRange
    @From DATETIME2(2),
    @To DATETIME2(2)
AS
BEGIN
    SET NOCOUNT ON;

    SELECT b.Id, b.RoomId, b.StartTime, b.EndTime, b.TotalCost, b.CreatedAt
    FROM IPiskurovSchema.Bookings b
    WHERE b.StartTime < @To AND b.EndTime > @From;

    SELECT bs.BookingId, bs.ServiceId
    FROM IPiskurovSchema.BookingServices bs
    INNER JOIN IPiskurovSchema.Bookings b ON b.Id = bs.BookingId
    WHERE b.StartTime < @To AND b.EndTime > @From;
END;
GO
