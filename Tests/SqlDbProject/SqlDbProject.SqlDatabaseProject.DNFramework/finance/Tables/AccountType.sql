CREATE TABLE [finance].[AccountType]
(
    [AccountTypeId] INT NOT NULL,
    [Description] NVARCHAR(255) NOT NULL,
    CONSTRAINT [PK_AccountType] PRIMARY KEY CLUSTERED ([AccountTypeId] ASC)
);