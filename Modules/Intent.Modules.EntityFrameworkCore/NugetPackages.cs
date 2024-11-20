using System;
using Intent.Engine;
using Intent.Modules.Common.CSharp.Nuget;
using Intent.Modules.Common.VisualStudio;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.NugetPackages", Version = "1.0")]

namespace Intent.Modules.EntityFrameworkCore
{
    public class NugetPackages : INugetPackages
    {
        public const string ErikEJEntityFrameworkCoreSqlServerDateOnlyTimeOnlyPackageName = "ErikEJ.EntityFrameworkCore.SqlServer.DateOnlyTimeOnly";
        public const string MicrosoftEntityFrameworkCorePackageName = "Microsoft.EntityFrameworkCore";
        public const string MicrosoftEntityFrameworkCoreCosmosPackageName = "Microsoft.EntityFrameworkCore.Cosmos";
        public const string MicrosoftEntityFrameworkCoreDesignPackageName = "Microsoft.EntityFrameworkCore.Design";
        public const string MicrosoftEntityFrameworkCoreInMemoryPackageName = "Microsoft.EntityFrameworkCore.InMemory";
        public const string MicrosoftEntityFrameworkCoreProxiesPackageName = "Microsoft.EntityFrameworkCore.Proxies";
        public const string MicrosoftEntityFrameworkCoreSqlServerPackageName = "Microsoft.EntityFrameworkCore.SqlServer";
        public const string MicrosoftEntityFrameworkCoreSqlServerNetTopologySuitePackageName = "Microsoft.EntityFrameworkCore.SqlServer.NetTopologySuite";
        public const string MicrosoftEntityFrameworkCoreToolsPackageName = "Microsoft.EntityFrameworkCore.Tools";
        public const string NpgsqlEntityFrameworkCorePostgreSQLPackageName = "Npgsql.EntityFrameworkCore.PostgreSQL";
        public const string NpgsqlEntityFrameworkCorePostgreSQLNetTopologySuitePackageName = "Npgsql.EntityFrameworkCore.PostgreSQL.NetTopologySuite";
        public const string OracleEntityFrameworkCorePackageName = "Oracle.EntityFrameworkCore";
        public const string PomeloEntityFrameworkCoreMySqlPackageName = "Pomelo.EntityFrameworkCore.MySql";
        public const string PomeloEntityFrameworkCoreMySqlNetTopologySuitePackageName = "Pomelo.EntityFrameworkCore.MySql.NetTopologySuite";

        public void RegisterPackages()
        {
            NugetRegistry.Register(ErikEJEntityFrameworkCoreSqlServerDateOnlyTimeOnlyPackageName,
                (framework) => framework switch
                    {
                        ( >= 6, 0) => new PackageVersion("7.0.10"),
                        _ => throw new Exception($"Unsupported Framework `{framework.Major}` for NuGet package '{ErikEJEntityFrameworkCoreSqlServerDateOnlyTimeOnlyPackageName}'"),
                    }
                );
            NugetRegistry.Register(MicrosoftEntityFrameworkCorePackageName,
                (framework) => framework switch
                    {
                        ( >= 9, 0) => new PackageVersion("9.0.0", locked: true)
                            .WithNugetDependency("Microsoft.EntityFrameworkCore.Abstractions", "9.0.0")
                            .WithNugetDependency("Microsoft.EntityFrameworkCore.Analyzers", "9.0.0")
                            .WithNugetDependency("Microsoft.Extensions.Caching.Memory", "9.0.0")
                            .WithNugetDependency("Microsoft.Extensions.Logging", "9.0.0"),
                        ( >= 8, 0) => new PackageVersion("8.0.11", locked: true),
                        ( >= 6, 0) => new PackageVersion("7.0.20"),
                        ( >= 2, 1) => new PackageVersion("5.0.17")
                            .WithNugetDependency("Microsoft.EntityFrameworkCore.Abstractions", "5.0.17")
                            .WithNugetDependency("Microsoft.EntityFrameworkCore.Analyzers", "5.0.17")
                            .WithNugetDependency("Microsoft.Extensions.Caching.Memory", "5.0.0")
                            .WithNugetDependency("Microsoft.Extensions.DependencyInjection", "5.0.2")
                            .WithNugetDependency("Microsoft.Extensions.Logging", "5.0.0")
                            .WithNugetDependency("System.Collections.Immutable", "5.0.0")
                            .WithNugetDependency("System.ComponentModel.Annotations", "5.0.0")
                            .WithNugetDependency("System.Diagnostics.DiagnosticSource", "5.0.1"),
                        ( >= 2, 0) => new PackageVersion("3.1.32")
                            .WithNugetDependency("Microsoft.Bcl.AsyncInterfaces", "1.1.1")
                            .WithNugetDependency("Microsoft.Bcl.HashCode", "1.1.1")
                            .WithNugetDependency("Microsoft.EntityFrameworkCore.Abstractions", "3.1.32")
                            .WithNugetDependency("Microsoft.EntityFrameworkCore.Analyzers", "3.1.32")
                            .WithNugetDependency("Microsoft.Extensions.Caching.Memory", "3.1.32")
                            .WithNugetDependency("Microsoft.Extensions.DependencyInjection", "3.1.32")
                            .WithNugetDependency("Microsoft.Extensions.Logging", "3.1.32")
                            .WithNugetDependency("System.Collections.Immutable", "1.7.1")
                            .WithNugetDependency("System.ComponentModel.Annotations", "4.7.0")
                            .WithNugetDependency("System.Diagnostics.DiagnosticSource", "4.7.1"),
                        _ => throw new Exception($"Unsupported Framework `{framework.Major}` for NuGet package '{MicrosoftEntityFrameworkCorePackageName}'"),
                    }
                );
            NugetRegistry.Register(MicrosoftEntityFrameworkCoreCosmosPackageName,
                (framework) => framework switch
                    {
                        ( >= 9, 0) => new PackageVersion("9.0.0", locked: true)
                            .WithNugetDependency("Microsoft.Azure.Cosmos", "3.43.0")
                            .WithNugetDependency("Microsoft.EntityFrameworkCore", "9.0.0")
                            .WithNugetDependency("Microsoft.Extensions.Caching.Memory", "9.0.0")
                            .WithNugetDependency("Microsoft.Extensions.Logging", "9.0.0")
                            .WithNugetDependency("Newtonsoft.Json", "13.0.3")
                            .WithNugetDependency("System.Text.Json", "9.0.0"),
                        ( >= 8, 0) => new PackageVersion("8.0.11", locked: true),
                        ( >= 6, 0) => new PackageVersion("7.0.20"),
                        ( >= 2, 1) => new PackageVersion("5.0.17")
                            .WithNugetDependency("Microsoft.Azure.Cosmos", "3.12.0")
                            .WithNugetDependency("Microsoft.EntityFrameworkCore", "5.0.17"),
                        ( >= 2, 0) => new PackageVersion("3.1.32")
                            .WithNugetDependency("Microsoft.Azure.Cosmos", "3.3.3")
                            .WithNugetDependency("Microsoft.EntityFrameworkCore", "3.1.32"),
                        _ => throw new Exception($"Unsupported Framework `{framework.Major}` for NuGet package '{MicrosoftEntityFrameworkCoreCosmosPackageName}'"),
                    }
                );
            NugetRegistry.Register(MicrosoftEntityFrameworkCoreDesignPackageName,
                (framework) => framework switch
                    {
                        ( >= 9, 0) => new PackageVersion("9.0.0", locked: true)
                            .WithNugetDependency("Humanizer.Core", "2.14.1")
                            .WithNugetDependency("Microsoft.Build.Framework", "17.8.3")
                            .WithNugetDependency("Microsoft.Build.Locator", "1.7.8")
                            .WithNugetDependency("Microsoft.CodeAnalysis.CSharp", "4.8.0")
                            .WithNugetDependency("Microsoft.CodeAnalysis.CSharp.Workspaces", "4.8.0")
                            .WithNugetDependency("Microsoft.CodeAnalysis.Workspaces.MSBuild", "4.8.0")
                            .WithNugetDependency("Microsoft.EntityFrameworkCore.Relational", "9.0.0")
                            .WithNugetDependency("Microsoft.Extensions.Caching.Memory", "9.0.0")
                            .WithNugetDependency("Microsoft.Extensions.Configuration.Abstractions", "9.0.0")
                            .WithNugetDependency("Microsoft.Extensions.DependencyModel", "9.0.0")
                            .WithNugetDependency("Microsoft.Extensions.Logging", "9.0.0")
                            .WithNugetDependency("Mono.TextTemplating", "3.0.0")
                            .WithNugetDependency("System.Text.Json", "9.0.0"),
                        ( >= 8, 0) => new PackageVersion("8.0.11", locked: true),
                        ( >= 6, 0) => new PackageVersion("7.0.20"),
                        ( >= 2, 1) => new PackageVersion("5.0.17")
                            .WithNugetDependency("Humanizer.Core", "2.8.26")
                            .WithNugetDependency("Microsoft.CSharp", "4.7.0")
                            .WithNugetDependency("Microsoft.EntityFrameworkCore.Relational", "5.0.17"),
                        ( >= 2, 0) => new PackageVersion("3.1.32")
                            .WithNugetDependency("Microsoft.CSharp", "4.7.0")
                            .WithNugetDependency("Microsoft.EntityFrameworkCore.Relational", "3.1.32"),
                        _ => throw new Exception($"Unsupported Framework `{framework.Major}` for NuGet package '{MicrosoftEntityFrameworkCoreDesignPackageName}'"),
                    }
                );
            NugetRegistry.Register(MicrosoftEntityFrameworkCoreInMemoryPackageName,
                (framework) => framework switch
                    {
                        ( >= 9, 0) => new PackageVersion("9.0.0", locked: true)
                            .WithNugetDependency("Humanizer.Core", "2.14.1")
                            .WithNugetDependency("Microsoft.Build.Framework", "17.8.3")
                            .WithNugetDependency("Microsoft.Build.Locator", "1.7.8")
                            .WithNugetDependency("Microsoft.CodeAnalysis.CSharp", "4.8.0")
                            .WithNugetDependency("Microsoft.CodeAnalysis.CSharp.Workspaces", "4.8.0")
                            .WithNugetDependency("Microsoft.CodeAnalysis.Workspaces.MSBuild", "4.8.0")
                            .WithNugetDependency("Microsoft.EntityFrameworkCore.Relational", "9.0.0")
                            .WithNugetDependency("Microsoft.Extensions.Caching.Memory", "9.0.0")
                            .WithNugetDependency("Microsoft.Extensions.Configuration.Abstractions", "9.0.0")
                            .WithNugetDependency("Microsoft.Extensions.DependencyModel", "9.0.0")
                            .WithNugetDependency("Microsoft.Extensions.Logging", "9.0.0")
                            .WithNugetDependency("Mono.TextTemplating", "3.0.0")
                            .WithNugetDependency("System.Text.Json", "9.0.0"),
                        ( >= 8, 0) => new PackageVersion("8.0.11", locked: true),
                        ( >= 6, 0) => new PackageVersion("7.0.20"),
                        ( >= 2, 1) => new PackageVersion("5.0.17")
                            .WithNugetDependency("Microsoft.EntityFrameworkCore", "5.0.17"),
                        ( >= 2, 0) => new PackageVersion("3.1.32")
                            .WithNugetDependency("Microsoft.EntityFrameworkCore", "3.1.32"),
                        _ => throw new Exception($"Unsupported Framework `{framework.Major}` for NuGet package '{MicrosoftEntityFrameworkCoreInMemoryPackageName}'"),
                    }
                );
            NugetRegistry.Register(MicrosoftEntityFrameworkCoreProxiesPackageName,
                (framework) => framework switch
                    {
                        ( >= 9, 0) => new PackageVersion("9.0.0", locked: true)
                            .WithNugetDependency("Humanizer.Core", "2.14.1")
                            .WithNugetDependency("Microsoft.Build.Framework", "17.8.3")
                            .WithNugetDependency("Microsoft.Build.Locator", "1.7.8")
                            .WithNugetDependency("Microsoft.CodeAnalysis.CSharp", "4.8.0")
                            .WithNugetDependency("Microsoft.CodeAnalysis.CSharp.Workspaces", "4.8.0")
                            .WithNugetDependency("Microsoft.CodeAnalysis.Workspaces.MSBuild", "4.8.0")
                            .WithNugetDependency("Microsoft.EntityFrameworkCore.Relational", "9.0.0")
                            .WithNugetDependency("Microsoft.Extensions.Caching.Memory", "9.0.0")
                            .WithNugetDependency("Microsoft.Extensions.Configuration.Abstractions", "9.0.0")
                            .WithNugetDependency("Microsoft.Extensions.DependencyModel", "9.0.0")
                            .WithNugetDependency("Microsoft.Extensions.Logging", "9.0.0")
                            .WithNugetDependency("Mono.TextTemplating", "3.0.0")
                            .WithNugetDependency("System.Text.Json", "9.0.0"),
                        ( >= 8, 0) => new PackageVersion("8.0.11", locked: true),
                        ( >= 6, 0) => new PackageVersion("7.0.20"),
                        ( >= 2, 1) => new PackageVersion("5.0.17")
                            .WithNugetDependency("Castle.Core", "4.4.1")
                            .WithNugetDependency("Microsoft.EntityFrameworkCore", "5.0.17"),
                        ( >= 2, 0) => new PackageVersion("3.1.32")
                            .WithNugetDependency("Castle.Core", "4.4.0")
                            .WithNugetDependency("Microsoft.EntityFrameworkCore", "3.1.32"),
                        _ => throw new Exception($"Unsupported Framework `{framework.Major}` for NuGet package '{MicrosoftEntityFrameworkCoreProxiesPackageName}'"),
                    }
                );
            NugetRegistry.Register(MicrosoftEntityFrameworkCoreSqlServerPackageName,
                (framework) => framework switch
                    {
                        ( >= 9, 0) => new PackageVersion("9.0.0", locked: true)
                            .WithNugetDependency("Humanizer.Core", "2.14.1")
                            .WithNugetDependency("Microsoft.Build.Framework", "17.8.3")
                            .WithNugetDependency("Microsoft.Build.Locator", "1.7.8")
                            .WithNugetDependency("Microsoft.CodeAnalysis.CSharp", "4.8.0")
                            .WithNugetDependency("Microsoft.CodeAnalysis.CSharp.Workspaces", "4.8.0")
                            .WithNugetDependency("Microsoft.CodeAnalysis.Workspaces.MSBuild", "4.8.0")
                            .WithNugetDependency("Microsoft.EntityFrameworkCore.Relational", "9.0.0")
                            .WithNugetDependency("Microsoft.Extensions.Caching.Memory", "9.0.0")
                            .WithNugetDependency("Microsoft.Extensions.Configuration.Abstractions", "9.0.0")
                            .WithNugetDependency("Microsoft.Extensions.DependencyModel", "9.0.0")
                            .WithNugetDependency("Microsoft.Extensions.Logging", "9.0.0")
                            .WithNugetDependency("Mono.TextTemplating", "3.0.0")
                            .WithNugetDependency("System.Text.Json", "9.0.0"),
                        ( >= 8, 0) => new PackageVersion("8.0.11", locked: true),
                        ( >= 6, 0) => new PackageVersion("7.0.20"),
                        ( >= 2, 1) => new PackageVersion("5.0.17")
                            .WithNugetDependency("Microsoft.Data.SqlClient", "2.0.1")
                            .WithNugetDependency("Microsoft.EntityFrameworkCore.Relational", "5.0.17"),
                        ( >= 2, 0) => new PackageVersion("3.1.32")
                            .WithNugetDependency("Microsoft.Data.SqlClient", "1.1.3")
                            .WithNugetDependency("Microsoft.EntityFrameworkCore.Relational", "3.1.32"),
                        _ => throw new Exception($"Unsupported Framework `{framework.Major}` for NuGet package '{MicrosoftEntityFrameworkCoreSqlServerPackageName}'"),
                    }
                );
            NugetRegistry.Register(MicrosoftEntityFrameworkCoreSqlServerNetTopologySuitePackageName,
                (framework) => framework switch
                    {
                        ( >= 9, 0) => new PackageVersion("9.0.0", locked: true)
                            .WithNugetDependency("Humanizer.Core", "2.14.1")
                            .WithNugetDependency("Microsoft.Build.Framework", "17.8.3")
                            .WithNugetDependency("Microsoft.Build.Locator", "1.7.8")
                            .WithNugetDependency("Microsoft.CodeAnalysis.CSharp", "4.8.0")
                            .WithNugetDependency("Microsoft.CodeAnalysis.CSharp.Workspaces", "4.8.0")
                            .WithNugetDependency("Microsoft.CodeAnalysis.Workspaces.MSBuild", "4.8.0")
                            .WithNugetDependency("Microsoft.EntityFrameworkCore.Relational", "9.0.0")
                            .WithNugetDependency("Microsoft.Extensions.Caching.Memory", "9.0.0")
                            .WithNugetDependency("Microsoft.Extensions.Configuration.Abstractions", "9.0.0")
                            .WithNugetDependency("Microsoft.Extensions.DependencyModel", "9.0.0")
                            .WithNugetDependency("Microsoft.Extensions.Logging", "9.0.0")
                            .WithNugetDependency("Mono.TextTemplating", "3.0.0")
                            .WithNugetDependency("System.Text.Json", "9.0.0"),
                        ( >= 8, 0) => new PackageVersion("8.0.11", locked: true),
                        ( >= 6, 0) => new PackageVersion("7.0.20"),
                        ( >= 2, 1) => new PackageVersion("5.0.17")
                            .WithNugetDependency("Microsoft.EntityFrameworkCore.SqlServer", "5.0.17")
                            .WithNugetDependency("NetTopologySuite", "2.1.0")
                            .WithNugetDependency("NetTopologySuite.IO.SqlServerBytes", "2.0.0"),
                        ( >= 2, 0) => new PackageVersion("3.1.32")
                            .WithNugetDependency("Microsoft.EntityFrameworkCore.SqlServer", "3.1.32")
                            .WithNugetDependency("NetTopologySuite.IO.SqlServerBytes", "2.0.0"),
                        _ => throw new Exception($"Unsupported Framework `{framework.Major}` for NuGet package '{MicrosoftEntityFrameworkCoreSqlServerNetTopologySuitePackageName}'"),
                    }
                );
            NugetRegistry.Register(MicrosoftEntityFrameworkCoreToolsPackageName,
                (framework) => framework switch
                    {
                        ( >= 9, 0) => new PackageVersion("9.0.0", locked: true)
                            .WithNugetDependency("Humanizer.Core", "2.14.1")
                            .WithNugetDependency("Microsoft.Build.Framework", "17.8.3")
                            .WithNugetDependency("Microsoft.Build.Locator", "1.7.8")
                            .WithNugetDependency("Microsoft.CodeAnalysis.CSharp", "4.8.0")
                            .WithNugetDependency("Microsoft.CodeAnalysis.CSharp.Workspaces", "4.8.0")
                            .WithNugetDependency("Microsoft.CodeAnalysis.Workspaces.MSBuild", "4.8.0")
                            .WithNugetDependency("Microsoft.EntityFrameworkCore.Relational", "9.0.0")
                            .WithNugetDependency("Microsoft.Extensions.Caching.Memory", "9.0.0")
                            .WithNugetDependency("Microsoft.Extensions.Configuration.Abstractions", "9.0.0")
                            .WithNugetDependency("Microsoft.Extensions.DependencyModel", "9.0.0")
                            .WithNugetDependency("Microsoft.Extensions.Logging", "9.0.0")
                            .WithNugetDependency("Mono.TextTemplating", "3.0.0")
                            .WithNugetDependency("System.Text.Json", "9.0.0"),
                        ( >= 8, 0) => new PackageVersion("8.0.11", locked: true),
                        ( >= 6, 0) => new PackageVersion("7.0.20"),
                        ( >= 2, 0) => new PackageVersion("5.0.17")
                            .WithNugetDependency("Microsoft.EntityFrameworkCore.Design", "5.0.17"),
                        _ => throw new Exception($"Unsupported Framework `{framework.Major}` for NuGet package '{MicrosoftEntityFrameworkCoreToolsPackageName}'"),
                    }
                );
            NugetRegistry.Register(NpgsqlEntityFrameworkCorePostgreSQLPackageName,
                (framework) => framework switch
                    {
                        ( >= 9, 0) => new PackageVersion("9.0.0", locked: true)
                            .WithNugetDependency("Humanizer.Core", "2.14.1")
                            .WithNugetDependency("Microsoft.Build.Framework", "17.8.3")
                            .WithNugetDependency("Microsoft.Build.Locator", "1.7.8")
                            .WithNugetDependency("Microsoft.CodeAnalysis.CSharp", "4.8.0")
                            .WithNugetDependency("Microsoft.CodeAnalysis.CSharp.Workspaces", "4.8.0")
                            .WithNugetDependency("Microsoft.CodeAnalysis.Workspaces.MSBuild", "4.8.0")
                            .WithNugetDependency("Microsoft.EntityFrameworkCore.Relational", "9.0.0")
                            .WithNugetDependency("Microsoft.Extensions.Caching.Memory", "9.0.0")
                            .WithNugetDependency("Microsoft.Extensions.Configuration.Abstractions", "9.0.0")
                            .WithNugetDependency("Microsoft.Extensions.DependencyModel", "9.0.0")
                            .WithNugetDependency("Microsoft.Extensions.Logging", "9.0.0")
                            .WithNugetDependency("Mono.TextTemplating", "3.0.0")
                            .WithNugetDependency("System.Text.Json", "9.0.0"),
                        ( >= 8, 0) => new PackageVersion("8.0.11", locked: true),
                        ( >= 7, 0) => new PackageVersion("7.0.18"),
                        ( >= 6, 0) => new PackageVersion("7.0.18"),
                        ( >= 2, 1) => new PackageVersion("5.0.10")
                            .WithNugetDependency("Microsoft.EntityFrameworkCore", "5.0.10")
                            .WithNugetDependency("Microsoft.EntityFrameworkCore.Abstractions", "5.0.10")
                            .WithNugetDependency("Microsoft.EntityFrameworkCore.Relational", "5.0.10")
                            .WithNugetDependency("Npgsql", "5.0.10"),
                        ( >= 2, 0) => new PackageVersion("3.1.18")
                            .WithNugetDependency("Microsoft.EntityFrameworkCore", "3.1.18")
                            .WithNugetDependency("Microsoft.EntityFrameworkCore.Abstractions", "3.1.18")
                            .WithNugetDependency("Microsoft.EntityFrameworkCore.Relational", "3.1.18")
                            .WithNugetDependency("Npgsql", "4.1.9"),
                        _ => throw new Exception($"Unsupported Framework `{framework.Major}` for NuGet package '{NpgsqlEntityFrameworkCorePostgreSQLPackageName}'"),
                    }
                );
            NugetRegistry.Register(NpgsqlEntityFrameworkCorePostgreSQLNetTopologySuitePackageName,
                (framework) => framework switch
                    {
                        ( >= 9, 0) => new PackageVersion("9.0.0", locked: true)
                            .WithNugetDependency("Humanizer.Core", "2.14.1")
                            .WithNugetDependency("Microsoft.Build.Framework", "17.8.3")
                            .WithNugetDependency("Microsoft.Build.Locator", "1.7.8")
                            .WithNugetDependency("Microsoft.CodeAnalysis.CSharp", "4.8.0")
                            .WithNugetDependency("Microsoft.CodeAnalysis.CSharp.Workspaces", "4.8.0")
                            .WithNugetDependency("Microsoft.CodeAnalysis.Workspaces.MSBuild", "4.8.0")
                            .WithNugetDependency("Microsoft.EntityFrameworkCore.Relational", "9.0.0")
                            .WithNugetDependency("Microsoft.Extensions.Caching.Memory", "9.0.0")
                            .WithNugetDependency("Microsoft.Extensions.Configuration.Abstractions", "9.0.0")
                            .WithNugetDependency("Microsoft.Extensions.DependencyModel", "9.0.0")
                            .WithNugetDependency("Microsoft.Extensions.Logging", "9.0.0")
                            .WithNugetDependency("Mono.TextTemplating", "3.0.0")
                            .WithNugetDependency("System.Text.Json", "9.0.0"),
                        ( >= 8, 0) => new PackageVersion("8.0.11", locked: true),
                        ( >= 6, 0) => new PackageVersion("7.0.18"),
                        ( >= 2, 1) => new PackageVersion("5.0.10")
                            .WithNugetDependency("Npgsql.EntityFrameworkCore.PostgreSQL", "5.0.10")
                            .WithNugetDependency("Npgsql.NetTopologySuite", "5.0.10"),
                        ( >= 2, 0) => new PackageVersion("3.1.18")
                            .WithNugetDependency("Npgsql.EntityFrameworkCore.PostgreSQL", "3.1.18")
                            .WithNugetDependency("Npgsql.NetTopologySuite", "4.1.9"),
                        _ => throw new Exception($"Unsupported Framework `{framework.Major}` for NuGet package '{NpgsqlEntityFrameworkCorePostgreSQLNetTopologySuitePackageName}'"),
                    }
                );
            NugetRegistry.Register(OracleEntityFrameworkCorePackageName,
                (framework) => framework switch
                    {
                        ( >= 8, 0) => new PackageVersion("8.23.60")
                            .WithNugetDependency("Microsoft.EntityFrameworkCore.Relational", "8.0.3")
                            .WithNugetDependency("Oracle.ManagedDataAccess.Core", "23.6.0"),
                        ( >= 6, 0) => new PackageVersion("7.21.13"),
                        ( >= 2, 1) => new PackageVersion("5.21.90")
                            .WithNugetDependency("Microsoft.EntityFrameworkCore.Relational", "5.0.17")
                            .WithNugetDependency("Oracle.ManagedDataAccess.Core", "3.21.90"),
                        ( >= 2, 0) => new PackageVersion("3.19.180")
                            .WithNugetDependency("Microsoft.EntityFrameworkCore.Relational", "3.1.32")
                            .WithNugetDependency("Oracle.ManagedDataAccess.Core", "2.19.180"),
                        _ => throw new Exception($"Unsupported Framework `{framework.Major}` for NuGet package '{OracleEntityFrameworkCorePackageName}'"),
                    }
                );
            NugetRegistry.Register(PomeloEntityFrameworkCoreMySqlPackageName,
                (framework) => framework switch
                    {
                        ( >= 8, 0) => new PackageVersion("8.0.2"),
                        ( >= 7, 0) => new PackageVersion("7.0.0"),
                        ( >= 6, 0) => new PackageVersion("7.0.0"),
                        ( >= 2, 1) => new PackageVersion("5.0.4")
                            .WithNugetDependency("Microsoft.EntityFrameworkCore.Relational", "5.0.10")
                            .WithNugetDependency("Microsoft.Extensions.DependencyInjection", "5.0.2")
                            .WithNugetDependency("MySqlConnector", "1.3.13"),
                        ( >= 2, 0) => new PackageVersion("3.2.7")
                            .WithNugetDependency("Microsoft.EntityFrameworkCore.Relational", "3.1.19")
                            .WithNugetDependency("MySqlConnector", "0.69.10")
                            .WithNugetDependency("Pomelo.JsonObject", "2.2.1"),
                        _ => throw new Exception($"Unsupported Framework `{framework.Major}` for NuGet package '{PomeloEntityFrameworkCoreMySqlPackageName}'"),
                    }
                );
            NugetRegistry.Register(PomeloEntityFrameworkCoreMySqlNetTopologySuitePackageName,
                (framework) => framework switch
                    {
                        ( >= 8, 0) => new PackageVersion("8.0.2"),
                        ( >= 7, 0) => new PackageVersion("7.0.0"),
                        ( >= 6, 0) => new PackageVersion("7.0.0"),
                        ( >= 2, 1) => new PackageVersion("5.0.4")
                            .WithNugetDependency("Microsoft.EntityFrameworkCore.Relational", "5.0.10")
                            .WithNugetDependency("Microsoft.Extensions.DependencyInjection", "5.0.2")
                            .WithNugetDependency("MySqlConnector", "1.3.13")
                            .WithNugetDependency("NetTopologySuite", "2.2.0")
                            .WithNugetDependency("Pomelo.EntityFrameworkCore.MySql", "5.0.4"),
                        ( >= 2, 0) => new PackageVersion("3.2.7")
                            .WithNugetDependency("Microsoft.EntityFrameworkCore.Relational", "3.1.19")
                            .WithNugetDependency("MySqlConnector", "0.69.10")
                            .WithNugetDependency("NetTopologySuite", "2.0.0")
                            .WithNugetDependency("Pomelo.EntityFrameworkCore.MySql", "3.2.7"),
                        _ => throw new Exception($"Unsupported Framework `{framework.Major}` for NuGet package '{PomeloEntityFrameworkCoreMySqlNetTopologySuitePackageName}'"),
                    }
                );
        }
        public static NugetPackageInfo MicrosoftEntityFrameworkCore(IOutputTarget outputTarget) => NugetRegistry.GetVersion(MicrosoftEntityFrameworkCorePackageName, outputTarget.GetMaxNetAppVersion());
        public static NugetPackageInfo MicrosoftEntityFrameworkCoreDesign(IOutputTarget outputTarget) => NugetRegistry.GetVersion(MicrosoftEntityFrameworkCoreDesignPackageName, outputTarget.GetMaxNetAppVersion());
        public static NugetPackageInfo MicrosoftEntityFrameworkCoreTools(IOutputTarget outputTarget) => NugetRegistry.GetVersion(MicrosoftEntityFrameworkCoreToolsPackageName, outputTarget.GetMaxNetAppVersion());
        public static NugetPackageInfo MicrosoftEntityFrameworkCoreSqlServer(IOutputTarget outputTarget) => NugetRegistry.GetVersion(MicrosoftEntityFrameworkCoreSqlServerPackageName, outputTarget.GetMaxNetAppVersion());
        public static NugetPackageInfo MicrosoftEntityFrameworkCoreCosmos(IOutputTarget outputTarget) => NugetRegistry.GetVersion(MicrosoftEntityFrameworkCoreCosmosPackageName, outputTarget.GetMaxNetAppVersion());
        public static NugetPackageInfo MicrosoftEntityFrameworkCoreInMemory(IOutputTarget outputTarget) => NugetRegistry.GetVersion(MicrosoftEntityFrameworkCoreInMemoryPackageName, outputTarget.GetMaxNetAppVersion());
        public static NugetPackageInfo MicrosoftEntityFrameworkCoreProxies(IOutputTarget outputTarget) => NugetRegistry.GetVersion(MicrosoftEntityFrameworkCoreProxiesPackageName, outputTarget.GetMaxNetAppVersion());

        public static NugetPackageInfo NpgsqlEntityFrameworkCorePostgreSQL(IOutputTarget outputTarget) => NugetRegistry.GetVersion(NpgsqlEntityFrameworkCorePostgreSQLPackageName, outputTarget.GetMaxNetAppVersion());

        public static NugetPackageInfo PomeloEntityFrameworkCoreMySql(IOutputTarget outputTarget) => NugetRegistry.GetVersion(PomeloEntityFrameworkCoreMySqlPackageName, outputTarget.GetMaxNetAppVersion());

        public static NugetPackageInfo OracleEntityFrameworkCore(IOutputTarget outputTarget) => NugetRegistry.GetVersion(OracleEntityFrameworkCorePackageName, outputTarget.GetMaxNetAppVersion());

        public static NugetPackageInfo ErikEJEntityFrameworkCoreSqlServerDateOnlyTimeOnly(IOutputTarget outputTarget) => NugetRegistry.GetVersion(ErikEJEntityFrameworkCoreSqlServerDateOnlyTimeOnlyPackageName, outputTarget.GetMaxNetAppVersion());

        public static NugetPackageInfo NpgsqlEntityFrameworkCorePostgreSQLNetTopologySuite(IOutputTarget outputTarget) => NugetRegistry.GetVersion(NpgsqlEntityFrameworkCorePostgreSQLNetTopologySuitePackageName, outputTarget.GetMaxNetAppVersion());

        public static NugetPackageInfo MicrosoftEntityFrameworkCoreSqlServerNetTopologySuite(IOutputTarget outputTarget) => NugetRegistry.GetVersion(MicrosoftEntityFrameworkCoreSqlServerNetTopologySuitePackageName, outputTarget.GetMaxNetAppVersion());

        public static NugetPackageInfo PomeloEntityFrameworkCoreMySqlNetTopologySuite(IOutputTarget outputTarget) => NugetRegistry.GetVersion(PomeloEntityFrameworkCoreMySqlNetTopologySuitePackageName, outputTarget.GetMaxNetAppVersion());
    }
}
