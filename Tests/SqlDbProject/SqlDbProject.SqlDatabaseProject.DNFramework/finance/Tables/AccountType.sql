CREATE TABLE [finance].[AccountType]
(
    [AccountTypeId] INT NOT NULL,
    [Description] VARCHAR(260) NOT NULL,
    [Blank] NVARCHAR(MAX) NOT NULL,
    CONSTRAINT [PK_AccountType] PRIMARY KEY CLUSTERED ([AccountTypeId] ASC)
);