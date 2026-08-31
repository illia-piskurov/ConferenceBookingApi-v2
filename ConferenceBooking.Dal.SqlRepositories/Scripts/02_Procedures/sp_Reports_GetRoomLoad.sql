CREATE OR ALTER PROCEDURE IPiskurovSchema.sp_Reports_GetRoomLoad
    @From DATETIME2(2),
    @To DATETIME2(2)
AS
BEGIN
    SET NOCOUNT ON;

    SELECT 
        r.Id AS [RoomId],
        r.Name AS [RoomName],
        ISNULL(SUM(
            DATEDIFF(MINUTE, 
                CASE WHEN b.StartTime < @From THEN @From ELSE b.StartTime END,
                CASE WHEN b.EndTime > @To THEN @To ELSE b.EndTime END
            ) / 60.0
        ), 0.0) AS [BookedHours]
    FROM IPiskurovSchema.Rooms r
    LEFT JOIN IPiskurovSchema.Bookings b 
        ON r.Id = b.RoomId 
        AND b.StartTime < @To 
        AND b.EndTime > @From
    WHERE r.IsDeleted = 0
    GROUP BY r.Id, r.Name;
END;
GO
