CREATE OR ALTER PROCEDURE IPiskurovSchema.sp_Bookings_GetOverlapping
    @RoomId UNIQUEIDENTIFIER,
    @StartTime DATETIME2(2),
    @EndTime DATETIME2(2)
AS
BEGIN
    SET NOCOUNT ON;

    SELECT b.Id, b.RoomId, b.StartTime, b.EndTime, b.TotalCost, b.CreatedAt
    FROM IPiskurovSchema.Bookings b
    WHERE b.RoomId = @RoomId 
      AND b.StartTime < @EndTime 
      AND b.EndTime > @StartTime;

    SELECT bs.BookingId, bs.ServiceId
    FROM IPiskurovSchema.BookingServices bs
    INNER JOIN IPiskurovSchema.Bookings b ON b.Id = bs.BookingId
    WHERE b.RoomId = @RoomId 
      AND b.StartTime < @EndTime 
      AND b.EndTime > @StartTime;
END;
GO
