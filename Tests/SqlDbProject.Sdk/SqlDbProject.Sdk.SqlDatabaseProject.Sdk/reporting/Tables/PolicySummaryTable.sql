CREATE TABLE [reporting].[PolicySummaryTable]
(
    [Id] UNIQUEIDENTIFIER NOT NULL DEFAULT (NEWSEQUENTIALID()),
    [PolicyNumber] NVARCHAR(MAX) NOT NULL,
    [AccountName] NVARCHAR(MAX) NOT NULL,
    [CountryCode] NVARCHAR(MAX) NOT NULL,
    [Status] NVARCHAR(MAX) NOT NULL,
    [TotalPremium] DECIMAL NOT NULL,
    [EffectiveDate] DATETIME NOT NULL,
    [ExpiryDate] DATETIME NULL,
    CONSTRAINT [PK_PolicySummaryTable] PRIMARY KEY CLUSTERED ([Id] ASC)
);