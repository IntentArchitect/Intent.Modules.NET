using Intent.Engine;
using Intent.Modules.Common.VisualStudio;

namespace Intent.Modules.AspNetCore.HealthChecks
{
    public static class NugetPackages
    {

        public static NugetPackageInfo AspNetCoreHealthChecksUI(IOutputTarget outputTarget) => new(
            name: "AspNetCore.HealthChecks.UI",
            version: outputTarget.GetMaxNetAppVersion() switch
            {
                (6, 0) => "7.0.2",
                _ => "8.0.1",
            });

        public static NugetPackageInfo AspNetCoreHealthChecksUIClient(IOutputTarget outputTarget) => new(
            name: "AspNetCore.HealthChecks.UI.Client",
            version: outputTarget.GetMaxNetAppVersion() switch
            {
                (6, 0) => "8.0.1",
                (7, 0) => "8.0.1",
                _ => "8.0.1",
            });

        public static NugetPackageInfo AspNetCoreHealthChecksUIInMemoryStorage(IOutputTarget outputTarget) => new(
            name: "AspNetCore.HealthChecks.UI.InMemory.Storage",
            version: outputTarget.GetMaxNetAppVersion() switch
            {
                (6, 0) => "7.0.0",
                _ => "8.0.1",
            });

        public static NugetPackageInfo AspNetcoreHealthChecksPublisherApplicationInsights(IOutputTarget outputTarget) => new(
            name: "AspNetcore.HealthChecks.Publisher.ApplicationInsights",
            version: outputTarget.GetMaxNetAppVersion() switch
            {
                _ => "8.0.1",
            });

        public static NugetPackageInfo AspNetCoreHealthChecksSqlServer(IOutputTarget outputTarget) => new(
            name: "AspNetCore.HealthChecks.SqlServer",
            version: outputTarget.GetMaxNetAppVersion() switch
            {
                _ => "8.0.2",
            });

        public static NugetPackageInfo AspNetCoreHealthChecksNpgSql(IOutputTarget outputTarget) => new(
            name: "AspNetCore.HealthChecks.NpgSql",
            version: outputTarget.GetMaxNetAppVersion() switch
            {
                (6, 0) => "6.0.2",
                (7, 0) => "8.0.1",
                _ => "8.0.1",
            });

        public static NugetPackageInfo AspNetCoreHealthChecksMySql(IOutputTarget outputTarget) => new(
            name: "AspNetCore.HealthChecks.MySql",
            version: outputTarget.GetMaxNetAppVersion() switch
            {
                _ => "8.0.1",
            });

        public static NugetPackageInfo AspNetCoreHealthChecksCosmosDb(IOutputTarget outputTarget) => new(
            name: "AspNetCore.HealthChecks.CosmosDb",
            version: outputTarget.GetMaxNetAppVersion() switch
            {
                _ => "8.0.1",
            });

        public static NugetPackageInfo AspNetCoreHealthChecksMongoDb(IOutputTarget outputTarget) => new(
            name: "AspNetCore.HealthChecks.MongoDb",
            version: outputTarget.GetMaxNetAppVersion() switch
            {
                _ => "8.0.1",
            });

        public static NugetPackageInfo AspNetCoreHealthChecksOracle(IOutputTarget outputTarget) => new(
            name: "AspNetCore.HealthChecks.Oracle",
            version: outputTarget.GetMaxNetAppVersion() switch
            {
                _ => "8.0.1",
            });

        public static NugetPackageInfo AspNetCoreHealthChecksRedis(IOutputTarget outputTarget) => new(
            name: "AspNetCore.HealthChecks.Redis",
            version: outputTarget.GetMaxNetAppVersion() switch
            {
                _ => "8.0.1",
            });

        public static NugetPackageInfo AspNetCoreHealthChecksKafka(IOutputTarget outputTarget) => new(
            name: "AspNetCore.HealthChecks.Kafka",
            version: outputTarget.GetMaxNetAppVersion() switch
            {
                _ => "8.0.1",
            });

        public static NugetPackageInfo AspNetCoreHealthChecksUris(IOutputTarget outputTarget) => new(
            name: "AspNetCore.HealthChecks.Uris",
            version: outputTarget.GetMaxNetAppVersion() switch
            {
                _ => "6.0.3",
            });
    }
}
