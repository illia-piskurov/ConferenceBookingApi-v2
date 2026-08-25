CREATE OR ALTER PROCEDURE IPiskurovSchema.sp_Rooms_Insert
    @Id UNIQUEIDENTIFIER OUTPUT,
    @Name NVARCHAR(150),
    @Capacity INT,
    @BaseHourlyRate DECIMAL(18, 2),
    @ServiceIds IPiskurovSchema.GuidListType READONLY
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;

    BEGIN TRANSACTION;

    IF @Id IS NULL OR @Id = '00000000-0000-0000-0000-000000000000'
        SET @Id = NEWID();

    INSERT INTO IPiskurovSchema.Rooms (Id, Name, Capacity, BaseHourlyRate, IsDeleted)
    VALUES (@Id, @Name, @Capacity, @BaseHourlyRate, 0);

    INSERT INTO IPiskurovSchema.RoomServices (RoomId, ServiceId)
    SELECT @Id, Id FROM @ServiceIds;

    COMMIT TRANSACTION;
END;
GO
