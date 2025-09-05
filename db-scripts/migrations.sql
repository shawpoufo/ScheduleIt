IF OBJECT_ID(N'[__EFMigrationsHistory]') IS NULL
BEGIN
    CREATE TABLE [__EFMigrationsHistory] (
        [MigrationId] nvarchar(150) NOT NULL,
        [ProductVersion] nvarchar(32) NOT NULL,
        CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY ([MigrationId])
    );
END;
GO

BEGIN TRANSACTION;

IF OBJECT_ID(N'[Appointments]') IS NULL
BEGIN
    CREATE TABLE [Appointments] (
        [Id] uniqueidentifier NOT NULL,
        [CustomerId] uniqueidentifier NOT NULL,
        [StartUtc] datetime2 NOT NULL,
        [EndUtc] datetime2 NOT NULL,
        [Status] nvarchar(max) NOT NULL,
        CONSTRAINT [PK_Appointments] PRIMARY KEY ([Id])
    );
END;

IF OBJECT_ID(N'[Customers]') IS NULL
BEGIN
    CREATE TABLE [Customers] (
        [Id] uniqueidentifier NOT NULL,
        [Name] nvarchar(100) NOT NULL,
        [Email] nvarchar(200) NOT NULL,
        CONSTRAINT [PK_Customers] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Appointments_StartUtc_EndUtc' AND object_id = OBJECT_ID('Appointments'))
BEGIN
    CREATE INDEX [IX_Appointments_StartUtc_EndUtc] ON [Appointments] ([StartUtc], [EndUtc]);
END;

IF NOT EXISTS (SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20250904010338_Init')
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20250904010338_Init', N'9.0.8');
END;

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Appointments') AND name = 'Notes')
BEGIN
    ALTER TABLE [Appointments] ADD [Notes] nvarchar(1000) NULL;
END;

IF NOT EXISTS (SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20250905013814_appointment-notes')
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20250905013814_appointment-notes', N'9.0.8');
END;

COMMIT;
GO

