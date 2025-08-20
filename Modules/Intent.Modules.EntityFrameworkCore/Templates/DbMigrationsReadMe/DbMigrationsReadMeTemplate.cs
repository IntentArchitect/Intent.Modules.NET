using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.FileTemplateStringInterpolation", Version = "1.0")]

namespace Intent.Modules.EntityFrameworkCore.Templates.DbMigrationsReadMe
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public partial class DbMigrationsReadMeTemplate
    {
        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public override string TransformText()
        {
            var sb = new StringBuilder();

            sb.AppendLine(
                """
                # Entity Framework Core migrations readme
                
                See <https://learn.microsoft.com//ef/core/managing-schemas/migrations> for information about migrations using Entity Framework Core.
                
                You can perform these commands in the Visual Studio IDE using the Package Manager Console (View > Other Windows > Package Manager Console) or using the dotnet Command Line Interface (CLI) instructions.
                
                Substitute the curly brace (`{}`) enclosed arguments below with the appropriate migration name when executing these commands.
                """);

            if (!PackageManagerConsoleSupported)
            {
                sb.AppendLine(
                    """
                    
                    > NOTE: When using `DesignTimeDbContextFactory`, it is recommended to use the `dotnet ef` CLI
                    > commands as the Visual Studio Package Manager Console may encounter issues in this scenario.);
                    """);
            }

            if (!string.IsNullOrWhiteSpace(ExtraComments))
            {
                sb.AppendLine(
                    $"""
                    
                    {ExtraComments}
                    """);
            }

            var quickReference = new List<Command>
            {
                new(
                    Heading: "Create a new migration",
                    PmcCommand: $$"""Add-Migration -Name {ChangeName} {{GetVsStartupProjectArgument()}}-Project "{{MigrationProject}}"{{GetVsDbContextArgument()}}{{GetExtraArguments()}}""",
                    CliCommand: $$"""dotnet ef migrations add {ChangeName} {{GetCliStartupProjectArgument()}}--project "{{MigrationProject}}"{{GetCliDbContextArgument()}}{{GetExtraArguments()}}"""),

                new(
                    Heading: "Update the schema to the latest version",
                    PmcCommand: $$"""Update-Database {{GetVsStartupProjectArgument()}}-Project "{{MigrationProject}}"{{GetVsDbContextArgument()}}{{GetExtraArguments()}}""",
                    CliCommand: $$"""dotnet ef database update {{GetCliStartupProjectArgument()}}--project "{{MigrationProject}}"{{GetCliDbContextArgument()}}{{GetExtraArguments()}}"""),
            };

            var other = new List<Command>()
            {
                new(
                    Heading: "Generate a script which detects the current database schema version and updates it to the latest",
                    PmcCommand: $$"""Script-Migration -Idempotent {{GetVsStartupProjectArgument()}}-Project "{{MigrationProject}}"{{GetVsDbContextArgument()}}{{GetExtraArguments()}}""",
                    CliCommand: $$"""dotnet ef migrations script --idempotent {{GetCliStartupProjectArgument()}}--project "{{MigrationProject}}"{{GetCliDbContextArgument()}}{{GetExtraArguments()}}"""),
                new(
                    Heading: "Generate a script which upgrades from and to a specific schema version",
                    PmcCommand: $$"""Script-Migration {Source} {Target} {{GetVsStartupProjectArgument()}}-Project "{{MigrationProject}}"{{GetVsDbContextArgument()}}{{GetExtraArguments()}}""",
                    CliCommand: $$"""dotnet ef migrations script {Source} {Target} {{GetCliStartupProjectArgument()}}--project "{{MigrationProject}}"{{GetCliDbContextArgument()}}{{GetExtraArguments()}}"""),
                new(
                    Heading: "Upgrade/downgrade the schema to a specific version",
                    PmcCommand: $$"""Update-Database -Migration {Target} {{GetVsStartupProjectArgument()}}-Project "{{MigrationProject}}"{{GetVsDbContextArgument()}}{{GetExtraArguments()}}""",
                    CliCommand: $$"""dotnet ef database update {Target} {{GetCliStartupProjectArgument()}}--project "{{MigrationProject}}"{{GetCliDbContextArgument()}}{{GetExtraArguments()}}"""),
                new(
                    Heading: "Remove the last created migration",
                    PmcCommand: $$"""Remove-Migration {{GetVsStartupProjectArgument()}}-Project "{{MigrationProject}}"{{GetVsDbContextArgument()}}{{GetExtraArguments()}}""",
                    CliCommand: $$"""dotnet ef migrations remove {{GetCliStartupProjectArgument()}}--project "{{MigrationProject}}"{{GetCliDbContextArgument()}}{{GetExtraArguments()}}"""),
            };

            sb.AppendLine(
                """
                
                ## Visual Studio Package Manager Console quick reference
                """);

            foreach (var command in quickReference)
            {
                AppendCommand(sb, command, CommandType.Pmc);
            }

            sb.AppendLine(
                """

                ## .NET CLI quick reference
                """);

            foreach (var command in quickReference)
            {
                AppendCommand(sb, command, CommandType.Cli);
            }

            sb.AppendLine(
                """

                ## Visual Studio Package Manager Console additional commands
                """);

            foreach (var command in other)
            {
                AppendCommand(sb, command, CommandType.Pmc);
            }

            sb.AppendLine(
                """

                ## .NET CLI additional commands
                """);

            foreach (var command in other)
            {
                AppendCommand(sb, command, CommandType.Cli);
            }

            sb.AppendLine(
                $"""
                
                ## Drop all tables in a schema
                
                ```sql
                DECLARE @SCHEMA AS varchar(max) = '{BoundedContextName}'
                DECLARE @EXECUTE_STATEMENT AS varchar(max) = (SELECT STUFF((SELECT CHAR(13) + CHAR(10) + [Statement] FROM (
                    SELECT 'ALTER TABLE ['+@SCHEMA+'].['+[t].[name]+'] DROP CONSTRAINT ['+[fk].[name]+']' AS [Statement] FROM [sys].[foreign_keys] AS [fk] INNER JOIN [sys].[tables] AS [t] ON [t].[object_id] = [fk].[parent_object_id] INNER JOIN [sys].[schemas] AS [s] ON [s].[schema_id] = [t].[schema_id] WHERE [s].[name] = @SCHEMA
                    UNION ALL
                    SELECT 'DROP TABLE ['+@SCHEMA+'].['+[t].[name]+']' AS [Statement] FROM [sys].[tables] AS [t] INNER JOIN [sys].[schemas] AS [s] ON [s].[schema_id] = [t].[schema_id] WHERE [s].[name] = @SCHEMA
                ) A FOR XML PATH(''), TYPE).value('.', 'NVARCHAR(MAX)'), 1, 1, ''))
                EXECUTE(@EXECUTE_STATEMENT)
                ```
                """);

            return sb.ToString();

            static void AppendCommand(StringBuilder sb, Command command, CommandType type)
            {
                var (cmd, bracketText) = type switch
                {
                    CommandType.Cli => (command.CliCommand, "CLI"),
                    CommandType.Pmc => (command.PmcCommand, "PMC"),
                    _ => throw new ArgumentOutOfRangeException(nameof(type))
                };

                sb.AppendLine(
                    $"""

                     ### {command.Heading} ({bracketText})

                     ```powershell
                     {cmd}
                     ```
                     """);
            }
        }

        private enum CommandType { Pmc, Cli }

        private record Command(string Heading, string PmcCommand, string CliCommand);
    }
}