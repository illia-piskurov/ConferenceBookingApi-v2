CREATE OR ALTER PROCEDURE IPiskurovSchema.sp_Bookings_Insert
    @Id UNIQUEIDENTIFIER OUTPUT,
    @RoomId UNIQUEIDENTIFIER,
    @StartTime DATETIME2(2),
    @EndTime DATETIME2(2),
    @TotalCost DECIMAL(18, 2),
    @CreatedAt DATETIME2(2) OUTPUT,
    @ServiceIds IPiskurovSchema.GuidListType READONLY
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;

    BEGIN TRANSACTION;

    IF EXISTS (
        SELECT 1 
        FROM IPiskurovSchema.Bookings WITH (UPDLOCK, HOLDLOCK)
        WHERE RoomId = @RoomId 
          AND StartTime < @EndTime 
          AND EndTime > @StartTime
    )
    BEGIN
        THROW 50001, 'Зал уже забронирован на указанное время.', 1;
    END

    IF @Id IS NULL OR @Id = '00000000-0000-0000-0000-000000000000'
        SET @Id = NEWID();

    SET @CreatedAt = SYSUTCDATETIME();

    INSERT INTO IPiskurovSchema.Bookings (Id, RoomId, StartTime, EndTime, TotalCost, CreatedAt)
    VALUES (@Id, @RoomId, @StartTime, @EndTime, @TotalCost, @CreatedAt);

    INSERT INTO IPiskurovSchema.BookingServices (BookingId, ServiceId)
    SELECT @Id, Id FROM @ServiceIds;

    COMMIT TRANSACTION;
END;
GO
