IF NOT EXISTS (SELECT * FROM sys.schemas WHERE name = 'IPiskurovSchema')
BEGIN
    EXEC('CREATE SCHEMA IPiskurovSchema');
END
GO

IF OBJECT_ID('IPiskurovSchema.Rooms', 'U') IS NULL
BEGIN
    CREATE TABLE IPiskurovSchema.Rooms
    (
        Id UNIQUEIDENTIFIER NOT NULL CONSTRAINT DF_Rooms_Id DEFAULT NEWSEQUENTIALID(),
        Name NVARCHAR(150) NOT NULL,
        Capacity INT NOT NULL,
        BaseHourlyRate DECIMAL(18, 2) NOT NULL,
        IsDeleted BIT NOT NULL CONSTRAINT DF_Rooms_IsDeleted DEFAULT 0,

        CONSTRAINT PK_Rooms PRIMARY KEY CLUSTERED (Id)
    );
END
GO

IF OBJECT_ID('IPiskurovSchema.Services', 'U') IS NULL
BEGIN
    CREATE TABLE IPiskurovSchema.Services
    (
        Id UNIQUEIDENTIFIER NOT NULL CONSTRAINT DF_Services_Id DEFAULT NEWSEQUENTIALID(),
        Name NVARCHAR(150) NOT NULL,
        Price DECIMAL(18, 2) NOT NULL,

        CONSTRAINT PK_Services PRIMARY KEY CLUSTERED (Id)
    );
END
GO

IF OBJECT_ID('IPiskurovSchema.RoomServices', 'U') IS NULL
BEGIN
    CREATE TABLE IPiskurovSchema.RoomServices
    (
        RoomId UNIQUEIDENTIFIER NOT NULL,
        ServiceId UNIQUEIDENTIFIER NOT NULL,

        CONSTRAINT PK_RoomServices PRIMARY KEY CLUSTERED (RoomId, ServiceId),
        CONSTRAINT FK_RoomServices_Rooms FOREIGN KEY (RoomId) 
            REFERENCES IPiskurovSchema.Rooms (Id) ON DELETE CASCADE,
        CONSTRAINT FK_RoomServices_Services FOREIGN KEY (ServiceId) 
            REFERENCES IPiskurovSchema.Services (Id) ON DELETE CASCADE,

        INDEX IX_RoomServices_ServiceId NONCLUSTERED (ServiceId)
    );
END
GO

IF OBJECT_ID('IPiskurovSchema.Bookings', 'U') IS NULL
BEGIN
    CREATE TABLE IPiskurovSchema.Bookings
    (
        Id UNIQUEIDENTIFIER NOT NULL CONSTRAINT DF_Bookings_Id DEFAULT NEWSEQUENTIALID(),
        RoomId UNIQUEIDENTIFIER NOT NULL,
        StartTime DATETIME2(2) NOT NULL,
        EndTime DATETIME2(2) NOT NULL,
        TotalCost DECIMAL(18, 2) NOT NULL,
        CreatedAt DATETIME2(2) NOT NULL CONSTRAINT DF_Bookings_CreatedAt DEFAULT SYSUTCDATETIME(),

        CONSTRAINT PK_Bookings PRIMARY KEY CLUSTERED (Id),
        CONSTRAINT FK_Bookings_Rooms FOREIGN KEY (RoomId) 
            REFERENCES IPiskurovSchema.Rooms (Id),
        CONSTRAINT CK_Bookings_Dates CHECK (EndTime > StartTime),

        INDEX IX_Bookings_RoomId NONCLUSTERED (RoomId),
        INDEX IX_Bookings_RoomId_StartTime_EndTime NONCLUSTERED (RoomId, StartTime, EndTime)
    );
END
GO

IF OBJECT_ID('IPiskurovSchema.BookingServices', 'U') IS NULL
BEGIN
    CREATE TABLE IPiskurovSchema.BookingServices
    (
        BookingId UNIQUEIDENTIFIER NOT NULL,
        ServiceId UNIQUEIDENTIFIER NOT NULL,

        CONSTRAINT PK_BookingServices PRIMARY KEY CLUSTERED (BookingId, ServiceId),
        CONSTRAINT FK_BookingServices_Bookings FOREIGN KEY (BookingId) 
            REFERENCES IPiskurovSchema.Bookings (Id) ON DELETE CASCADE,
        CONSTRAINT FK_BookingServices_Services FOREIGN KEY (ServiceId) 
            REFERENCES IPiskurovSchema.Services (Id),

        INDEX IX_BookingServices_ServiceId NONCLUSTERED (ServiceId)
    );
END
GO

-- For Stored Procedures
IF TYPE_ID('IPiskurovSchema.GuidListType') IS NULL
BEGIN
    CREATE TYPE IPiskurovSchema.GuidListType AS TABLE
    (
        Id UNIQUEIDENTIFIER NOT NULL PRIMARY KEY
    );
END
GO