using Intent.Engine;
using Intent.Modules.Common.VisualStudio;

namespace Intent.Modules.AspNetCore.IntegrationTesting
{
    public static class NugetPackages
    {

        public static NugetPackageInfo AutoFixture(IOutputTarget outputTarget) => new(
            name: "AutoFixture",
            version: outputTarget.GetMaxNetAppVersion() switch
            {
                _ => "4.18.1",
            });

        public static NugetPackageInfo MicrosoftNETTestSdk(IOutputTarget outputTarget) => new(
            name: "Microsoft.NET.Test.Sdk",
            version: outputTarget.GetMaxNetAppVersion() switch
            {
                _ => "15.5.0",
            });

        public static NugetPackageInfo Xunit(IOutputTarget outputTarget) => new(
            name: "xunit",
            version: outputTarget.GetMaxNetAppVersion() switch
            {
                _ => "2.9.0",
            });

        public static NugetPackageInfo XunitRunnerVisualstudio(IOutputTarget outputTarget) => new(
            name: "xunit.runner.visualstudio",
            version: outputTarget.GetMaxNetAppVersion() switch
            {
                _ => "2.8.2",
            });

        public static NugetPackageInfo TestcontainersCosmosDb(IOutputTarget outputTarget) => new(
            name: "Testcontainers.CosmosDb",
            version: outputTarget.GetMaxNetAppVersion() switch
            {
                (6, 0) => "3.9.0",
                _ => "3.9.0",
            });

        public static NugetPackageInfo TestcontainersMsSql(IOutputTarget outputTarget) => new(
            name: "Testcontainers.MsSql",
            version: outputTarget.GetMaxNetAppVersion() switch
            {
                (6, 0) => "3.9.0",
                _ => "3.9.0",
            });

        public static NugetPackageInfo TestcontainersPostgreSql(IOutputTarget outputTarget) => new(
            name: "Testcontainers.PostgreSql",
            version: outputTarget.GetMaxNetAppVersion() switch
            {
                (6, 0) => "3.9.0",
                _ => "3.9.0",
            });

        public static NugetPackageInfo TestcontainersMongoDb(IOutputTarget outputTarget) => new(
            name: "Testcontainers.MongoDb",
            version: outputTarget.GetMaxNetAppVersion() switch
            {
                (6, 0) => "3.9.0",
                _ => "3.9.0",
            });

        public static NugetPackageInfo IEvangelistAzureCosmosRepository(IOutputTarget outputTarget) => new(
            name: "IEvangelist.Azure.CosmosRepository",
            version: outputTarget.GetMaxNetAppVersion() switch
            {
                (7, 0) => "8.1.7",
                _ => "8.1.7",
            });

        public static NugetPackageInfo MicrosoftAspNetCoreMvcTesting(IOutputTarget outputTarget) => new(
            name: "Microsoft.AspNetCore.Mvc.Testing",
            version: outputTarget.GetMaxNetAppVersion() switch
            {
                (6, 0) => "6.0.32",
                (7, 0) => "7.0.20",
                _ => "8.0.7",
            });
    }
}
