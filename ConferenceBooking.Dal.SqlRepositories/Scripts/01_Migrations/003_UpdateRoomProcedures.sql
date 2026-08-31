CREATE OR ALTER PROCEDURE IPiskurovSchema.sp_Rooms_Delete
    @Id UNIQUEIDENTIFIER
AS
BEGIN
    SET NOCOUNT ON;

    IF EXISTS (
        SELECT 1 
        FROM IPiskurovSchema.Bookings 
        WHERE RoomId = @Id AND EndTime > SYSUTCDATETIME()
    )
    BEGIN
        THROW 50002, 'Неможливо видалити зал, оскільки на нього є активні або майбутні бронювання.', 1;
    END

    UPDATE IPiskurovSchema.Rooms
    SET IsDeleted = 1
    WHERE Id = @Id;
END;
GO

CREATE OR ALTER PROCEDURE IPiskurovSchema.sp_Rooms_GetById
    @Id UNIQUEIDENTIFIER
AS
BEGIN
    SET NOCOUNT ON;

    SELECT r.Id, r.Name, r.Capacity, r.BaseHourlyRate, r.IsDeleted
    FROM IPiskurovSchema.Rooms r
    WHERE r.Id = @Id;

    SELECT s.Id, s.Name, s.Price
    FROM IPiskurovSchema.RoomServices rs
    INNER JOIN IPiskurovSchema.Services s ON s.Id = rs.ServiceId
    WHERE rs.RoomId = @Id;
END;
GO
