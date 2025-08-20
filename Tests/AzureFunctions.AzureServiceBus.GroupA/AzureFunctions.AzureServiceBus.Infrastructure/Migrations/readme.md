# Entity Framework Core migrations readme

See <https://learn.microsoft.com//ef/core/managing-schemas/migrations> for information about migrations using Entity Framework Core.

You can perform these commands in the Visual Studio IDE using the Package Manager Console (View > Other Windows > Package Manager Console) or using the dotnet Command Line Interface (CLI) instructions.

Substitute the curly brace (`{}`) enclosed arguments below with the appropriate migration name when executing these commands.

> NOTE: When using `DesignTimeDbContextFactory`, it is recommended to use the `dotnet ef` CLI
> commands as the Visual Studio Package Manager Console may encounter issues in this scenario.);

A separate "appsettings.json" file is used in this project for managing connection strings.

## Visual Studio Package Manager Console quick reference

### Create a new migration (PMC)

```powershell
Add-Migration -Name {ChangeName} -Project "AzureFunctions.AzureServiceBus.Infrastructure" -Context "ApplicationDbContext"  -- {ConnectionStringName}
```

### Update the schema to the latest version (PMC)

```powershell
Update-Database -Project "AzureFunctions.AzureServiceBus.Infrastructure" -Context "ApplicationDbContext"  -- {ConnectionStringName}
```

## .NET CLI quick reference

### Create a new migration (CLI)

```powershell
dotnet ef migrations add {ChangeName} --project "AzureFunctions.AzureServiceBus.Infrastructure" --context "ApplicationDbContext" -- {ConnectionStringName}
```

### Update the schema to the latest version (CLI)

```powershell
dotnet ef database update --project "AzureFunctions.AzureServiceBus.Infrastructure" --context "ApplicationDbContext" -- {ConnectionStringName}
```

## Visual Studio Package Manager Console additional commands

### Generate a script which detects the current database schema version and updates it to the latest (PMC)

```powershell
Script-Migration -Idempotent -Project "AzureFunctions.AzureServiceBus.Infrastructure" -Context "ApplicationDbContext"  -- {ConnectionStringName}
```

### Generate a script which upgrades from and to a specific schema version (PMC)

```powershell
Script-Migration {Source} {Target} -Project "AzureFunctions.AzureServiceBus.Infrastructure" -Context "ApplicationDbContext"  -- {ConnectionStringName}
```

### Upgrade/downgrade the schema to a specific version (PMC)

```powershell
Update-Database -Migration {Target} -Project "AzureFunctions.AzureServiceBus.Infrastructure" -Context "ApplicationDbContext"  -- {ConnectionStringName}
```

### Remove the last created migration (PMC)

```powershell
Remove-Migration -Project "AzureFunctions.AzureServiceBus.Infrastructure" -Context "ApplicationDbContext"  -- {ConnectionStringName}
```

## .NET CLI additional commands

### Generate a script which detects the current database schema version and updates it to the latest (CLI)

```powershell
dotnet ef migrations script --idempotent --project "AzureFunctions.AzureServiceBus.Infrastructure" --context "ApplicationDbContext" -- {ConnectionStringName}
```

### Generate a script which upgrades from and to a specific schema version (CLI)

```powershell
dotnet ef migrations script {Source} {Target} --project "AzureFunctions.AzureServiceBus.Infrastructure" --context "ApplicationDbContext" -- {ConnectionStringName}
```

### Upgrade/downgrade the schema to a specific version (CLI)

```powershell
dotnet ef database update {Target} --project "AzureFunctions.AzureServiceBus.Infrastructure" --context "ApplicationDbContext" -- {ConnectionStringName}
```

### Remove the last created migration (CLI)

```powershell
dotnet ef migrations remove --project "AzureFunctions.AzureServiceBus.Infrastructure" --context "ApplicationDbContext" -- {ConnectionStringName}
```

## Drop all tables in a schema

```sql
DECLARE @SCHEMA AS varchar(max) = 'AzureFunctions.AzureServiceBus.GroupA'
DECLARE @EXECUTE_STATEMENT AS varchar(max) = (SELECT STUFF((SELECT CHAR(13) + CHAR(10) + [Statement] FROM (
    SELECT 'ALTER TABLE ['+@SCHEMA+'].['+[t].[name]+'] DROP CONSTRAINT ['+[fk].[name]+']' AS [Statement] FROM [sys].[foreign_keys] AS [fk] INNER JOIN [sys].[tables] AS [t] ON [t].[object_id] = [fk].[parent_object_id] INNER JOIN [sys].[schemas] AS [s] ON [s].[schema_id] = [t].[schema_id] WHERE [s].[name] = @SCHEMA
    UNION ALL
    SELECT 'DROP TABLE ['+@SCHEMA+'].['+[t].[name]+']' AS [Statement] FROM [sys].[tables] AS [t] INNER JOIN [sys].[schemas] AS [s] ON [s].[schema_id] = [t].[schema_id] WHERE [s].[name] = @SCHEMA
) A FOR XML PATH(''), TYPE).value('.', 'NVARCHAR(MAX)'), 1, 1, ''))
EXECUTE(@EXECUTE_STATEMENT)
```
