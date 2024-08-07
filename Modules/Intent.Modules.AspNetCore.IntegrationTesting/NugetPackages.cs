using System;
using Intent.Engine;
using Intent.Modules.Common.VisualStudio;

namespace Intent.Modules.AspNetCore.IntegrationTesting
{
    public static class NugetPackages
    {

        public static NugetPackageInfo AutoFixture(IOutputTarget outputTarget) => new NugetPackageInfo(
            name: "AutoFixture",
            version: outputTarget.GetMaxNetAppVersion() switch
            {
                (>= 2, 0) => "4.18.1",
                _ => throw new Exception($"Unsupported Framework `{outputTarget.GetMaxNetAppVersion().Major}` for NuGet package 'AutoFixture'")
            });

        public static NugetPackageInfo MicrosoftNETTestSdk(IOutputTarget outputTarget) => new NugetPackageInfo(
            name: "Microsoft.NET.Test.Sdk",
            version: outputTarget.GetMaxNetAppVersion() switch
            {
                (>= 0, 0) => "15.5.0",
                _ => throw new Exception($"Unsupported Framework `{outputTarget.GetMaxNetAppVersion().Major}` for NuGet package 'Microsoft.NET.Test.Sdk'")
            });

        public static NugetPackageInfo Xunit(IOutputTarget outputTarget) => new NugetPackageInfo(
            name: "xunit",
            version: outputTarget.GetMaxNetAppVersion() switch
            {
                (>= 0, 0) => "2.9.0",
                _ => throw new Exception($"Unsupported Framework `{outputTarget.GetMaxNetAppVersion().Major}` for NuGet package 'xunit'")
            });

        public static NugetPackageInfo XunitRunnerVisualstudio(IOutputTarget outputTarget) => new NugetPackageInfo(
            name: "xunit.runner.visualstudio",
            version: outputTarget.GetMaxNetAppVersion() switch
            {
                (>= 6, 0) => "2.8.2",
                _ => throw new Exception($"Unsupported Framework `{outputTarget.GetMaxNetAppVersion().Major}` for NuGet package 'xunit.runner.visualstudio'")
            });

        public static NugetPackageInfo TestcontainersCosmosDb(IOutputTarget outputTarget) => new NugetPackageInfo(
            name: "Testcontainers.CosmosDb",
            version: outputTarget.GetMaxNetAppVersion() switch
            {
                (>= 8, 0) => "3.9.0",
                (>= 6, 0) => "3.9.0",
                _ => throw new Exception($"Unsupported Framework `{outputTarget.GetMaxNetAppVersion().Major}` for NuGet package 'Testcontainers.CosmosDb'")
            });

        public static NugetPackageInfo TestcontainersMsSql(IOutputTarget outputTarget) => new NugetPackageInfo(
            name: "Testcontainers.MsSql",
            version: outputTarget.GetMaxNetAppVersion() switch
            {
                (>= 8, 0) => "3.9.0",
                (>= 6, 0) => "3.9.0",
                _ => throw new Exception($"Unsupported Framework `{outputTarget.GetMaxNetAppVersion().Major}` for NuGet package 'Testcontainers.MsSql'")
            });

        public static NugetPackageInfo TestcontainersPostgreSql(IOutputTarget outputTarget) => new NugetPackageInfo(
            name: "Testcontainers.PostgreSql",
            version: outputTarget.GetMaxNetAppVersion() switch
            {
                (>= 8, 0) => "3.9.0",
                (>= 6, 0) => "3.9.0",
                _ => throw new Exception($"Unsupported Framework `{outputTarget.GetMaxNetAppVersion().Major}` for NuGet package 'Testcontainers.PostgreSql'")
            });

        public static NugetPackageInfo TestcontainersMongoDb(IOutputTarget outputTarget) => new NugetPackageInfo(
            name: "Testcontainers.MongoDb",
            version: outputTarget.GetMaxNetAppVersion() switch
            {
                (>= 8, 0) => "3.9.0",
                (>= 6, 0) => "3.9.0",
                _ => throw new Exception($"Unsupported Framework `{outputTarget.GetMaxNetAppVersion().Major}` for NuGet package 'Testcontainers.MongoDb'")
            });

        public static NugetPackageInfo IEvangelistAzureCosmosRepository(IOutputTarget outputTarget) => new NugetPackageInfo(
            name: "IEvangelist.Azure.CosmosRepository",
            version: outputTarget.GetMaxNetAppVersion() switch
            {
                (>= 8, 0) => "8.1.7",
                (>= 7, 0) => "8.1.7",
                _ => throw new Exception($"Unsupported Framework `{outputTarget.GetMaxNetAppVersion().Major}` for NuGet package 'IEvangelist.Azure.CosmosRepository'")
            });

        public static NugetPackageInfo MicrosoftAspNetCoreMvcTesting(IOutputTarget outputTarget) => new NugetPackageInfo(
            name: "Microsoft.AspNetCore.Mvc.Testing",
            version: outputTarget.GetMaxNetAppVersion() switch
            {
                (>= 8, 0) => "8.0.7",
                (>= 7, 0) => "7.0.20",
                (>= 6, 0) => "6.0.32",
                _ => throw new Exception($"Unsupported Framework `{outputTarget.GetMaxNetAppVersion().Major}` for NuGet package 'Microsoft.AspNetCore.Mvc.Testing'")
            });
    }
}
