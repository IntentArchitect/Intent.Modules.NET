CREATE TABLE [policy].[Policy]
(
    [PolicyId] BIGINT NOT NULL,
    [PolicyStatusId] UNIQUEIDENTIFIER NOT NULL,
    [AccountHolderId] BIGINT NOT NULL,
    [ProductId] INT NOT NULL,
    [PolicyNumber] NVARCHAR(MAX) NOT NULL,
    [OriginalInceptionDate] DATETIME NOT NULL,
    [StartDate] DATETIME NOT NULL,
    [ReviewDate] DATETIME NULL,
    [ExpiryDate] DATETIME NULL,
    [IsDeleted] BIT NOT NULL DEFAULT 0,
    CONSTRAINT [PK_Policy] PRIMARY KEY CLUSTERED ([PolicyId] ASC),
    CONSTRAINT [FK_Policy_PolicyStatus] FOREIGN KEY ([PolicyStatusId]) REFERENCES [accountholder].[PolicyStatus] ([PolicyStatusId]),
    CONSTRAINT [FK_Policy_AccountHolder] FOREIGN KEY ([AccountHolderId]) REFERENCES [accountholder].[AccountHolder] ([AccountHolderId]),
    CONSTRAINT [FK_Policy_Product] FOREIGN KEY ([ProductId]) REFERENCES [policy].[Product] ([ProductId])
);