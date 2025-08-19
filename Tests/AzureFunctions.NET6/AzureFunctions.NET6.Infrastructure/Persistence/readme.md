# Entity Framework Core migrations readme

See <https://learn.microsoft.com//ef/core/managing-schemas/migrations> for information about migrations using Entity Framework Core.

You can perform these commands in the Visual Studio IDE using the Package Manager Console (View > Other Windows > Package Manager Console) or using the dotnet Command Line Interface (CLI) instructions.

Substitute the curly brace (`{}`) enclosed arguments below with the appropriate migration name when executing these commands.

> NOTE: When using `DesignTimeDbContextFactory`, it is recommended to use the `dotnet ef` CLI
> commands as the Visual Studio Package Manager Console may encounter issues in this scenario.);

A separate "appsettings.json" file is used in this project for managing connection strings.

## Visual Studio Package Manager Console quick reference

```powershell
# Create a new migration
Add-Migration -Name {ChangeName} -Project "AzureFunctions.NET6.Infrastructure" -Context "ApplicationDbContext"  -- {ConnectionStringName}

# Update the schema to the latest version
Update-Database -Project "AzureFunctions.NET6.Infrastructure" -Context "ApplicationDbContext"  -- {ConnectionStringName}

# Generate a script which detects the current database schema version and updates it to the latest
Script-Migration -Idempotent -Project "AzureFunctions.NET6.Infrastructure" -Context "ApplicationDbContext"  -- {ConnectionStringName}

# Generate a script which upgrades from and to a specific schema version
Script-Migration {Source} {Target} -Project "AzureFunctions.NET6.Infrastructure" -Context "ApplicationDbContext"  -- {ConnectionStringName}

# Upgrade/downgrade the schema to a specific version
Update-Database -Migration {Target} -Project "AzureFunctions.NET6.Infrastructure" -Context "ApplicationDbContext"  -- {ConnectionStringName}

# Remove the last created migration
Remove-Migration -Project "AzureFunctions.NET6.Infrastructure" -Context "ApplicationDbContext"  -- {ConnectionStringName}
```

## .NET CLI quick reference

```powershell
# Create a new migration
dotnet ef migrations add {ChangeName} --project "AzureFunctions.NET6.Infrastructure" --context "ApplicationDbContext" -- {ConnectionStringName}

# Update the schema to the latest version
dotnet ef database update --project "AzureFunctions.NET6.Infrastructure" --context "ApplicationDbContext" -- {ConnectionStringName}

# Generate a script which detects the current database schema version and updates it to the latest
dotnet ef migrations script --idempotent --project "AzureFunctions.NET6.Infrastructure" --context "ApplicationDbContext" -- {ConnectionStringName}

# Generate a script which upgrades from and to a specific schema version
dotnet ef migrations script {Source} {Target} --project "AzureFunctions.NET6.Infrastructure" --context "ApplicationDbContext" -- {ConnectionStringName}

# Upgrade/downgrade the schema to a specific version
dotnet ef database update {Target} --project "AzureFunctions.NET6.Infrastructure" --context "ApplicationDbContext" -- {ConnectionStringName}

# Remove the last created migration
dotnet ef migrations remove --project "AzureFunctions.NET6.Infrastructure" --context "ApplicationDbContext" -- {ConnectionStringName}
```

## Drop all tables in a schema

```sql
DECLARE @SCHEMA AS varchar(max) = 'AzureFunctions.NET6'
DECLARE @EXECUTE_STATEMENT AS varchar(max) = (SELECT STUFF((SELECT CHAR(13) + CHAR(10) + [Statement] FROM (
    SELECT 'ALTER TABLE ['+@SCHEMA+'].['+[t].[name]+'] DROP CONSTRAINT ['+[fk].[name]+']' AS [Statement] FROM [sys].[foreign_keys] AS [fk] INNER JOIN [sys].[tables] AS [t] ON [t].[object_id] = [fk].[parent_object_id] INNER JOIN [sys].[schemas] AS [s] ON [s].[schema_id] = [t].[schema_id] WHERE [s].[name] = @SCHEMA
    UNION ALL
    SELECT 'DROP TABLE ['+@SCHEMA+'].['+[t].[name]+']' AS [Statement] FROM [sys].[tables] AS [t] INNER JOIN [sys].[schemas] AS [s] ON [s].[schema_id] = [t].[schema_id] WHERE [s].[name] = @SCHEMA
) A FOR XML PATH(''), TYPE).value('.', 'NVARCHAR(MAX)'), 1, 1, ''))
EXECUTE(@EXECUTE_STATEMENT)
```
