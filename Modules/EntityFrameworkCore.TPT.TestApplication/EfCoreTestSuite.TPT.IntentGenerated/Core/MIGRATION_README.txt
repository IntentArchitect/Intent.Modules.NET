Create a new migration:
-------------------------------------------------------------------------------------------------------------------------------------------------------
Add-Migration -Name {ChangeName} -StartupProject "UNKNOWN" -Project EfCoreTestSuite.TPT.IntentGenerated


Overwrite an existing migration:
-------------------------------------------------------------------------------------------------------------------------------------------------------
Add-Migration -Name {ChangeName} -StartupProject "UNKNOWN" -Project EfCoreTestSuite.TPT.IntentGenerated


Update schema to the latest version:
-------------------------------------------------------------------------------------------------------------------------------------------------------
Update-Database -StartupProject "UNKNOWN" -Project EfCoreTestSuite.TPT.IntentGenerated


Upgrade/downgrade schema to specific version:
-------------------------------------------------------------------------------------------------------------------------------------------------------
Update-Database -Migration {Target} -StartupProject "UNKNOWN" -Project EfCoreTestSuite.TPT.IntentGenerated


Generate a script which detects the current database schema version and updates it to the latest:
-------------------------------------------------------------------------------------------------------------------------------------------------------
Script-Migration -SourceMigration:$InitialDatabase -Script -StartupProject "UNKNOWN" -Project EfCoreTestSuite.TPT.IntentGenerated


Generate a script which upgrades from and to a specific schema version:
-------------------------------------------------------------------------------------------------------------------------------------------------------
Script-Migration -SourceMigration:{Source} -TargetMigration:{Target} -Script -StartupProject "UNKNOWN" -Project EfCoreTestSuite.TPT.IntentGenerated


Drop all tables in schema:
-------------------------------------------------------------------------------------------------------------------------------------------------------
DECLARE @SCHEMA AS varchar(max) = 'EntityFrameworkCore.TPT.TestApplication'
DECLARE @EXECUTE_STATEMENT AS varchar(max) = (SELECT STUFF((SELECT CHAR(13) + CHAR(10) + [Statement] FROM (
    SELECT 'ALTER TABLE ['+@SCHEMA+'].['+[t].[name]+'] DROP CONSTRAINT ['+[fk].[name]+']' AS [Statement] FROM [sys].[foreign_keys] AS [fk] INNER JOIN [sys].[tables] AS [t] ON [t].[object_id] = [fk].[parent_object_id] INNER JOIN [sys].[schemas] AS [s] ON [s].[schema_id] = [t].[schema_id] WHERE [s].[name] = @SCHEMA
    UNION ALL
    SELECT 'DROP TABLE ['+@SCHEMA+'].['+[t].[name]+']' AS [Statement] FROM [sys].[tables] AS [t] INNER JOIN [sys].[schemas] AS [s] ON [s].[schema_id] = [t].[schema_id] WHERE [s].[name] = @SCHEMA
) A FOR XML PATH(''), TYPE).value('.', 'NVARCHAR(MAX)'), 1, 1, ''))
EXECUTE(@EXECUTE_STATEMENT)
