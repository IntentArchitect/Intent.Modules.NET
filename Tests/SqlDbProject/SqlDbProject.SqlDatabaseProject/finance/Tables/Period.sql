CREATE TABLE [finance].[Period]
(
    [PeriodId] INT IDENTITY (1,1) NOT NULL,
    [Description] NVARCHAR(255) NOT NULL,
    [StartDate] DATE NOT NULL,
    CONSTRAINT [PK_Period] PRIMARY KEY CLUSTERED ([PeriodId] ASC)
);