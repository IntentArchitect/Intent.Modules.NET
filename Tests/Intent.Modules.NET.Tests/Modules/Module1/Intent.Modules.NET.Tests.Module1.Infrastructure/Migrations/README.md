# Entity Framework Core migrations readme

See <https://learn.microsoft.com/ef/core/managing-schemas/migrations> for information about migrations
using Entity Framework Core.

You can perform these commands in the Visual Studio IDE using the Package Manager Console (VS PMC), which can
be opened through the `View` > `Other Windows` > `Package Manager Console` menu option, or using the .NET
Command Line Interface (.NET CLI) tools.

Substitute the curly brace (`{}`) enclosed arguments below with the appropriate migration name when
executing these commands.

## Visual Studio Package Manager Console quick reference

### Create a new migration (VS PMC)

```powershell
Add-Migration -Name {ChangeName} -StartupProject "Intent.Modules.NET.Tests.Host" -Project "Intent.Modules.NET.Tests.Module1.Infrastructure"
```

### Update the schema to the latest version (VS PMC)

```powershell
Update-Database -StartupProject "Intent.Modules.NET.Tests.Host" -Project "Intent.Modules.NET.Tests.Module1.Infrastructure"
```

## .NET CLI quick reference

### Create a new migration (.NET CLI)

```powershell
dotnet ef migrations add {ChangeName} --startup-project "Intent.Modules.NET.Tests.Host" --project "Intent.Modules.NET.Tests.Module1.Infrastructure"
```

### Update the schema to the latest version (.NET CLI)

```powershell
dotnet ef database update --startup-project "Intent.Modules.NET.Tests.Host" --project "Intent.Modules.NET.Tests.Module1.Infrastructure"
```

## Visual Studio Package Manager Console additional commands

### Generate a script which detects the current database schema version and updates it to the latest (VS PMC)

```powershell
Script-Migration -Idempotent -StartupProject "Intent.Modules.NET.Tests.Host" -Project "Intent.Modules.NET.Tests.Module1.Infrastructure"
```

### Generate a script which upgrades from and to a specific schema version (VS PMC)

```powershell
Script-Migration {Source} {Target} -StartupProject "Intent.Modules.NET.Tests.Host" -Project "Intent.Modules.NET.Tests.Module1.Infrastructure"
```

### Upgrade/downgrade the schema to a specific version (VS PMC)

```powershell
Update-Database -Migration {Target} -StartupProject "Intent.Modules.NET.Tests.Host" -Project "Intent.Modules.NET.Tests.Module1.Infrastructure"
```

### Remove the last created migration (VS PMC)

```powershell
Remove-Migration -StartupProject "Intent.Modules.NET.Tests.Host" -Project "Intent.Modules.NET.Tests.Module1.Infrastructure"
```

## .NET CLI additional commands

### Generate a script which detects the current database schema version and updates it to the latest (.NET CLI)

```powershell
dotnet ef migrations script --idempotent --startup-project "Intent.Modules.NET.Tests.Host" --project "Intent.Modules.NET.Tests.Module1.Infrastructure"
```

### Generate a script which upgrades from and to a specific schema version (.NET CLI)

```powershell
dotnet ef migrations script {Source} {Target} --startup-project "Intent.Modules.NET.Tests.Host" --project "Intent.Modules.NET.Tests.Module1.Infrastructure"
```

### Upgrade/downgrade the schema to a specific version (.NET CLI)

```powershell
dotnet ef database update {Target} --startup-project "Intent.Modules.NET.Tests.Host" --project "Intent.Modules.NET.Tests.Module1.Infrastructure"
```

### Remove the last created migration (.NET CLI)

```powershell
dotnet ef migrations remove --startup-project "Intent.Modules.NET.Tests.Host" --project "Intent.Modules.NET.Tests.Module1.Infrastructure"
```

## Drop all tables in a schema

```sql
DECLARE @SCHEMA AS varchar(max) = 'Module1'
DECLARE @EXECUTE_STATEMENT AS varchar(max) = (SELECT STUFF((SELECT CHAR(13) + CHAR(10) + [Statement] FROM (
    SELECT 'ALTER TABLE ['+@SCHEMA+'].['+[t].[name]+'] DROP CONSTRAINT ['+[fk].[name]+']' AS [Statement] FROM [sys].[foreign_keys] AS [fk] INNER JOIN [sys].[tables] AS [t] ON [t].[object_id] = [fk].[parent_object_id] INNER JOIN [sys].[schemas] AS [s] ON [s].[schema_id] = [t].[schema_id] WHERE [s].[name] = @SCHEMA
    UNION ALL
    SELECT 'DROP TABLE ['+@SCHEMA+'].['+[t].[name]+']' AS [Statement] FROM [sys].[tables] AS [t] INNER JOIN [sys].[schemas] AS [s] ON [s].[schema_id] = [t].[schema_id] WHERE [s].[name] = @SCHEMA
) A FOR XML PATH(''), TYPE).value('.', 'NVARCHAR(MAX)'), 1, 1, ''))
EXECUTE(@EXECUTE_STATEMENT)
```
