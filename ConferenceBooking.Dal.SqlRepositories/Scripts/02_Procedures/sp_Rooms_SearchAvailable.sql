CREATE OR ALTER PROCEDURE IPiskurovSchema.sp_Rooms_SearchAvailable
    @StartTime DATETIME2(2),
    @EndTime DATETIME2(2),
    @Capacity INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT r.Id, r.Name, r.Capacity, r.BaseHourlyRate, r.IsDeleted
    FROM IPiskurovSchema.Rooms r
    WHERE r.IsDeleted = 0
      AND r.Capacity >= @Capacity
      AND NOT EXISTS (
          SELECT 1 
          FROM IPiskurovSchema.Bookings b
          WHERE b.RoomId = r.Id
            AND b.StartTime < @EndTime
            AND b.EndTime > @StartTime
      )
    ORDER BY r.Capacity ASC;

    SELECT rs.RoomId, s.Id AS ServiceId, s.Name, s.Price
    FROM IPiskurovSchema.RoomServices rs
    INNER JOIN IPiskurovSchema.Services s ON s.Id = rs.ServiceId
    INNER JOIN IPiskurovSchema.Rooms r ON r.Id = rs.RoomId
    WHERE r.IsDeleted = 0
      AND r.Capacity >= @Capacity
      AND NOT EXISTS (
          SELECT 1 
          FROM IPiskurovSchema.Bookings b
          WHERE b.RoomId = r.Id
            AND b.StartTime < @EndTime
            AND b.EndTime > @StartTime
      );
END;
GO
