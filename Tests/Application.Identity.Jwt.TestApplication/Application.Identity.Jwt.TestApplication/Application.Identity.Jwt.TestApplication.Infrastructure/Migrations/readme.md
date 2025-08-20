# Entity Framework Core migrations readme

See <https://learn.microsoft.com//ef/core/managing-schemas/migrations> for information about migrations using Entity Framework Core.

You can perform these commands in the Visual Studio IDE using the Package Manager Console (View > Other Windows > Package Manager Console) or using the dotnet Command Line Interface (CLI) instructions.

Substitute the curly brace (`{}`) enclosed arguments below with the appropriate migration name when executing these commands.

## Visual Studio Package Manager Console quick reference

### Create a new migration (PMC)

```powershell
Add-Migration -Name {ChangeName} -StartupProject "Application.Identity.Jwt.TestApplication.Api" -Project "Application.Identity.Jwt.TestApplication.Infrastructure"
```

### Update the schema to the latest version (PMC)

```powershell
Update-Database -StartupProject "Application.Identity.Jwt.TestApplication.Api" -Project "Application.Identity.Jwt.TestApplication.Infrastructure"
```

## .NET CLI quick reference

### Create a new migration (CLI)

```powershell
dotnet ef migrations add {ChangeName} --startup-project "Application.Identity.Jwt.TestApplication.Api" --project "Application.Identity.Jwt.TestApplication.Infrastructure"
```

### Update the schema to the latest version (CLI)

```powershell
dotnet ef database update --startup-project "Application.Identity.Jwt.TestApplication.Api" --project "Application.Identity.Jwt.TestApplication.Infrastructure"
```

## Visual Studio Package Manager Console additional commands

### Generate a script which detects the current database schema version and updates it to the latest (PMC)

```powershell
Script-Migration -Idempotent -StartupProject "Application.Identity.Jwt.TestApplication.Api" -Project "Application.Identity.Jwt.TestApplication.Infrastructure"
```

### Generate a script which upgrades from and to a specific schema version (PMC)

```powershell
Script-Migration {Source} {Target} -StartupProject "Application.Identity.Jwt.TestApplication.Api" -Project "Application.Identity.Jwt.TestApplication.Infrastructure"
```

### Upgrade/downgrade the schema to a specific version (PMC)

```powershell
Update-Database -Migration {Target} -StartupProject "Application.Identity.Jwt.TestApplication.Api" -Project "Application.Identity.Jwt.TestApplication.Infrastructure"
```

### Remove the last created migration (PMC)

```powershell
Remove-Migration -StartupProject "Application.Identity.Jwt.TestApplication.Api" -Project "Application.Identity.Jwt.TestApplication.Infrastructure"
```

## .NET CLI additional commands

### Generate a script which detects the current database schema version and updates it to the latest (CLI)

```powershell
dotnet ef migrations script --idempotent --startup-project "Application.Identity.Jwt.TestApplication.Api" --project "Application.Identity.Jwt.TestApplication.Infrastructure"
```

### Generate a script which upgrades from and to a specific schema version (CLI)

```powershell
dotnet ef migrations script {Source} {Target} --startup-project "Application.Identity.Jwt.TestApplication.Api" --project "Application.Identity.Jwt.TestApplication.Infrastructure"
```

### Upgrade/downgrade the schema to a specific version (CLI)

```powershell
dotnet ef database update {Target} --startup-project "Application.Identity.Jwt.TestApplication.Api" --project "Application.Identity.Jwt.TestApplication.Infrastructure"
```

### Remove the last created migration (CLI)

```powershell
dotnet ef migrations remove --startup-project "Application.Identity.Jwt.TestApplication.Api" --project "Application.Identity.Jwt.TestApplication.Infrastructure"
```

## Drop all tables in a schema

```sql
DECLARE @SCHEMA AS varchar(max) = 'Application.Identity.Jwt.TestApplication'
DECLARE @EXECUTE_STATEMENT AS varchar(max) = (SELECT STUFF((SELECT CHAR(13) + CHAR(10) + [Statement] FROM (
    SELECT 'ALTER TABLE ['+@SCHEMA+'].['+[t].[name]+'] DROP CONSTRAINT ['+[fk].[name]+']' AS [Statement] FROM [sys].[foreign_keys] AS [fk] INNER JOIN [sys].[tables] AS [t] ON [t].[object_id] = [fk].[parent_object_id] INNER JOIN [sys].[schemas] AS [s] ON [s].[schema_id] = [t].[schema_id] WHERE [s].[name] = @SCHEMA
    UNION ALL
    SELECT 'DROP TABLE ['+@SCHEMA+'].['+[t].[name]+']' AS [Statement] FROM [sys].[tables] AS [t] INNER JOIN [sys].[schemas] AS [s] ON [s].[schema_id] = [t].[schema_id] WHERE [s].[name] = @SCHEMA
) A FOR XML PATH(''), TYPE).value('.', 'NVARCHAR(MAX)'), 1, 1, ''))
EXECUTE(@EXECUTE_STATEMENT)
```
