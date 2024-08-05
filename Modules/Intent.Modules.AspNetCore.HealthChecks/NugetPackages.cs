using System;
using Intent.Engine;
using Intent.Modules.Common.VisualStudio;

namespace Intent.Modules.AspNetCore.HealthChecks
{
    public static class NugetPackages
    {

        public static NugetPackageInfo AspNetCoreHealthChecksUI(IOutputTarget outputTarget) => new NugetPackageInfo(
            name: "AspNetCore.HealthChecks.UI",
            version: outputTarget.GetMaxNetAppVersion() switch
            {
                (>= 8, 0) => "8.0.1",
                (>= 6, 0) => "7.0.2",
                _ => throw new Exception($"Unsupported Framework `{outputTarget.GetMaxNetAppVersion().Major}` for NuGet package 'AspNetCore.HealthChecks.UI'")
            });

        public static NugetPackageInfo AspNetCoreHealthChecksUIClient(IOutputTarget outputTarget) => new NugetPackageInfo(
            name: "AspNetCore.HealthChecks.UI.Client",
            version: outputTarget.GetMaxNetAppVersion() switch
            {
                (>= 8, 0) => "8.0.1",
                (>= 7, 0) => "8.0.1",
                (>= 6, 0) => "8.0.1",
                _ => throw new Exception($"Unsupported Framework `{outputTarget.GetMaxNetAppVersion().Major}` for NuGet package 'AspNetCore.HealthChecks.UI.Client'")
            });

        public static NugetPackageInfo AspNetCoreHealthChecksUIInMemoryStorage(IOutputTarget outputTarget) => new NugetPackageInfo(
            name: "AspNetCore.HealthChecks.UI.InMemory.Storage",
            version: outputTarget.GetMaxNetAppVersion() switch
            {
                (>= 8, 0) => "8.0.1",
                (>= 6, 0) => "7.0.0",
                _ => throw new Exception($"Unsupported Framework `{outputTarget.GetMaxNetAppVersion().Major}` for NuGet package 'AspNetCore.HealthChecks.UI.InMemory.Storage'")
            });

        public static NugetPackageInfo AspNetcoreHealthChecksPublisherApplicationInsights(IOutputTarget outputTarget) => new NugetPackageInfo(
            name: "AspNetcore.HealthChecks.Publisher.ApplicationInsights",
            version: outputTarget.GetMaxNetAppVersion() switch
            {
                (>= 6, 0) => "8.0.1",
                _ => throw new Exception($"Unsupported Framework `{outputTarget.GetMaxNetAppVersion().Major}` for NuGet package 'AspNetcore.HealthChecks.Publisher.ApplicationInsights'")
            });

        public static NugetPackageInfo AspNetCoreHealthChecksSqlServer(IOutputTarget outputTarget) => new NugetPackageInfo(
            name: "AspNetCore.HealthChecks.SqlServer",
            version: outputTarget.GetMaxNetAppVersion() switch
            {
                (>= 6, 0) => "8.0.2",
                _ => throw new Exception($"Unsupported Framework `{outputTarget.GetMaxNetAppVersion().Major}` for NuGet package 'AspNetCore.HealthChecks.SqlServer'")
            });

        public static NugetPackageInfo AspNetCoreHealthChecksNpgSql(IOutputTarget outputTarget) => new NugetPackageInfo(
            name: "AspNetCore.HealthChecks.NpgSql",
            version: outputTarget.GetMaxNetAppVersion() switch
            {
                (>= 8, 0) => "8.0.1",
                (>= 7, 0) => "8.0.1",
                (>= 6, 0) => "6.0.2",
                _ => throw new Exception($"Unsupported Framework `{outputTarget.GetMaxNetAppVersion().Major}` for NuGet package 'AspNetCore.HealthChecks.NpgSql'")
            });

        public static NugetPackageInfo AspNetCoreHealthChecksMySql(IOutputTarget outputTarget) => new NugetPackageInfo(
            name: "AspNetCore.HealthChecks.MySql",
            version: outputTarget.GetMaxNetAppVersion() switch
            {
                (>= 6, 0) => "8.0.1",
                _ => throw new Exception($"Unsupported Framework `{outputTarget.GetMaxNetAppVersion().Major}` for NuGet package 'AspNetCore.HealthChecks.MySql'")
            });

        public static NugetPackageInfo AspNetCoreHealthChecksCosmosDb(IOutputTarget outputTarget) => new NugetPackageInfo(
            name: "AspNetCore.HealthChecks.CosmosDb",
            version: outputTarget.GetMaxNetAppVersion() switch
            {
                (>= 6, 0) => "8.0.1",
                _ => throw new Exception($"Unsupported Framework `{outputTarget.GetMaxNetAppVersion().Major}` for NuGet package 'AspNetCore.HealthChecks.CosmosDb'")
            });

        public static NugetPackageInfo AspNetCoreHealthChecksMongoDb(IOutputTarget outputTarget) => new NugetPackageInfo(
            name: "AspNetCore.HealthChecks.MongoDb",
            version: outputTarget.GetMaxNetAppVersion() switch
            {
                (>= 6, 0) => "8.0.1",
                _ => throw new Exception($"Unsupported Framework `{outputTarget.GetMaxNetAppVersion().Major}` for NuGet package 'AspNetCore.HealthChecks.MongoDb'")
            });

        public static NugetPackageInfo AspNetCoreHealthChecksOracle(IOutputTarget outputTarget) => new NugetPackageInfo(
            name: "AspNetCore.HealthChecks.Oracle",
            version: outputTarget.GetMaxNetAppVersion() switch
            {
                (>= 6, 0) => "8.0.1",
                _ => throw new Exception($"Unsupported Framework `{outputTarget.GetMaxNetAppVersion().Major}` for NuGet package 'AspNetCore.HealthChecks.Oracle'")
            });

        public static NugetPackageInfo AspNetCoreHealthChecksRedis(IOutputTarget outputTarget) => new NugetPackageInfo(
            name: "AspNetCore.HealthChecks.Redis",
            version: outputTarget.GetMaxNetAppVersion() switch
            {
                (>= 6, 0) => "8.0.1",
                _ => throw new Exception($"Unsupported Framework `{outputTarget.GetMaxNetAppVersion().Major}` for NuGet package 'AspNetCore.HealthChecks.Redis'")
            });

        public static NugetPackageInfo AspNetCoreHealthChecksKafka(IOutputTarget outputTarget) => new NugetPackageInfo(
            name: "AspNetCore.HealthChecks.Kafka",
            version: outputTarget.GetMaxNetAppVersion() switch
            {
                (>= 6, 0) => "8.0.1",
                _ => throw new Exception($"Unsupported Framework `{outputTarget.GetMaxNetAppVersion().Major}` for NuGet package 'AspNetCore.HealthChecks.Kafka'")
            });

        public static NugetPackageInfo AspNetCoreHealthChecksUris(IOutputTarget outputTarget) => new NugetPackageInfo(
            name: "AspNetCore.HealthChecks.Uris",
            version: outputTarget.GetMaxNetAppVersion() switch
            {
                (>= 6, 0) => "6.0.3",
                _ => throw new Exception($"Unsupported Framework `{outputTarget.GetMaxNetAppVersion().Major}` for NuGet package 'AspNetCore.HealthChecks.Uris'")
            });
    }
}
