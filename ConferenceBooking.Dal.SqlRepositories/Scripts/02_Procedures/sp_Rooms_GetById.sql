CREATE OR ALTER PROCEDURE IPiskurovSchema.sp_Rooms_GetById
    @Id UNIQUEIDENTIFIER
AS
BEGIN
    SET NOCOUNT ON;

    SELECT r.Id, r.Name, r.Capacity, r.BaseHourlyRate, r.IsDeleted
    FROM IPiskurovSchema.Rooms r
    WHERE r.Id = @Id AND r.IsDeleted = 0;

    SELECT s.Id, s.Name, s.Price
    FROM IPiskurovSchema.RoomServices rs
    INNER JOIN IPiskurovSchema.Services s ON s.Id = rs.ServiceId
    WHERE rs.RoomId = @Id;
END;
GO
