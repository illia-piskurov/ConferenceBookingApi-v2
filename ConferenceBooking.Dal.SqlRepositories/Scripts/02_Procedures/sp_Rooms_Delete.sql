CREATE OR ALTER PROCEDURE IPiskurovSchema.sp_Rooms_Delete
    @Id UNIQUEIDENTIFIER
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE IPiskurovSchema.Rooms
    SET IsDeleted = 1
    WHERE Id = @Id;
END;
GO
