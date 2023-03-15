// ------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version: 17.0.0.0
//  
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
// ------------------------------------------------------------------------------
namespace Intent.Modules.EntityFrameworkCore.DesignTimeDbContextFactory.Templates.DesignTimeDbMigrationsReadMe
{
    using Intent.Modules.Common.Templates;
    using System;
    
    /// <summary>
    /// Class to produce the template output
    /// </summary>
    
    #line 1 "C:\Dev\Intent.Modules.NET\Modules\Intent.Modules.EntityFrameworkCore.DesignTimeDbContextFactory\Templates\DesignTimeDbMigrationsReadMe\DesignTimeDbMigrationsReadMeTemplate.tt"
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.VisualStudio.TextTemplating", "17.0.0.0")]
    public partial class DesignTimeDbMigrationsReadMeTemplate : IntentTemplateBase<object>
    {
#line hidden
        /// <summary>
        /// Create the template output
        /// </summary>
        public override string TransformText()
        {
            this.Write(@"Read here about Migrations using EF Core: https://learn.microsoft.com/en-us/ef/core/managing-schemas/migrations
You can perform these commands in Visual Studio IDE (VS) using the Package Manager Console (View > Other Windows > Package Manager Console)
or using the dotnet Command Line Interface (CLI) instructions.
Substitute the {Keywords} below with the appropriate migration name when executing these commands.

This file has been created in addition to the MIGRATION_README.txt for allowing Design Time DB Contexts
to be used instead of the original one. This allows for migration operations to be performed without
making use of the Hosting application's Startup procedure.
A separate ""appsettings.json"" file is used in this project for managing connection strings.

Create a new migration:
-------------------------------------------------------------------------------------------------------------------------------------------------------
VS:  Add-Migration -Name {ChangeName} -Project """);
            
            #line 16 "C:\Dev\Intent.Modules.NET\Modules\Intent.Modules.EntityFrameworkCore.DesignTimeDbContextFactory\Templates\DesignTimeDbMigrationsReadMe\DesignTimeDbMigrationsReadMeTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(MigrationProject));
            
            #line default
            #line hidden
            this.Write("\" -- {ConnectionStringName}\r\nCLI: dotnet ef migrations add {ChangeName} --project" +
                    " \"");
            
            #line 17 "C:\Dev\Intent.Modules.NET\Modules\Intent.Modules.EntityFrameworkCore.DesignTimeDbContextFactory\Templates\DesignTimeDbMigrationsReadMe\DesignTimeDbMigrationsReadMeTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(MigrationProject));
            
            #line default
            #line hidden
            this.Write("\" -- {ConnectionStringName}\r\n\r\nRemove last migration:\r\n--------------------------" +
                    "--------------------------------------------------------------------------------" +
                    "---------------------------------------------\r\nVS:  Remove-Migration  -Project \"" +
                    "");
            
            #line 21 "C:\Dev\Intent.Modules.NET\Modules\Intent.Modules.EntityFrameworkCore.DesignTimeDbContextFactory\Templates\DesignTimeDbMigrationsReadMe\DesignTimeDbMigrationsReadMeTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(MigrationProject));
            
            #line default
            #line hidden
            this.Write("\" -- {ConnectionStringName}\r\nCLI: dotnet ef migrations remove --project \"");
            
            #line 22 "C:\Dev\Intent.Modules.NET\Modules\Intent.Modules.EntityFrameworkCore.DesignTimeDbContextFactory\Templates\DesignTimeDbMigrationsReadMe\DesignTimeDbMigrationsReadMeTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(MigrationProject));
            
            #line default
            #line hidden
            this.Write("\" -- {ConnectionStringName}\r\n\r\nUpdate schema to the latest version:\r\n------------" +
                    "--------------------------------------------------------------------------------" +
                    "-----------------------------------------------------------\r\nVS:  Update-Databas" +
                    "e -Project \"");
            
            #line 26 "C:\Dev\Intent.Modules.NET\Modules\Intent.Modules.EntityFrameworkCore.DesignTimeDbContextFactory\Templates\DesignTimeDbMigrationsReadMe\DesignTimeDbMigrationsReadMeTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(MigrationProject));
            
            #line default
            #line hidden
            this.Write("\" -- {ConnectionStringName}\r\nCLI: dotnet ef database update --project \"");
            
            #line 27 "C:\Dev\Intent.Modules.NET\Modules\Intent.Modules.EntityFrameworkCore.DesignTimeDbContextFactory\Templates\DesignTimeDbMigrationsReadMe\DesignTimeDbMigrationsReadMeTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(MigrationProject));
            
            #line default
            #line hidden
            this.Write(@"""  -- {ConnectionStringName}

Upgrade/downgrade schema to specific version:
-------------------------------------------------------------------------------------------------------------------------------------------------------
VS:  Update-Database -Migration {Target} -Project """);
            
            #line 31 "C:\Dev\Intent.Modules.NET\Modules\Intent.Modules.EntityFrameworkCore.DesignTimeDbContextFactory\Templates\DesignTimeDbMigrationsReadMe\DesignTimeDbMigrationsReadMeTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(MigrationProject));
            
            #line default
            #line hidden
            this.Write("\" -- {ConnectionStringName}\r\nCLI: dotnet ef database update {Target} --project \"");
            
            #line 32 "C:\Dev\Intent.Modules.NET\Modules\Intent.Modules.EntityFrameworkCore.DesignTimeDbContextFactory\Templates\DesignTimeDbMigrationsReadMe\DesignTimeDbMigrationsReadMeTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(MigrationProject));
            
            #line default
            #line hidden
            this.Write(@""" -- {ConnectionStringName}

Generate a script which detects the current database schema version and updates it to the latest:
-------------------------------------------------------------------------------------------------------------------------------------------------------
VS:  Script-Migration -Project """);
            
            #line 36 "C:\Dev\Intent.Modules.NET\Modules\Intent.Modules.EntityFrameworkCore.DesignTimeDbContextFactory\Templates\DesignTimeDbMigrationsReadMe\DesignTimeDbMigrationsReadMeTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(MigrationProject));
            
            #line default
            #line hidden
            this.Write("\" -- {ConnectionStringName}\r\nCLI: dotnet ef migrations script --project \"");
            
            #line 37 "C:\Dev\Intent.Modules.NET\Modules\Intent.Modules.EntityFrameworkCore.DesignTimeDbContextFactory\Templates\DesignTimeDbMigrationsReadMe\DesignTimeDbMigrationsReadMeTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(MigrationProject));
            
            #line default
            #line hidden
            this.Write(@""" -- {ConnectionStringName}

Generate a script which upgrades from and to a specific schema version:
-------------------------------------------------------------------------------------------------------------------------------------------------------
VS:  Script-Migration {Source} {Target} -Project """);
            
            #line 41 "C:\Dev\Intent.Modules.NET\Modules\Intent.Modules.EntityFrameworkCore.DesignTimeDbContextFactory\Templates\DesignTimeDbMigrationsReadMe\DesignTimeDbMigrationsReadMeTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(MigrationProject));
            
            #line default
            #line hidden
            this.Write("\" -- {ConnectionStringName}\r\nCLI: dotnet ef migrations script {Source} {Target} -" +
                    "-project \"");
            
            #line 42 "C:\Dev\Intent.Modules.NET\Modules\Intent.Modules.EntityFrameworkCore.DesignTimeDbContextFactory\Templates\DesignTimeDbMigrationsReadMe\DesignTimeDbMigrationsReadMeTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(MigrationProject));
            
            #line default
            #line hidden
            this.Write("\" -- {ConnectionStringName}\r\n\r\nDrop all tables in schema:\r\n----------------------" +
                    "--------------------------------------------------------------------------------" +
                    "-------------------------------------------------\r\nDECLARE @SCHEMA AS varchar(ma" +
                    "x) = \'");
            
            #line 46 "C:\Dev\Intent.Modules.NET\Modules\Intent.Modules.EntityFrameworkCore.DesignTimeDbContextFactory\Templates\DesignTimeDbMigrationsReadMe\DesignTimeDbMigrationsReadMeTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(BoundedContextName));
            
            #line default
            #line hidden
            this.Write(@"'
DECLARE @EXECUTE_STATEMENT AS varchar(max) = (SELECT STUFF((SELECT CHAR(13) + CHAR(10) + [Statement] FROM (
    SELECT 'ALTER TABLE ['+@SCHEMA+'].['+[t].[name]+'] DROP CONSTRAINT ['+[fk].[name]+']' AS [Statement] FROM [sys].[foreign_keys] AS [fk] INNER JOIN [sys].[tables] AS [t] ON [t].[object_id] = [fk].[parent_object_id] INNER JOIN [sys].[schemas] AS [s] ON [s].[schema_id] = [t].[schema_id] WHERE [s].[name] = @SCHEMA
    UNION ALL
    SELECT 'DROP TABLE ['+@SCHEMA+'].['+[t].[name]+']' AS [Statement] FROM [sys].[tables] AS [t] INNER JOIN [sys].[schemas] AS [s] ON [s].[schema_id] = [t].[schema_id] WHERE [s].[name] = @SCHEMA
) A FOR XML PATH(''), TYPE).value('.', 'NVARCHAR(MAX)'), 1, 1, ''))
EXECUTE(@EXECUTE_STATEMENT)
");
            return this.GenerationEnvironment.ToString();
        }
    }
    
    #line default
    #line hidden
}
