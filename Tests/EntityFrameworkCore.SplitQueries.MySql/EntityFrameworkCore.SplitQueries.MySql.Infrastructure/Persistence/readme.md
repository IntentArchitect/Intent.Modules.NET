# Entity Framework Core migrations readme

See <https://learn.microsoft.com//ef/core/managing-schemas/migrations> for information about migrations using Entity Framework Core.

You can perform these commands in the Visual Studio IDE using the Package Manager Console (View > Other Windows > Package Manager Console) or using the dotnet Command Line Interface (CLI) instructions.

Substitute the curly brace (`{}`) enclosed arguments below with the appropriate migration name when executing these commands.

## Visual Studio Package Manager Console quick reference

```powershell
# Create a new migration
Add-Migration -Name {ChangeName} -StartupProject "EntityFrameworkCore.SplitQueries.MySql.Api" -Project "EntityFrameworkCore.SplitQueries.MySql.Infrastructure"

# Update the schema to the latest version
Update-Database -StartupProject "EntityFrameworkCore.SplitQueries.MySql.Api" -Project "EntityFrameworkCore.SplitQueries.MySql.Infrastructure"

# Generate a script which detects the current database schema version and updates it to the latest
Script-Migration -Idempotent -StartupProject "EntityFrameworkCore.SplitQueries.MySql.Api" -Project "EntityFrameworkCore.SplitQueries.MySql.Infrastructure"

# Generate a script which upgrades from and to a specific schema version
Script-Migration {Source} {Target} -StartupProject "EntityFrameworkCore.SplitQueries.MySql.Api" -Project "EntityFrameworkCore.SplitQueries.MySql.Infrastructure"

# Upgrade/downgrade the schema to a specific version
Update-Database -Migration {Target} -StartupProject "EntityFrameworkCore.SplitQueries.MySql.Api" -Project "EntityFrameworkCore.SplitQueries.MySql.Infrastructure"

# Remove the last created migration
Remove-Migration -StartupProject "EntityFrameworkCore.SplitQueries.MySql.Api" -Project "EntityFrameworkCore.SplitQueries.MySql.Infrastructure"
```

## .NET CLI quick reference

```powershell
# Create a new migration
dotnet ef migrations add {ChangeName} --startup-project "EntityFrameworkCore.SplitQueries.MySql.Api" --project "EntityFrameworkCore.SplitQueries.MySql.Infrastructure"

# Update the schema to the latest version
dotnet ef database update --startup-project "EntityFrameworkCore.SplitQueries.MySql.Api" --project "EntityFrameworkCore.SplitQueries.MySql.Infrastructure"

# Generate a script which detects the current database schema version and updates it to the latest
dotnet ef migrations script --idempotent --startup-project "EntityFrameworkCore.SplitQueries.MySql.Api" --project "EntityFrameworkCore.SplitQueries.MySql.Infrastructure"

# Generate a script which upgrades from and to a specific schema version
dotnet ef migrations script {Source} {Target} --startup-project "EntityFrameworkCore.SplitQueries.MySql.Api" --project "EntityFrameworkCore.SplitQueries.MySql.Infrastructure"

# Upgrade/downgrade the schema to a specific version
dotnet ef database update {Target} --startup-project "EntityFrameworkCore.SplitQueries.MySql.Api" --project "EntityFrameworkCore.SplitQueries.MySql.Infrastructure"

# Remove the last created migration
dotnet ef migrations remove --startup-project "EntityFrameworkCore.SplitQueries.MySql.Api" --project "EntityFrameworkCore.SplitQueries.MySql.Infrastructure"
```

## Drop all tables in a schema

```sql
DECLARE @SCHEMA AS varchar(max) = 'EntityFrameworkCore.SplitQueries.MySql'
DECLARE @EXECUTE_STATEMENT AS varchar(max) = (SELECT STUFF((SELECT CHAR(13) + CHAR(10) + [Statement] FROM (
    SELECT 'ALTER TABLE ['+@SCHEMA+'].['+[t].[name]+'] DROP CONSTRAINT ['+[fk].[name]+']' AS [Statement] FROM [sys].[foreign_keys] AS [fk] INNER JOIN [sys].[tables] AS [t] ON [t].[object_id] = [fk].[parent_object_id] INNER JOIN [sys].[schemas] AS [s] ON [s].[schema_id] = [t].[schema_id] WHERE [s].[name] = @SCHEMA
    UNION ALL
    SELECT 'DROP TABLE ['+@SCHEMA+'].['+[t].[name]+']' AS [Statement] FROM [sys].[tables] AS [t] INNER JOIN [sys].[schemas] AS [s] ON [s].[schema_id] = [t].[schema_id] WHERE [s].[name] = @SCHEMA
) A FOR XML PATH(''), TYPE).value('.', 'NVARCHAR(MAX)'), 1, 1, ''))
EXECUTE(@EXECUTE_STATEMENT)
```
