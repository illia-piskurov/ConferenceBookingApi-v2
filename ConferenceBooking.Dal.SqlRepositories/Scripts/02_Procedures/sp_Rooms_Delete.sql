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
