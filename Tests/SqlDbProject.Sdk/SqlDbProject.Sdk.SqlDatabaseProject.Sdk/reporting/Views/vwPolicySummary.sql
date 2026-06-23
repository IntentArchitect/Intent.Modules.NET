CREATE VIEW [reporting].[vwPolicySummary]
AS
SELECT
         [Id],
         [PolicyNumber],
         [AccountName],
         [CountryCode],
         [Status],
         [TotalPremium],
         [EffectiveDate],
         [ExpiryDate]
     FROM [reporting].[PolicySummaryTable]
;