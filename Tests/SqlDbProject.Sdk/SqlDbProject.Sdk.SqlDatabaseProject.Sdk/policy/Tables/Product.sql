CREATE TABLE [policy].[Product]
(
    [ProductId] INT NOT NULL,
    [CountryIso] CHAR(2) NOT NULL,
    [Name] NVARCHAR(255) NOT NULL,
    [Description] NVARCHAR(1024) NOT NULL,
    [IsDefault] BIT NOT NULL,
    [RatingMethodId] INT NOT NULL,
    [ProductRatingConfig] TEXT NOT NULL,
    CONSTRAINT [PK_Product] PRIMARY KEY CLUSTERED ([ProductId] ASC),
    CONSTRAINT [FK_Product_Country] FOREIGN KEY ([CountryIso]) REFERENCES [dbo].[Country] ([CountryIso])
);