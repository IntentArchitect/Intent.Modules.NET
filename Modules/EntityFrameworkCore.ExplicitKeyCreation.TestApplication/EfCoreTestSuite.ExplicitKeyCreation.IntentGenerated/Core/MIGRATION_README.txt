Read here about Migrations using EF Core: https://learn.microsoft.com/en-us/ef/core/managing-schemas/migrations
You can perform these commands in Visual Studio IDE (VS) using the Package Manager Console (View > Other Windows > Package Manager Console)
or using the dotnet Command Line Interface (CLI) instructions.
Substitute the {Keywords} below with the appropriate migration name when executing these commands.

Create a new migration:
-------------------------------------------------------------------------------------------------------------------------------------------------------
VS:  Add-Migration -Name {ChangeName} -StartupProject "UNKNOWN" -Project "EfCoreTestSuite.ExplicitKeyCreation.IntentGenerated"
CLI: dotnet ef migrations add {ChangeName} --project "EfCoreTestSuite.ExplicitKeyCreation.IntentGenerated" --startup-project "UNKNOWN"

Remove last migration:
-------------------------------------------------------------------------------------------------------------------------------------------------------
VS:  Remove-Migration
CLI: dotnet ef migrations remove

Update schema to the latest version:
-------------------------------------------------------------------------------------------------------------------------------------------------------
VS:  Update-Database -StartupProject "UNKNOWN" -Project "EfCoreTestSuite.ExplicitKeyCreation.IntentGenerated"
CLI: dotnet ef database update --project "EfCoreTestSuite.ExplicitKeyCreation.IntentGenerated" --startup-project "UNKNOWN" 

Upgrade/downgrade schema to specific version:
-------------------------------------------------------------------------------------------------------------------------------------------------------
VS:  Update-Database -Migration {Target} -StartupProject "UNKNOWN" -Project EfCoreTestSuite.ExplicitKeyCreation.IntentGenerated
CLI: dotnet ef database update {Target} --project "EfCoreTestSuite.ExplicitKeyCreation.IntentGenerated" --startup-project "UNKNOWN"

Generate a script which detects the current database schema version and updates it to the latest:
-------------------------------------------------------------------------------------------------------------------------------------------------------
VS:  Script-Migration -StartupProject "UNKNOWN" -Project EfCoreTestSuite.ExplicitKeyCreation.IntentGenerated
CLI: dotnet ef migrations script --project "EfCoreTestSuite.ExplicitKeyCreation.IntentGenerated" --startup-project "UNKNOWN"

Generate a script which upgrades from and to a specific schema version:
-------------------------------------------------------------------------------------------------------------------------------------------------------
VS:  Script-Migration {Source} {Target} -StartupProject "UNKNOWN" -Project EfCoreTestSuite.ExplicitKeyCreation.IntentGenerated
CLI: dotnet ef migrations script {Source} {Target} --project "EfCoreTestSuite.ExplicitKeyCreation.IntentGenerated" --startup-project "UNKNOWN"

Drop all tables in schema:
-------------------------------------------------------------------------------------------------------------------------------------------------------
DECLARE @SCHEMA AS varchar(max) = 'EntityFrameworkCore.ExplicitKeyCreation.TestApplication'
DECLARE @EXECUTE_STATEMENT AS varchar(max) = (SELECT STUFF((SELECT CHAR(13) + CHAR(10) + [Statement] FROM (
    SELECT 'ALTER TABLE ['+@SCHEMA+'].['+[t].[name]+'] DROP CONSTRAINT ['+[fk].[name]+']' AS [Statement] FROM [sys].[foreign_keys] AS [fk] INNER JOIN [sys].[tables] AS [t] ON [t].[object_id] = [fk].[parent_object_id] INNER JOIN [sys].[schemas] AS [s] ON [s].[schema_id] = [t].[schema_id] WHERE [s].[name] = @SCHEMA
    UNION ALL
    SELECT 'DROP TABLE ['+@SCHEMA+'].['+[t].[name]+']' AS [Statement] FROM [sys].[tables] AS [t] INNER JOIN [sys].[schemas] AS [s] ON [s].[schema_id] = [t].[schema_id] WHERE [s].[name] = @SCHEMA
) A FOR XML PATH(''), TYPE).value('.', 'NVARCHAR(MAX)'), 1, 1, ''))
EXECUTE(@EXECUTE_STATEMENT)
