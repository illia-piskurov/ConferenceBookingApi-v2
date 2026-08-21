CREATE OR ALTER PROCEDURE IPiskurovSchema.sp_Rooms_GetAll
AS
BEGIN
    SET NOCOUNT ON;

    SELECT r.Id, r.Name, r.Capacity, r.BaseHourlyRate, r.IsDeleted
    FROM IPiskurovSchema.Rooms r
    WHERE r.IsDeleted = 0;

    SELECT rs.RoomId, s.Id AS ServiceId, s.Name, s.Price
    FROM IPiskurovSchema.RoomServices rs
    INNER JOIN IPiskurovSchema.Services s ON s.Id = rs.ServiceId
    INNER JOIN IPiskurovSchema.Rooms r ON r.Id = rs.RoomId
    WHERE r.IsDeleted = 0;
END;
GO
