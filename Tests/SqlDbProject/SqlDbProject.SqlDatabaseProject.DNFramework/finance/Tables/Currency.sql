CREATE TABLE [finance].[Currency]
(
    [CurrencyIso] INT NOT NULL,
    [Country] NVARCHAR(255) NOT NULL,
    [Description] NVARCHAR(255) NOT NULL,
    [AlphaCode] NCHAR(3) NOT NULL,
    [NumericCode] DECIMAL NOT NULL,
    [IsEnabled] BIT NOT NULL DEFAULT 1,
    [Sequence] INT NULL,
    CONSTRAINT [PK_Currency] PRIMARY KEY CLUSTERED ([CurrencyIso] ASC)
);