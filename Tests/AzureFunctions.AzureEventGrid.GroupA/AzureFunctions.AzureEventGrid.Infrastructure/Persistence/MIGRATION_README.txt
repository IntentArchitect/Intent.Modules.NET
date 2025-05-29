See https://learn.microsoft.com/ef/core/managing-schemas/migrations for information about
migrations using EF Core. You can perform these commands in the Visual Studio IDE using the
Package Manager Console (View > Other Windows > Package Manager Console) or using the dotnet
Command Line Interface (CLI) instructions. Substitute the {Keywords} below with the appropriate
migration name when executing these commands.

NOTE: When using DesignTimeDbContextFactory, it is recommended to use the dotnet ef CLI tool. The Visual Studio Package Manager Console (PMC) may encounter issues in this scenario.

A separate "appsettings.json" file is used in this project for managing connection strings.

-------------------------------------------------------------------------------------------------------------------------------------------------------
Create a new migration:
-------------------------------------------------------------------------------------------------------------------------------------------------------
CLI:
dotnet ef migrations add {ChangeName} --project "AzureFunctions.AzureEventGrid.Infrastructure" --context "ApplicationDbContext" -- {ConnectionStringName}

-------------------------------------------------------------------------------------------------------------------------------------------------------
Remove last migration:
-------------------------------------------------------------------------------------------------------------------------------------------------------
CLI:
dotnet ef migrations remove --project "AzureFunctions.AzureEventGrid.Infrastructure" --context "ApplicationDbContext" -- {ConnectionStringName}

-------------------------------------------------------------------------------------------------------------------------------------------------------
Update schema to the latest version:
-------------------------------------------------------------------------------------------------------------------------------------------------------
CLI:
dotnet ef database update --project "AzureFunctions.AzureEventGrid.Infrastructure" --context "ApplicationDbContext" -- {ConnectionStringName}

-------------------------------------------------------------------------------------------------------------------------------------------------------
Upgrade/downgrade schema to specific version:
-------------------------------------------------------------------------------------------------------------------------------------------------------
CLI:
dotnet ef database update {Target} --project "AzureFunctions.AzureEventGrid.Infrastructure" --context "ApplicationDbContext" -- {ConnectionStringName}

-------------------------------------------------------------------------------------------------------------------------------------------------------
Generate a script which detects the current database schema version and updates it to the latest:
-------------------------------------------------------------------------------------------------------------------------------------------------------
CLI:
dotnet ef migrations script --idempotent --project "AzureFunctions.AzureEventGrid.Infrastructure" --context "ApplicationDbContext" -- {ConnectionStringName}

-------------------------------------------------------------------------------------------------------------------------------------------------------
Generate a script which upgrades from and to a specific schema version:
-------------------------------------------------------------------------------------------------------------------------------------------------------
CLI:
dotnet ef migrations script {Source} {Target} --project "AzureFunctions.AzureEventGrid.Infrastructure" --context "ApplicationDbContext" -- {ConnectionStringName}

-------------------------------------------------------------------------------------------------------------------------------------------------------
Drop all tables in schema:
-------------------------------------------------------------------------------------------------------------------------------------------------------
DECLARE @SCHEMA AS varchar(max) = 'AzureFunctions.AzureEventGrid.GroupA'
DECLARE @EXECUTE_STATEMENT AS varchar(max) = (SELECT STUFF((SELECT CHAR(13) + CHAR(10) + [Statement] FROM (
    SELECT 'ALTER TABLE ['+@SCHEMA+'].['+[t].[name]+'] DROP CONSTRAINT ['+[fk].[name]+']' AS [Statement] FROM [sys].[foreign_keys] AS [fk] INNER JOIN [sys].[tables] AS [t] ON [t].[object_id] = [fk].[parent_object_id] INNER JOIN [sys].[schemas] AS [s] ON [s].[schema_id] = [t].[schema_id] WHERE [s].[name] = @SCHEMA
    UNION ALL
    SELECT 'DROP TABLE ['+@SCHEMA+'].['+[t].[name]+']' AS [Statement] FROM [sys].[tables] AS [t] INNER JOIN [sys].[schemas] AS [s] ON [s].[schema_id] = [t].[schema_id] WHERE [s].[name] = @SCHEMA
) A FOR XML PATH(''), TYPE).value('.', 'NVARCHAR(MAX)'), 1, 1, ''))
EXECUTE(@EXECUTE_STATEMENT)
