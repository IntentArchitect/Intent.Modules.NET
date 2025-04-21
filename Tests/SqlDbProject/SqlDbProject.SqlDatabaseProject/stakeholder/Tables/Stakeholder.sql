CREATE TABLE [stakeholder].[Stakeholder]
(
    [StakeholderId] BIGINT NOT NULL,
    [Name] NVARCHAR(255) NOT NULL,
    [DateCreated] DATETIME NOT NULL,
    CONSTRAINT [PK_Stakeholder] PRIMARY KEY CLUSTERED ([StakeholderId] ASC)
);