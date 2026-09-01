CREATE OR ALTER PROCEDURE IPiskurovSchema.sp_Reports_GetRoomPopularity
AS
BEGIN
    SET NOCOUNT ON;

    SELECT 
        r.Id AS [RoomId],
        r.Name AS [RoomName],
        COUNT(b.Id) AS [TotalBookings],
        ISNULL(SUM(b.TotalCost), 0.0) AS [TotalRevenue],
        ISNULL(AVG(DATEDIFF(MINUTE, b.StartTime, b.EndTime) / 60.0), 0.0) AS [AverageBookingDurationHours]
    FROM IPiskurovSchema.Rooms r
    LEFT JOIN IPiskurovSchema.Bookings b ON r.Id = b.RoomId
    WHERE r.IsDeleted = 0
    GROUP BY r.Id, r.Name
    ORDER BY [TotalBookings] DESC;
END;
GO
