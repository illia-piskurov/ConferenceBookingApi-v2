CREATE OR ALTER PROCEDURE IPiskurovSchema.sp_Rooms_Update
    @Id UNIQUEIDENTIFIER,
    @Name NVARCHAR(150),
    @Capacity INT,
    @BaseHourlyRate DECIMAL(18, 2),
    @ServiceIds IPiskurovSchema.GuidListType READONLY
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;

    BEGIN TRANSACTION;

    UPDATE IPiskurovSchema.Rooms
    SET Name = @Name,
        Capacity = @Capacity,
        BaseHourlyRate = @BaseHourlyRate
    WHERE Id = @Id AND IsDeleted = 0;

    DELETE FROM IPiskurovSchema.RoomServices WHERE RoomId = @Id;

    INSERT INTO IPiskurovSchema.RoomServices (RoomId, ServiceId)
    SELECT @Id, Id FROM @ServiceIds;

    COMMIT TRANSACTION;
END;
GO
