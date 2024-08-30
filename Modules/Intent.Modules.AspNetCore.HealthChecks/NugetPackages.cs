using System;
using Intent.Engine;
using Intent.Modules.Common.CSharp.Nuget;
using Intent.Modules.Common.VisualStudio;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.NugetPackages", Version = "1.0")]

namespace Intent.Modules.AspNetCore.HealthChecks
{
    public class NugetPackages : INugetPackages
    {
        public const string AspNetCoreHealthChecksCosmosDbPackageName = "AspNetCore.HealthChecks.CosmosDb";
        public const string AspNetCoreHealthChecksKafkaPackageName = "AspNetCore.HealthChecks.Kafka";
        public const string AspNetCoreHealthChecksMongoDbPackageName = "AspNetCore.HealthChecks.MongoDb";
        public const string AspNetCoreHealthChecksMySqlPackageName = "AspNetCore.HealthChecks.MySql";
        public const string AspNetCoreHealthChecksNpgSqlPackageName = "AspNetCore.HealthChecks.NpgSql";
        public const string AspNetCoreHealthChecksOraclePackageName = "AspNetCore.HealthChecks.Oracle";
        public const string AspNetcoreHealthChecksPublisherApplicationInsightsPackageName = "AspNetcore.HealthChecks.Publisher.ApplicationInsights";
        public const string AspNetCoreHealthChecksRedisPackageName = "AspNetCore.HealthChecks.Redis";
        public const string AspNetCoreHealthChecksSqlServerPackageName = "AspNetCore.HealthChecks.SqlServer";
        public const string AspNetCoreHealthChecksUIPackageName = "AspNetCore.HealthChecks.UI";
        public const string AspNetCoreHealthChecksUIClientPackageName = "AspNetCore.HealthChecks.UI.Client";
        public const string AspNetCoreHealthChecksUIInMemoryStoragePackageName = "AspNetCore.HealthChecks.UI.InMemory.Storage";
        public const string AspNetCoreHealthChecksUrisPackageName = "AspNetCore.HealthChecks.Uris";

        public void RegisterPackages()
        {
            NugetRegistry.Register(AspNetCoreHealthChecksCosmosDbPackageName,
                (framework) => framework switch
                    {
                        ( >= 6, 0) => new PackageVersion("8.0.1"),
                        _ => throw new Exception($"Unsupported Framework `{framework.Major}` for NuGet package '{AspNetCoreHealthChecksCosmosDbPackageName}'"),
                    }
                );
            NugetRegistry.Register(AspNetCoreHealthChecksKafkaPackageName,
                (framework) => framework switch
                    {
                        ( >= 6, 0) => new PackageVersion("8.0.1"),
                        _ => throw new Exception($"Unsupported Framework `{framework.Major}` for NuGet package '{AspNetCoreHealthChecksKafkaPackageName}'"),
                    }
                );
            NugetRegistry.Register(AspNetCoreHealthChecksMongoDbPackageName,
                (framework) => framework switch
                    {
                        ( >= 6, 0) => new PackageVersion("8.0.1"),
                        _ => throw new Exception($"Unsupported Framework `{framework.Major}` for NuGet package '{AspNetCoreHealthChecksMongoDbPackageName}'"),
                    }
                );
            NugetRegistry.Register(AspNetCoreHealthChecksMySqlPackageName,
                (framework) => framework switch
                    {
                        ( >= 6, 0) => new PackageVersion("8.0.1"),
                        _ => throw new Exception($"Unsupported Framework `{framework.Major}` for NuGet package '{AspNetCoreHealthChecksMySqlPackageName}'"),
                    }
                );
            NugetRegistry.Register(AspNetCoreHealthChecksNpgSqlPackageName,
                (framework) => framework switch
                    {
                        ( >= 8, 0) => new PackageVersion("8.0.1"),
                        ( >= 7, 0) => new PackageVersion("8.0.1"),
                        ( >= 6, 0) => new PackageVersion("6.0.2"),
                        _ => throw new Exception($"Unsupported Framework `{framework.Major}` for NuGet package '{AspNetCoreHealthChecksNpgSqlPackageName}'"),
                    }
                );
            NugetRegistry.Register(AspNetCoreHealthChecksOraclePackageName,
                (framework) => framework switch
                    {
                        ( >= 6, 0) => new PackageVersion("8.0.1"),
                        _ => throw new Exception($"Unsupported Framework `{framework.Major}` for NuGet package '{AspNetCoreHealthChecksOraclePackageName}'"),
                    }
                );
            NugetRegistry.Register(AspNetcoreHealthChecksPublisherApplicationInsightsPackageName,
                (framework) => framework switch
                    {
                        ( >= 6, 0) => new PackageVersion("8.0.1"),
                        _ => throw new Exception($"Unsupported Framework `{framework.Major}` for NuGet package '{AspNetcoreHealthChecksPublisherApplicationInsightsPackageName}'"),
                    }
                );
            NugetRegistry.Register(AspNetCoreHealthChecksRedisPackageName,
                (framework) => framework switch
                    {
                        ( >= 6, 0) => new PackageVersion("8.0.1"),
                        _ => throw new Exception($"Unsupported Framework `{framework.Major}` for NuGet package '{AspNetCoreHealthChecksRedisPackageName}'"),
                    }
                );
            NugetRegistry.Register(AspNetCoreHealthChecksSqlServerPackageName,
                (framework) => framework switch
                    {
                        ( >= 6, 0) => new PackageVersion("8.0.2"),
                        _ => throw new Exception($"Unsupported Framework `{framework.Major}` for NuGet package '{AspNetCoreHealthChecksSqlServerPackageName}'"),
                    }
                );
            NugetRegistry.Register(AspNetCoreHealthChecksUIPackageName,
                (framework) => framework switch
                    {
                        ( >= 8, 0) => new PackageVersion("8.0.2"),
                        ( >= 6, 0) => new PackageVersion("7.0.2"),
                        _ => throw new Exception($"Unsupported Framework `{framework.Major}` for NuGet package '{AspNetCoreHealthChecksUIPackageName}'"),
                    }
                );
            NugetRegistry.Register(AspNetCoreHealthChecksUIClientPackageName,
                (framework) => framework switch
                    {
                        ( >= 8, 0) => new PackageVersion("8.0.1"),
                        ( >= 7, 0) => new PackageVersion("8.0.1"),
                        ( >= 6, 0) => new PackageVersion("8.0.1"),
                        _ => throw new Exception($"Unsupported Framework `{framework.Major}` for NuGet package '{AspNetCoreHealthChecksUIClientPackageName}'"),
                    }
                );
            NugetRegistry.Register(AspNetCoreHealthChecksUIInMemoryStoragePackageName,
                (framework) => framework switch
                    {
                        ( >= 8, 0) => new PackageVersion("8.0.1"),
                        ( >= 6, 0) => new PackageVersion("7.0.0"),
                        _ => throw new Exception($"Unsupported Framework `{framework.Major}` for NuGet package '{AspNetCoreHealthChecksUIInMemoryStoragePackageName}'"),
                    }
                );
            NugetRegistry.Register(AspNetCoreHealthChecksUrisPackageName,
                (framework) => framework switch
                    {
                        ( >= 6, 0) => new PackageVersion("6.0.3"),
                        _ => throw new Exception($"Unsupported Framework `{framework.Major}` for NuGet package '{AspNetCoreHealthChecksUrisPackageName}'"),
                    }
                );
        }

        public static NugetPackageInfo AspNetCoreHealthChecksUI(IOutputTarget outputTarget) => NugetRegistry.GetVersion(AspNetCoreHealthChecksUIPackageName, outputTarget.GetMaxNetAppVersion());

        public static NugetPackageInfo AspNetCoreHealthChecksUIClient(IOutputTarget outputTarget) => NugetRegistry.GetVersion(AspNetCoreHealthChecksUIClientPackageName, outputTarget.GetMaxNetAppVersion());

        public static NugetPackageInfo AspNetCoreHealthChecksUIInMemoryStorage(IOutputTarget outputTarget) => NugetRegistry.GetVersion(AspNetCoreHealthChecksUIInMemoryStoragePackageName, outputTarget.GetMaxNetAppVersion());

        public static NugetPackageInfo AspNetcoreHealthChecksPublisherApplicationInsights(IOutputTarget outputTarget) => NugetRegistry.GetVersion(AspNetcoreHealthChecksPublisherApplicationInsightsPackageName, outputTarget.GetMaxNetAppVersion());

        public static NugetPackageInfo AspNetCoreHealthChecksSqlServer(IOutputTarget outputTarget) => NugetRegistry.GetVersion(AspNetCoreHealthChecksSqlServerPackageName, outputTarget.GetMaxNetAppVersion());

        public static NugetPackageInfo AspNetCoreHealthChecksNpgSql(IOutputTarget outputTarget) => NugetRegistry.GetVersion(AspNetCoreHealthChecksNpgSqlPackageName, outputTarget.GetMaxNetAppVersion());

        public static NugetPackageInfo AspNetCoreHealthChecksMySql(IOutputTarget outputTarget) => NugetRegistry.GetVersion(AspNetCoreHealthChecksMySqlPackageName, outputTarget.GetMaxNetAppVersion());

        public static NugetPackageInfo AspNetCoreHealthChecksCosmosDb(IOutputTarget outputTarget) => NugetRegistry.GetVersion(AspNetCoreHealthChecksCosmosDbPackageName, outputTarget.GetMaxNetAppVersion());

        public static NugetPackageInfo AspNetCoreHealthChecksMongoDb(IOutputTarget outputTarget) => NugetRegistry.GetVersion(AspNetCoreHealthChecksMongoDbPackageName, outputTarget.GetMaxNetAppVersion());

        public static NugetPackageInfo AspNetCoreHealthChecksOracle(IOutputTarget outputTarget) => NugetRegistry.GetVersion(AspNetCoreHealthChecksOraclePackageName, outputTarget.GetMaxNetAppVersion());

        public static NugetPackageInfo AspNetCoreHealthChecksRedis(IOutputTarget outputTarget) => NugetRegistry.GetVersion(AspNetCoreHealthChecksRedisPackageName, outputTarget.GetMaxNetAppVersion());

        public static NugetPackageInfo AspNetCoreHealthChecksKafka(IOutputTarget outputTarget) => NugetRegistry.GetVersion(AspNetCoreHealthChecksKafkaPackageName, outputTarget.GetMaxNetAppVersion());

        public static NugetPackageInfo AspNetCoreHealthChecksUris(IOutputTarget outputTarget) => NugetRegistry.GetVersion(AspNetCoreHealthChecksUrisPackageName, outputTarget.GetMaxNetAppVersion());
    }
}
