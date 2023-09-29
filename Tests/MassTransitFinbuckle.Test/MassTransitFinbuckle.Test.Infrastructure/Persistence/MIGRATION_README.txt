See https://learn.microsoft.com/ef/core/managing-schemas/migrations for information about
migrations using EF Core. You can perform these commands in the Visual Studio IDE using the
Package Manager Console (View > Other Windows > Package Manager Console) or using the dotnet
Command Line Interface (CLI) instructions. Substitute the {Keywords} below with the appropriate
migration name when executing these commands.

A separate "appsettings.json" file is used in this project for managing connection strings.

-------------------------------------------------------------------------------------------------------------------------------------------------------
Create a new migration:
-------------------------------------------------------------------------------------------------------------------------------------------------------
From the Visual Studio Package Manager Console:
Add-Migration -Name {ChangeName} -Project "MassTransitFinbuckle.Test.Infrastructure" -Context "ApplicationDbContext"  -- {ConnectionStringName}

CLI:
dotnet ef migrations add {ChangeName} --project "MassTransitFinbuckle.Test.Infrastructure" --context "ApplicationDbContext" -- {ConnectionStringName}

-------------------------------------------------------------------------------------------------------------------------------------------------------
Remove last migration:
-------------------------------------------------------------------------------------------------------------------------------------------------------
From the Visual Studio Package Manager Console:
Remove-Migration -Project "MassTransitFinbuckle.Test.Infrastructure" -Context "ApplicationDbContext"  -- {ConnectionStringName}

CLI:
dotnet ef migrations remove --project "MassTransitFinbuckle.Test.Infrastructure" --context "ApplicationDbContext" -- {ConnectionStringName}

-------------------------------------------------------------------------------------------------------------------------------------------------------
Update schema to the latest version:
-------------------------------------------------------------------------------------------------------------------------------------------------------
From the Visual Studio Package Manager Console:
Update-Database -Project "MassTransitFinbuckle.Test.Infrastructure" -Context "ApplicationDbContext"  -- {ConnectionStringName}

CLI:
dotnet ef database update --project "MassTransitFinbuckle.Test.Infrastructure" --context "ApplicationDbContext" -- {ConnectionStringName}

-------------------------------------------------------------------------------------------------------------------------------------------------------
Upgrade/downgrade schema to specific version:
-------------------------------------------------------------------------------------------------------------------------------------------------------
From the Visual Studio Package Manager Console:
Update-Database -Migration {Target} -Project "MassTransitFinbuckle.Test.Infrastructure" -Context "ApplicationDbContext"  -- {ConnectionStringName}

CLI:
dotnet ef database update {Target} --project "MassTransitFinbuckle.Test.Infrastructure" --context "ApplicationDbContext" -- {ConnectionStringName}

-------------------------------------------------------------------------------------------------------------------------------------------------------
Generate a script which detects the current database schema version and updates it to the latest:
-------------------------------------------------------------------------------------------------------------------------------------------------------
From the Visual Studio Package Manager Console:
Script-Migration -Idempotent -Project "MassTransitFinbuckle.Test.Infrastructure" -Context "ApplicationDbContext"  -- {ConnectionStringName}

CLI:
dotnet ef migrations script --idempotent --project "MassTransitFinbuckle.Test.Infrastructure" --context "ApplicationDbContext" -- {ConnectionStringName}

-------------------------------------------------------------------------------------------------------------------------------------------------------
Generate a script which upgrades from and to a specific schema version:
-------------------------------------------------------------------------------------------------------------------------------------------------------
From the Visual Studio Package Manager Console:
Script-Migration {Source} {Target} -Project "MassTransitFinbuckle.Test.Infrastructure" -Context "ApplicationDbContext"  -- {ConnectionStringName}

CLI:
dotnet ef migrations script {Source} {Target} --project "MassTransitFinbuckle.Test.Infrastructure" --context "ApplicationDbContext" -- {ConnectionStringName}

-------------------------------------------------------------------------------------------------------------------------------------------------------
Drop all tables in schema:
-------------------------------------------------------------------------------------------------------------------------------------------------------
DECLARE @SCHEMA AS varchar(max) = 'MassTransitFinbuckle.Test'
DECLARE @EXECUTE_STATEMENT AS varchar(max) = (SELECT STUFF((SELECT CHAR(13) + CHAR(10) + [Statement] FROM (
    SELECT 'ALTER TABLE ['+@SCHEMA+'].['+[t].[name]+'] DROP CONSTRAINT ['+[fk].[name]+']' AS [Statement] FROM [sys].[foreign_keys] AS [fk] INNER JOIN [sys].[tables] AS [t] ON [t].[object_id] = [fk].[parent_object_id] INNER JOIN [sys].[schemas] AS [s] ON [s].[schema_id] = [t].[schema_id] WHERE [s].[name] = @SCHEMA
    UNION ALL
    SELECT 'DROP TABLE ['+@SCHEMA+'].['+[t].[name]+']' AS [Statement] FROM [sys].[tables] AS [t] INNER JOIN [sys].[schemas] AS [s] ON [s].[schema_id] = [t].[schema_id] WHERE [s].[name] = @SCHEMA
) A FOR XML PATH(''), TYPE).value('.', 'NVARCHAR(MAX)'), 1, 1, ''))
EXECUTE(@EXECUTE_STATEMENT)
