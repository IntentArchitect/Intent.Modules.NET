See https://learn.microsoft.com/ef/core/managing-schemas/migrations for information about
migrations using EF Core. You can perform these commands in the Visual Studio IDE using the
Package Manager Console (View > Other Windows > Package Manager Console) or using the dotnet
Command Line Interface (CLI) instructions. Substitute the {Keywords} below with the appropriate
migration name when executing these commands.

-------------------------------------------------------------------------------------------------------------------------------------------------------
Create a new migration:
-------------------------------------------------------------------------------------------------------------------------------------------------------
From the Visual Studio Package Manager Console:
Add-Migration -Name {ChangeName} -StartupProject "Subscribe.MassTransit.TestApplication.Api" -Project "Subscribe.MassTransit.TestApplication.Infrastructure"

CLI:
dotnet ef migrations add {ChangeName} --startup-project "Subscribe.MassTransit.TestApplication.Api" --project "Subscribe.MassTransit.TestApplication.Infrastructure"

-------------------------------------------------------------------------------------------------------------------------------------------------------
Remove last migration:
-------------------------------------------------------------------------------------------------------------------------------------------------------
From the Visual Studio Package Manager Console:
Remove-Migration -StartupProject "Subscribe.MassTransit.TestApplication.Api" -Project "Subscribe.MassTransit.TestApplication.Infrastructure"

CLI:
dotnet ef migrations remove --startup-project "Subscribe.MassTransit.TestApplication.Api" --project "Subscribe.MassTransit.TestApplication.Infrastructure"

-------------------------------------------------------------------------------------------------------------------------------------------------------
Update schema to the latest version:
-------------------------------------------------------------------------------------------------------------------------------------------------------
From the Visual Studio Package Manager Console:
Update-Database -StartupProject "Subscribe.MassTransit.TestApplication.Api" -Project "Subscribe.MassTransit.TestApplication.Infrastructure"

CLI:
dotnet ef database update --startup-project "Subscribe.MassTransit.TestApplication.Api" --project "Subscribe.MassTransit.TestApplication.Infrastructure" 

-------------------------------------------------------------------------------------------------------------------------------------------------------
Upgrade/downgrade schema to specific version:
-------------------------------------------------------------------------------------------------------------------------------------------------------
From the Visual Studio Package Manager Console:
Update-Database -Migration {Target} -StartupProject "Subscribe.MassTransit.TestApplication.Api" -Project "Subscribe.MassTransit.TestApplication.Infrastructure"

CLI:
dotnet ef database update {Target} --startup-project "Subscribe.MassTransit.TestApplication.Api" --project "Subscribe.MassTransit.TestApplication.Infrastructure"

-------------------------------------------------------------------------------------------------------------------------------------------------------
Generate a script which detects the current database schema version and updates it to the latest:
-------------------------------------------------------------------------------------------------------------------------------------------------------
From the Visual Studio Package Manager Console:
Script-Migration -Idempotent -StartupProject "Subscribe.MassTransit.TestApplication.Api" -Project "Subscribe.MassTransit.TestApplication.Infrastructure"

CLI:
dotnet ef migrations script --idempotent --startup-project "Subscribe.MassTransit.TestApplication.Api" --project "Subscribe.MassTransit.TestApplication.Infrastructure"

-------------------------------------------------------------------------------------------------------------------------------------------------------
Generate a script which upgrades from and to a specific schema version:
-------------------------------------------------------------------------------------------------------------------------------------------------------
From the Visual Studio Package Manager Console:
Script-Migration {Source} {Target} -StartupProject "Subscribe.MassTransit.TestApplication.Api" -Project "Subscribe.MassTransit.TestApplication.Infrastructure"

CLI:
dotnet ef migrations script {Source} {Target} --startup-project "Subscribe.MassTransit.TestApplication.Api" --project "Subscribe.MassTransit.TestApplication.Infrastructure"

-------------------------------------------------------------------------------------------------------------------------------------------------------
Drop all tables in schema:
-------------------------------------------------------------------------------------------------------------------------------------------------------
DECLARE @SCHEMA AS varchar(max) = 'Subscribe.MassTransit.TestApplication'
DECLARE @EXECUTE_STATEMENT AS varchar(max) = (SELECT STUFF((SELECT CHAR(13) + CHAR(10) + [Statement] FROM (
    SELECT 'ALTER TABLE ['+@SCHEMA+'].['+[t].[name]+'] DROP CONSTRAINT ['+[fk].[name]+']' AS [Statement] FROM [sys].[foreign_keys] AS [fk] INNER JOIN [sys].[tables] AS [t] ON [t].[object_id] = [fk].[parent_object_id] INNER JOIN [sys].[schemas] AS [s] ON [s].[schema_id] = [t].[schema_id] WHERE [s].[name] = @SCHEMA
    UNION ALL
    SELECT 'DROP TABLE ['+@SCHEMA+'].['+[t].[name]+']' AS [Statement] FROM [sys].[tables] AS [t] INNER JOIN [sys].[schemas] AS [s] ON [s].[schema_id] = [t].[schema_id] WHERE [s].[name] = @SCHEMA
) A FOR XML PATH(''), TYPE).value('.', 'NVARCHAR(MAX)'), 1, 1, ''))
EXECUTE(@EXECUTE_STATEMENT)
