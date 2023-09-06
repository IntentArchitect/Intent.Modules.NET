Read here about Migrations using EF Core: https://learn.microsoft.com/en-us/ef/core/managing-schemas/migrations
You can perform these commands in Visual Studio IDE (VS) using the Package Manager Console (View > Other Windows > Package Manager Console)
or using the dotnet Command Line Interface (CLI) instructions.
Substitute the {Keywords} below with the appropriate migration name when executing these commands.

This file has been created in addition to the MIGRATION_README.txt for allowing Design Time DB Contexts
to be used instead of the original one. This allows for migration operations to be performed without
making use of the Hosting application's Startup procedure.
A separate "appsettings.json" file is used in this project for managing connection strings.

Create a new migration:
-------------------------------------------------------------------------------------------------------------------------------------------------------
VS:  Add-Migration -Name {ChangeName} -Project "AzureFunctions.TestApplication.Infrastructure" -- {ConnectionStringName}
CLI: dotnet ef migrations add {ChangeName} --project "AzureFunctions.TestApplication.Infrastructure" -- {ConnectionStringName}

Remove last migration:
-------------------------------------------------------------------------------------------------------------------------------------------------------
VS:  Remove-Migration  -Project "AzureFunctions.TestApplication.Infrastructure" -- {ConnectionStringName}
CLI: dotnet ef migrations remove --project "AzureFunctions.TestApplication.Infrastructure" -- {ConnectionStringName}

Update schema to the latest version:
-------------------------------------------------------------------------------------------------------------------------------------------------------
VS:  Update-Database -Project "AzureFunctions.TestApplication.Infrastructure" -- {ConnectionStringName}
CLI: dotnet ef database update --project "AzureFunctions.TestApplication.Infrastructure"  -- {ConnectionStringName}

Upgrade/downgrade schema to specific version:
-------------------------------------------------------------------------------------------------------------------------------------------------------
VS:  Update-Database -Migration {Target} -Project "AzureFunctions.TestApplication.Infrastructure" -- {ConnectionStringName}
CLI: dotnet ef database update {Target} --project "AzureFunctions.TestApplication.Infrastructure" -- {ConnectionStringName}

Generate a script which detects the current database schema version and updates it to the latest:
-------------------------------------------------------------------------------------------------------------------------------------------------------
VS:  Script-Migration -Project "AzureFunctions.TestApplication.Infrastructure" -- {ConnectionStringName}
CLI: dotnet ef migrations script --project "AzureFunctions.TestApplication.Infrastructure" -- {ConnectionStringName}

Generate a script which upgrades from and to a specific schema version:
-------------------------------------------------------------------------------------------------------------------------------------------------------
VS:  Script-Migration {Source} {Target} -Project "AzureFunctions.TestApplication.Infrastructure" -- {ConnectionStringName}
CLI: dotnet ef migrations script {Source} {Target} --project "AzureFunctions.TestApplication.Infrastructure" -- {ConnectionStringName}

Drop all tables in schema:
-------------------------------------------------------------------------------------------------------------------------------------------------------
DECLARE @SCHEMA AS varchar(max) = 'AzureFunctions.TestApplication'
DECLARE @EXECUTE_STATEMENT AS varchar(max) = (SELECT STUFF((SELECT CHAR(13) + CHAR(10) + [Statement] FROM (
    SELECT 'ALTER TABLE ['+@SCHEMA+'].['+[t].[name]+'] DROP CONSTRAINT ['+[fk].[name]+']' AS [Statement] FROM [sys].[foreign_keys] AS [fk] INNER JOIN [sys].[tables] AS [t] ON [t].[object_id] = [fk].[parent_object_id] INNER JOIN [sys].[schemas] AS [s] ON [s].[schema_id] = [t].[schema_id] WHERE [s].[name] = @SCHEMA
    UNION ALL
    SELECT 'DROP TABLE ['+@SCHEMA+'].['+[t].[name]+']' AS [Statement] FROM [sys].[tables] AS [t] INNER JOIN [sys].[schemas] AS [s] ON [s].[schema_id] = [t].[schema_id] WHERE [s].[name] = @SCHEMA
) A FOR XML PATH(''), TYPE).value('.', 'NVARCHAR(MAX)'), 1, 1, ''))
EXECUTE(@EXECUTE_STATEMENT)
