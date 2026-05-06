CREATE TABLE [dbo].[Country]
(
    [CountryIso] CHAR(2) NOT NULL,
    [Description] NVARCHAR(64) NOT NULL,
    [CurrencyIso] CHAR(3) NOT NULL,
    [DialingCode] NVARCHAR(8) NOT NULL,
    CONSTRAINT [PK_Country] PRIMARY KEY CLUSTERED ([CountryIso] ASC)
);