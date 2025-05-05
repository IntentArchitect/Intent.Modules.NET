CREATE TABLE [accountholder].[AccountHolder]
(
    [AccountHolderId] BIGINT NOT NULL,
    [Name] NVARCHAR(255) NOT NULL,
    [DateCreated] DATETIME NOT NULL,
    CONSTRAINT [PK_AccountHolder] PRIMARY KEY CLUSTERED ([AccountHolderId] ASC)
);