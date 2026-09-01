CREATE OR ALTER PROCEDURE IPiskurovSchema.sp_Reports_GetRevenue
    @From DATETIME2(2),
    @To DATETIME2(2)
AS
BEGIN
    SET NOCOUNT ON;

    SELECT 
        CAST(b.StartTime AS DATE) AS [Date],
        COUNT(1) AS [Bookings],
        ISNULL(SUM(b.TotalCost), 0.0) AS [Revenue]
    FROM IPiskurovSchema.Bookings b
    WHERE b.StartTime >= @From AND b.StartTime <= @To
    GROUP BY CAST(b.StartTime AS DATE)
    ORDER BY [Date] ASC;
END;
GO
