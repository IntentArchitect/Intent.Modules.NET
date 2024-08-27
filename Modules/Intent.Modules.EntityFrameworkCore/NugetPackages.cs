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
                        ( >= 8, 0) => new PackageVersion("8.0.7"),
                        ( >= 7, 0) => new PackageVersion("7.0.20"),
                        ( >= 6, 0) => new PackageVersion("7.0.20"),
                        _ => throw new Exception($"Unsupported Framework `{framework.Major}` for NuGet package '{MicrosoftEntityFrameworkCorePackageName}'"),
                    }
                );
            NugetRegistry.Register(MicrosoftEntityFrameworkCoreCosmosPackageName,
                (framework) => framework switch
                    {
                        ( >= 8, 0) => new PackageVersion("8.0.7"),
                        ( >= 7, 0) => new PackageVersion("7.0.20"),
                        ( >= 6, 0) => new PackageVersion("7.0.20"),
                        _ => throw new Exception($"Unsupported Framework `{framework.Major}` for NuGet package '{MicrosoftEntityFrameworkCoreCosmosPackageName}'"),
                    }
                );
            NugetRegistry.Register(MicrosoftEntityFrameworkCoreDesignPackageName,
                (framework) => framework switch
                    {
                        ( >= 8, 0) => new PackageVersion("8.0.7"),
                        ( >= 7, 0) => new PackageVersion("7.0.20"),
                        ( >= 6, 0) => new PackageVersion("7.0.20"),
                        _ => throw new Exception($"Unsupported Framework `{framework.Major}` for NuGet package '{MicrosoftEntityFrameworkCoreDesignPackageName}'"),
                    }
                );
            NugetRegistry.Register(MicrosoftEntityFrameworkCoreInMemoryPackageName,
                (framework) => framework switch
                    {
                        ( >= 8, 0) => new PackageVersion("8.0.7"),
                        ( >= 7, 0) => new PackageVersion("7.0.20"),
                        ( >= 6, 0) => new PackageVersion("7.0.20"),
                        _ => throw new Exception($"Unsupported Framework `{framework.Major}` for NuGet package '{MicrosoftEntityFrameworkCoreInMemoryPackageName}'"),
                    }
                );
            NugetRegistry.Register(MicrosoftEntityFrameworkCoreProxiesPackageName,
                (framework) => framework switch
                    {
                        ( >= 8, 0) => new PackageVersion("8.0.7"),
                        ( >= 7, 0) => new PackageVersion("7.0.20"),
                        ( >= 6, 0) => new PackageVersion("7.0.20"),
                        _ => throw new Exception($"Unsupported Framework `{framework.Major}` for NuGet package '{MicrosoftEntityFrameworkCoreProxiesPackageName}'"),
                    }
                );
            NugetRegistry.Register(MicrosoftEntityFrameworkCoreSqlServerPackageName,
                (framework) => framework switch
                    {
                        ( >= 8, 0) => new PackageVersion("8.0.7"),
                        ( >= 7, 0) => new PackageVersion("7.0.20"),
                        ( >= 6, 0) => new PackageVersion("7.0.20"),
                        _ => throw new Exception($"Unsupported Framework `{framework.Major}` for NuGet package '{MicrosoftEntityFrameworkCoreSqlServerPackageName}'"),
                    }
                );
            NugetRegistry.Register(MicrosoftEntityFrameworkCoreSqlServerNetTopologySuitePackageName,
                (framework) => framework switch
                    {
                        ( >= 8, 0) => new PackageVersion("8.0.7"),
                        ( >= 6, 0) => new PackageVersion("7.0.20"),
                        _ => throw new Exception($"Unsupported Framework `{framework.Major}` for NuGet package '{MicrosoftEntityFrameworkCoreSqlServerNetTopologySuitePackageName}'"),
                    }
                );
            NugetRegistry.Register(MicrosoftEntityFrameworkCoreToolsPackageName,
                (framework) => framework switch
                    {
                        ( >= 8, 0) => new PackageVersion("8.0.7"),
                        ( >= 7, 0) => new PackageVersion("7.0.20"),
                        ( >= 6, 0) => new PackageVersion("7.0.20"),
                        _ => throw new Exception($"Unsupported Framework `{framework.Major}` for NuGet package '{MicrosoftEntityFrameworkCoreToolsPackageName}'"),
                    }
                );
            NugetRegistry.Register(NpgsqlEntityFrameworkCorePostgreSQLPackageName,
                (framework) => framework switch
                    {
                        ( >= 8, 0) => new PackageVersion("8.0.4"),
                        ( >= 7, 0) => new PackageVersion("7.0.18"),
                        ( >= 6, 0) => new PackageVersion("7.0.18"),
                        _ => throw new Exception($"Unsupported Framework `{framework.Major}` for NuGet package '{NpgsqlEntityFrameworkCorePostgreSQLPackageName}'"),
                    }
                );
            NugetRegistry.Register(NpgsqlEntityFrameworkCorePostgreSQLNetTopologySuitePackageName,
                (framework) => framework switch
                    {
                        ( >= 8, 0) => new PackageVersion("8.0.4"),
                        ( >= 6, 0) => new PackageVersion("7.0.18"),
                        _ => throw new Exception($"Unsupported Framework `{framework.Major}` for NuGet package '{NpgsqlEntityFrameworkCorePostgreSQLNetTopologySuitePackageName}'"),
                    }
                );
            NugetRegistry.Register(OracleEntityFrameworkCorePackageName,
                (framework) => framework switch
                    {
                        ( >= 8, 0) => new PackageVersion("8.23.50"),
                        ( >= 6, 0) => new PackageVersion("7.21.13"),
                        _ => throw new Exception($"Unsupported Framework `{framework.Major}` for NuGet package '{OracleEntityFrameworkCorePackageName}'"),
                    }
                );
            NugetRegistry.Register(PomeloEntityFrameworkCoreMySqlPackageName,
                (framework) => framework switch
                    {
                        ( >= 8, 0) => new PackageVersion("8.0.2"),
                        ( >= 7, 0) => new PackageVersion("7.0.0"),
                        ( >= 6, 0) => new PackageVersion("7.0.0"),
                        _ => throw new Exception($"Unsupported Framework `{framework.Major}` for NuGet package '{PomeloEntityFrameworkCoreMySqlPackageName}'"),
                    }
                );
            NugetRegistry.Register(PomeloEntityFrameworkCoreMySqlNetTopologySuitePackageName,
                (framework) => framework switch
                    {
                        ( >= 8, 0) => new PackageVersion("8.0.2"),
                        ( >= 7, 0) => new PackageVersion("7.0.0"),
                        ( >= 6, 0) => new PackageVersion("7.0.0"),
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
