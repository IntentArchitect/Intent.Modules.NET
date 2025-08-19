# Entity Framework Core migrations readme

See <https://learn.microsoft.com//ef/core/managing-schemas/migrations> for information about migrations using Entity Framework Core.

You can perform these commands in the Visual Studio IDE using the Package Manager Console (View > Other Windows > Package Manager Console) or using the dotnet Command Line Interface (CLI) instructions.

Substitute the curly brace (`{}`) enclosed arguments below with the appropriate migration name when executing these commands.

## Visual Studio Package Manager Console quick reference

```powershell
# Create a new migration
Add-Migration -Name {ChangeName} -StartupProject "AspNetCoreMvc.Api" -Project "AspNetCoreMvc.Infrastructure"

# Update the schema to the latest version
Update-Database -StartupProject "AspNetCoreMvc.Api" -Project "AspNetCoreMvc.Infrastructure"

# Generate a script which detects the current database schema version and updates it to the latest
Script-Migration -Idempotent -StartupProject "AspNetCoreMvc.Api" -Project "AspNetCoreMvc.Infrastructure"

# Generate a script which upgrades from and to a specific schema version
Script-Migration {Source} {Target} -StartupProject "AspNetCoreMvc.Api" -Project "AspNetCoreMvc.Infrastructure"

# Upgrade/downgrade the schema to a specific version
Update-Database -Migration {Target} -StartupProject "AspNetCoreMvc.Api" -Project "AspNetCoreMvc.Infrastructure"

# Remove the last created migration
Remove-Migration -StartupProject "AspNetCoreMvc.Api" -Project "AspNetCoreMvc.Infrastructure"
```

## .NET CLI quick reference

```powershell
# Create a new migration
dotnet ef migrations add {ChangeName} --startup-project "AspNetCoreMvc.Api" --project "AspNetCoreMvc.Infrastructure"

# Update the schema to the latest version
dotnet ef database update --startup-project "AspNetCoreMvc.Api" --project "AspNetCoreMvc.Infrastructure"

# Generate a script which detects the current database schema version and updates it to the latest
dotnet ef migrations script --idempotent --startup-project "AspNetCoreMvc.Api" --project "AspNetCoreMvc.Infrastructure"

# Generate a script which upgrades from and to a specific schema version
dotnet ef migrations script {Source} {Target} --startup-project "AspNetCoreMvc.Api" --project "AspNetCoreMvc.Infrastructure"

# Upgrade/downgrade the schema to a specific version
dotnet ef database update {Target} --startup-project "AspNetCoreMvc.Api" --project "AspNetCoreMvc.Infrastructure"

# Remove the last created migration
dotnet ef migrations remove --startup-project "AspNetCoreMvc.Api" --project "AspNetCoreMvc.Infrastructure"
```

## Drop all tables in a schema

```sql
DECLARE @SCHEMA AS varchar(max) = 'AspNetCoreMvc'
DECLARE @EXECUTE_STATEMENT AS varchar(max) = (SELECT STUFF((SELECT CHAR(13) + CHAR(10) + [Statement] FROM (
    SELECT 'ALTER TABLE ['+@SCHEMA+'].['+[t].[name]+'] DROP CONSTRAINT ['+[fk].[name]+']' AS [Statement] FROM [sys].[foreign_keys] AS [fk] INNER JOIN [sys].[tables] AS [t] ON [t].[object_id] = [fk].[parent_object_id] INNER JOIN [sys].[schemas] AS [s] ON [s].[schema_id] = [t].[schema_id] WHERE [s].[name] = @SCHEMA
    UNION ALL
    SELECT 'DROP TABLE ['+@SCHEMA+'].['+[t].[name]+']' AS [Statement] FROM [sys].[tables] AS [t] INNER JOIN [sys].[schemas] AS [s] ON [s].[schema_id] = [t].[schema_id] WHERE [s].[name] = @SCHEMA
) A FOR XML PATH(''), TYPE).value('.', 'NVARCHAR(MAX)'), 1, 1, ''))
EXECUTE(@EXECUTE_STATEMENT)
```
