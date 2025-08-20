# Entity Framework Core migrations readme

See <https://learn.microsoft.com//ef/core/managing-schemas/migrations> for information about migrations using Entity Framework Core.

You can perform these commands in the Visual Studio IDE using the Package Manager Console (View > Other Windows > Package Manager Console) or using the dotnet Command Line Interface (CLI) instructions.

Substitute the curly brace (`{}`) enclosed arguments below with the appropriate migration name when executing these commands.

## Visual Studio Package Manager Console quick reference

### Create a new migration (PMC)

```powershell
Add-Migration -Name {ChangeName} -StartupProject "ProxyServiceTests.Proxy.PTH.Api" -Project "ProxyServiceTests.Proxy.PTH.Infrastructure"
```

### Update the schema to the latest version (PMC)

```powershell
Update-Database -StartupProject "ProxyServiceTests.Proxy.PTH.Api" -Project "ProxyServiceTests.Proxy.PTH.Infrastructure"
```

## .NET CLI quick reference

### Create a new migration (CLI)

```powershell
dotnet ef migrations add {ChangeName} --startup-project "ProxyServiceTests.Proxy.PTH.Api" --project "ProxyServiceTests.Proxy.PTH.Infrastructure"
```

### Update the schema to the latest version (CLI)

```powershell
dotnet ef database update --startup-project "ProxyServiceTests.Proxy.PTH.Api" --project "ProxyServiceTests.Proxy.PTH.Infrastructure"
```

## Visual Studio Package Manager Console additional commands

### Generate a script which detects the current database schema version and updates it to the latest (PMC)

```powershell
Script-Migration -Idempotent -StartupProject "ProxyServiceTests.Proxy.PTH.Api" -Project "ProxyServiceTests.Proxy.PTH.Infrastructure"
```

### Generate a script which upgrades from and to a specific schema version (PMC)

```powershell
Script-Migration {Source} {Target} -StartupProject "ProxyServiceTests.Proxy.PTH.Api" -Project "ProxyServiceTests.Proxy.PTH.Infrastructure"
```

### Upgrade/downgrade the schema to a specific version (PMC)

```powershell
Update-Database -Migration {Target} -StartupProject "ProxyServiceTests.Proxy.PTH.Api" -Project "ProxyServiceTests.Proxy.PTH.Infrastructure"
```

### Remove the last created migration (PMC)

```powershell
Remove-Migration -StartupProject "ProxyServiceTests.Proxy.PTH.Api" -Project "ProxyServiceTests.Proxy.PTH.Infrastructure"
```

## .NET CLI additional commands

### Generate a script which detects the current database schema version and updates it to the latest (CLI)

```powershell
dotnet ef migrations script --idempotent --startup-project "ProxyServiceTests.Proxy.PTH.Api" --project "ProxyServiceTests.Proxy.PTH.Infrastructure"
```

### Generate a script which upgrades from and to a specific schema version (CLI)

```powershell
dotnet ef migrations script {Source} {Target} --startup-project "ProxyServiceTests.Proxy.PTH.Api" --project "ProxyServiceTests.Proxy.PTH.Infrastructure"
```

### Upgrade/downgrade the schema to a specific version (CLI)

```powershell
dotnet ef database update {Target} --startup-project "ProxyServiceTests.Proxy.PTH.Api" --project "ProxyServiceTests.Proxy.PTH.Infrastructure"
```

### Remove the last created migration (CLI)

```powershell
dotnet ef migrations remove --startup-project "ProxyServiceTests.Proxy.PTH.Api" --project "ProxyServiceTests.Proxy.PTH.Infrastructure"
```

## Drop all tables in a schema

```sql
DECLARE @SCHEMA AS varchar(max) = 'ProxyServiceTests.Proxy.PassThruHeader'
DECLARE @EXECUTE_STATEMENT AS varchar(max) = (SELECT STUFF((SELECT CHAR(13) + CHAR(10) + [Statement] FROM (
    SELECT 'ALTER TABLE ['+@SCHEMA+'].['+[t].[name]+'] DROP CONSTRAINT ['+[fk].[name]+']' AS [Statement] FROM [sys].[foreign_keys] AS [fk] INNER JOIN [sys].[tables] AS [t] ON [t].[object_id] = [fk].[parent_object_id] INNER JOIN [sys].[schemas] AS [s] ON [s].[schema_id] = [t].[schema_id] WHERE [s].[name] = @SCHEMA
    UNION ALL
    SELECT 'DROP TABLE ['+@SCHEMA+'].['+[t].[name]+']' AS [Statement] FROM [sys].[tables] AS [t] INNER JOIN [sys].[schemas] AS [s] ON [s].[schema_id] = [t].[schema_id] WHERE [s].[name] = @SCHEMA
) A FOR XML PATH(''), TYPE).value('.', 'NVARCHAR(MAX)'), 1, 1, ''))
EXECUTE(@EXECUTE_STATEMENT)
```
