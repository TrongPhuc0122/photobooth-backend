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
CREATE TABLE [Branches] (
    [BranchId] int NOT NULL IDENTITY,
    [BranchName] nvarchar(max) NOT NULL,
    [BranchCode] nvarchar(max) NOT NULL,
    [Creator] nvarchar(max) NOT NULL,
    [ManagerName] nvarchar(max) NOT NULL,
    [Address] nvarchar(max) NOT NULL,
    [Status] int NOT NULL,
    [CreatedAt] datetime2 NOT NULL,
    [PhoneNumber] nvarchar(max) NOT NULL,
    [IsDeleted] bit NOT NULL,
    [DeletedAt] datetime2 NULL,
    CONSTRAINT [PK_Branches] PRIMARY KEY ([BranchId])
);

CREATE TABLE [Payments] (
    [PaymentId] int NOT NULL IDENTITY,
    [Price] decimal(18,2) NOT NULL,
    [Method] int NOT NULL,
    [TransactionRef] nvarchar(max) NOT NULL,
    [CreatedAt] datetime2 NOT NULL,
    CONSTRAINT [PK_Payments] PRIMARY KEY ([PaymentId])
);

CREATE TABLE [Topics] (
    [TopicId] int NOT NULL IDENTITY,
    [TopicName] nvarchar(max) NOT NULL,
    CONSTRAINT [PK_Topics] PRIMARY KEY ([TopicId])
);

CREATE TABLE [Vouchers] (
    [VoucherId] int NOT NULL IDENTITY,
    [BranchCode] nvarchar(max) NULL,
    [VoucherCode] nvarchar(max) NOT NULL,
    [Purpose] nvarchar(max) NOT NULL,
    [DiscountPercent] real NOT NULL,
    [CreatedAt] datetime2 NOT NULL,
    [StartDate] datetime2 NOT NULL,
    [EndDate] datetime2 NULL,
    [UsageLimit] int NULL,
    [UsageCount] int NOT NULL,
    [Creator] nvarchar(max) NOT NULL,
    [IsDeleted] bit NOT NULL,
    [DeletedAt] datetime2 NULL,
    CONSTRAINT [PK_Vouchers] PRIMARY KEY ([VoucherId])
);

CREATE TABLE [Booths] (
    [BoothId] uniqueidentifier NOT NULL,
    [BoothIp] nvarchar(max) NOT NULL,
    [BoothName] nvarchar(max) NOT NULL,
    [Brand] nvarchar(max) NOT NULL,
    [BranchId] int NOT NULL,
    [Creator] nvarchar(max) NULL,
    [CreatedAt] datetime2 NOT NULL,
    [IsDeleted] bit NOT NULL,
    [DeletedAt] datetime2 NULL,
    CONSTRAINT [PK_Booths] PRIMARY KEY ([BoothId]),
    CONSTRAINT [FK_Booths_Branches_BranchId] FOREIGN KEY ([BranchId]) REFERENCES [Branches] ([BranchId]) ON DELETE CASCADE
);

CREATE TABLE [Frames] (
    [FrameId] int NOT NULL IDENTITY,
    [BranchId] int NULL,
    [Branchname] nvarchar(max) NULL,
    [FrameName] nvarchar(max) NOT NULL,
    [TopicId] int NOT NULL,
    [LayoutType] int NOT NULL,
    [SubjectImageUrl] nvarchar(max) NOT NULL,
    [BackgroundUrl] nvarchar(max) NOT NULL,
    [OverlayUrl] nvarchar(max) NOT NULL,
    [CreatedAt] datetime2 NOT NULL,
    [IsDeleted] bit NOT NULL,
    [DeletedAt] datetime2 NULL,
    CONSTRAINT [PK_Frames] PRIMARY KEY ([FrameId]),
    CONSTRAINT [FK_Frames_Branches_BranchId] FOREIGN KEY ([BranchId]) REFERENCES [Branches] ([BranchId]),
    CONSTRAINT [FK_Frames_Topics_TopicId] FOREIGN KEY ([TopicId]) REFERENCES [Topics] ([TopicId]) ON DELETE CASCADE
);

CREATE TABLE [BoothError] (
    [ErrorId] int NOT NULL IDENTITY,
    [BoothId] uniqueidentifier NOT NULL,
    [ErrorCode] nvarchar(max) NOT NULL,
    [Cause] nvarchar(max) NOT NULL,
    [Solution] nvarchar(max) NULL,
    [IsFixed] bit NOT NULL,
    [ResolvedBy] nvarchar(max) NULL,
    [CreatedAt] datetime2 NOT NULL,
    CONSTRAINT [PK_BoothError] PRIMARY KEY ([ErrorId]),
    CONSTRAINT [FK_BoothError_Booths_BoothId] FOREIGN KEY ([BoothId]) REFERENCES [Booths] ([BoothId]) ON DELETE CASCADE
);

CREATE TABLE [BoothHealth] (
    [BoothId] uniqueidentifier NOT NULL,
    [Status] int NOT NULL,
    [LastHeartbeat] datetime2 NOT NULL,
    CONSTRAINT [PK_BoothHealth] PRIMARY KEY ([BoothId]),
    CONSTRAINT [FK_BoothHealth_Booths_BoothId] FOREIGN KEY ([BoothId]) REFERENCES [Booths] ([BoothId]) ON DELETE CASCADE
);

CREATE TABLE [BoothResources] (
    [BoothId] uniqueidentifier NOT NULL,
    [PaperCount] int NOT NULL,
    [PaperMax] int NOT NULL,
    [RibbonCount] int NOT NULL,
    [RibbonMax] int NOT NULL,
    [LastUpdated] datetime2 NOT NULL,
    CONSTRAINT [PK_BoothResources] PRIMARY KEY ([BoothId]),
    CONSTRAINT [FK_BoothResources_Booths_BoothId] FOREIGN KEY ([BoothId]) REFERENCES [Booths] ([BoothId]) ON DELETE CASCADE
);

CREATE TABLE [Invoices] (
    [InvoiceId] int NOT NULL IDENTITY,
    [InvoiceCode] nvarchar(max) NOT NULL,
    [BoothId] uniqueidentifier NOT NULL,
    [VoucherId] int NULL,
    [Price] decimal(18,2) NOT NULL,
    [FinalPrice] decimal(18,2) NOT NULL,
    [FlowStatus] bit NOT NULL,
    [PaymentMethod] int NOT NULL,
    [CreatedAt] datetime2 NOT NULL,
    CONSTRAINT [PK_Invoices] PRIMARY KEY ([InvoiceId]),
    CONSTRAINT [FK_Invoices_Booths_BoothId] FOREIGN KEY ([BoothId]) REFERENCES [Booths] ([BoothId]) ON DELETE CASCADE,
    CONSTRAINT [FK_Invoices_Vouchers_VoucherId] FOREIGN KEY ([VoucherId]) REFERENCES [Vouchers] ([VoucherId])
);

CREATE TABLE [Photos] (
    [PhotoId] int NOT NULL IDENTITY,
    [BoothId] uniqueidentifier NOT NULL,
    [FrameId] int NULL,
    [ImageUrl] nvarchar(max) NOT NULL,
    [QRCode] nvarchar(max) NOT NULL,
    [PrintCount] int NOT NULL,
    [CreatedAt] datetime2 NOT NULL,
    CONSTRAINT [PK_Photos] PRIMARY KEY ([PhotoId]),
    CONSTRAINT [FK_Photos_Booths_BoothId] FOREIGN KEY ([BoothId]) REFERENCES [Booths] ([BoothId]) ON DELETE CASCADE
);

CREATE INDEX [IX_BoothError_BoothId] ON [BoothError] ([BoothId]);

CREATE INDEX [IX_Booths_BranchId] ON [Booths] ([BranchId]);

CREATE INDEX [IX_Frames_BranchId] ON [Frames] ([BranchId]);

CREATE INDEX [IX_Frames_TopicId] ON [Frames] ([TopicId]);

CREATE INDEX [IX_Invoices_BoothId] ON [Invoices] ([BoothId]);

CREATE INDEX [IX_Invoices_VoucherId] ON [Invoices] ([VoucherId]);

CREATE INDEX [IX_Photos_BoothId] ON [Photos] ([BoothId]);

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20260807100510_New', N'10.0.5');

COMMIT;
GO

