CREATE TABLE [policy].[Policy]
(
    [PolicyId] BIGINT NOT NULL,
    [PolicyStatusId] UNIQUEIDENTIFIER NOT NULL,
    [StakeholderId] BIGINT NOT NULL,
    [ProductId] INT NOT NULL,
    [PolicyNumber] NVARCHAR(MAX) NOT NULL,
    [OriginalInceptionDate] DATETIME NOT NULL,
    [StartDate] DATETIME NOT NULL,
    [ReviewDate] DATETIME NULL,
    [ExpiryDate] DATETIME NULL,
    [ExternalSystemReference] NVARCHAR(MAX) NULL,
    [IsDeleted] BIT NOT NULL DEFAULT 0,
    CONSTRAINT [PK_Policy] PRIMARY KEY CLUSTERED ([PolicyId] ASC),
    CONSTRAINT [FK_Policy_PolicyStatus] FOREIGN KEY ([PolicyStatusId]) REFERENCES [stakeholder].[PolicyStatus] ([PolicyStatusId]),
    CONSTRAINT [FK_Policy_Stakeholder] FOREIGN KEY ([StakeholderId]) REFERENCES [stakeholder].[Stakeholder] ([StakeholderId]),
    CONSTRAINT [FK_Policy_Product] FOREIGN KEY ([ProductId]) REFERENCES [policy].[Product] ([ProductId])
);