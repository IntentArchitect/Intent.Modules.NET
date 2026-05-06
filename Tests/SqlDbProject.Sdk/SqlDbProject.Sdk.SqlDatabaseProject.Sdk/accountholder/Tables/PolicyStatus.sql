CREATE TABLE [accountholder].[PolicyStatus]
(
    [PolicyStatusId] UNIQUEIDENTIFIER NOT NULL,
    [Description] NVARCHAR(255) NOT NULL,
    CONSTRAINT [PK_PolicyStatus] PRIMARY KEY CLUSTERED ([PolicyStatusId] ASC)
);