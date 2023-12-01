using Intent.Engine;
using Intent.Modules.Common.VisualStudio;

namespace Intent.Modules.AspNetCore.HealthChecks;

public static class NugetPackage
{
    public static INugetPackageInfo AspNetCoreHealthChecksUI(IOutputTarget outputTarget) => new NugetPackageInfo(
        name: "AspNetCore.HealthChecks.UI",
        version: outputTarget.GetMaxNetAppVersion() switch
        {
            (5, 0) => "5.0.1",
            (6, 0) => "6.0.5",
            _ => "7.0.2"
        });

    public static INugetPackageInfo AspNetCoreHealthChecksUIClient(IOutputTarget outputTarget) => new NugetPackageInfo(
        name: "AspNetCore.HealthChecks.UI.Client",
        version: outputTarget.GetMaxNetAppVersion() switch
        {
            (5, 0) => "5.0.1",
            (6, 0) => "6.0.5",
            _ => "7.1.0"
        });

    public static INugetPackageInfo AspNetCoreHealthChecksUIInMemoryStorage(IOutputTarget outputTarget) => new NugetPackageInfo(
        name: "AspNetCore.HealthChecks.UI.InMemory.Storage",
        version: outputTarget.GetMaxNetAppVersion() switch
        {
            (5, 0) => "5.0.1",
            (6, 0) => "6.0.5",
            _ => "7.0.0"
        });

    public static INugetPackageInfo AspNetcoreHealthChecksPublisherApplicationInsights(IOutputTarget outputTarget) => new NugetPackageInfo(
        name: "AspNetcore.HealthChecks.Publisher.ApplicationInsights",
        version: outputTarget.GetMaxNetAppVersion() switch
        {
            (5, 0) => "5.0.1",
            (6, 0) => "6.0.2",
            _ => "7.0.0"
        });

    public static INugetPackageInfo AspNetCoreHealthChecksSqlServer(IOutputTarget outputTarget) => new NugetPackageInfo(
        name: "AspNetCore.HealthChecks.SqlServer",
        version: outputTarget.GetMaxNetAppVersion() switch
        {
            (5, 0) => "5.0.3",
            (6, 0) => "6.0.2",
            _ => "7.0.0"
        });

    public static INugetPackageInfo AspNetCoreHealthChecksNpgSql(IOutputTarget outputTarget) => new NugetPackageInfo(
        name: "AspNetCore.HealthChecks.NpgSql",
        version: outputTarget.GetMaxNetAppVersion() switch
        {
            (5, 0) => "5.0.2",
            (6, 0) => "6.0.2",
            _ => "7.1.0"
        });

    public static INugetPackageInfo AspNetCoreHealthChecksMySql(IOutputTarget outputTarget) => new NugetPackageInfo(
        name: "AspNetCore.HealthChecks.MySql",
        version: outputTarget.GetMaxNetAppVersion() switch
        {
            (5, 0) => "5.0.1",
            (6, 0) => "6.0.2",
            _ => "7.0.0"
        });

    public static INugetPackageInfo AspNetCoreHealthChecksCosmosDb(IOutputTarget outputTarget) => new NugetPackageInfo(
        name: "AspNetCore.HealthChecks.CosmosDb",
        version: outputTarget.GetMaxNetAppVersion() switch
        {
            (5, 0) => "5.0.4",
            (6, 0) => "6.1.0",
            _ => "7.0.0"
        });

    public static INugetPackageInfo AspNetCoreHealthChecksMongoDb(IOutputTarget outputTarget) => new NugetPackageInfo(
        name: "AspNetCore.HealthChecks.MongoDb",
        version: outputTarget.GetMaxNetAppVersion() switch
        {
            (5, 0) => "5.0.1",
            (6, 0) => "6.0.2",
            _ => "7.0.0"
        });
}