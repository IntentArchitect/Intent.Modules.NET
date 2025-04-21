CREATE TABLE [finance].[Account]
(
    [AccountId] BIGINT IDENTITY (1,1) NOT NULL,
    [Description] NVARCHAR(255) NOT NULL,
    [AccountNumber] NVARCHAR(64) NOT NULL,
    [ExternalReference] NVARCHAR(255) NOT NULL,
    [AccountTypeId] INT NOT NULL,
    [StakeholderId] BIGINT NOT NULL,
    CONSTRAINT [PK_Account] PRIMARY KEY CLUSTERED ([AccountId] ASC),
    CONSTRAINT [FK_Account_AccountType] FOREIGN KEY ([AccountTypeId]) REFERENCES [finance].[AccountType] ([AccountTypeId]),
    CONSTRAINT [FK_Account_Stakeholder] FOREIGN KEY ([StakeholderId]) REFERENCES [stakeholder].[Stakeholder] ([StakeholderId])
);