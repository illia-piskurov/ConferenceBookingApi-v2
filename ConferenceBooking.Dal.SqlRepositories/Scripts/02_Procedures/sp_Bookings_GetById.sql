CREATE OR ALTER PROCEDURE IPiskurovSchema.sp_Bookings_GetById
    @Id UNIQUEIDENTIFIER
AS
BEGIN
    SET NOCOUNT ON;

    SELECT b.Id, b.RoomId, b.StartTime, b.EndTime, b.TotalCost, b.CreatedAt
    FROM IPiskurovSchema.Bookings b
    WHERE b.Id = @Id;

    SELECT bs.ServiceId
    FROM IPiskurovSchema.BookingServices bs
    WHERE bs.BookingId = @Id;
END;
GO
