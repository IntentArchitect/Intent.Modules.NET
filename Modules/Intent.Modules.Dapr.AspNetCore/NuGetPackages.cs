using System;
using Intent.Engine;
using Intent.Modules.Common.VisualStudio;

namespace Intent.Modules.Dapr.AspNetCore
{
    public static class NugetPackages
    {

        public static NugetPackageInfo DaprAspNetCore(IOutputTarget outputTarget) => new NugetPackageInfo(
            name: "Dapr.AspNetCore",
            version: outputTarget.GetMaxNetAppVersion() switch
            {
                (>= 8, 0) => "1.13.1",
                (>= 6, 0) => "1.13.1",
                _ => throw new Exception($"Unsupported Framework `{outputTarget.GetMaxNetAppVersion().Major}` for NuGet package 'Dapr.AspNetCore'")
            });

        public static NugetPackageInfo DaprClient(IOutputTarget outputTarget) => new NugetPackageInfo(
            name: "Dapr.Client",
            version: outputTarget.GetMaxNetAppVersion() switch
            {
                (>= 8, 0) => "1.13.1",
                (>= 6, 0) => "1.13.1",
                _ => throw new Exception($"Unsupported Framework `{outputTarget.GetMaxNetAppVersion().Major}` for NuGet package 'Dapr.Client'")
            });

        public static NugetPackageInfo MediatR(IOutputTarget outputTarget) => new NugetPackageInfo(
            name: "MediatR",
            version: outputTarget.GetMaxNetAppVersion() switch
            {
                (>= 7, 0) => "12.4.0",
                (>= 6, 0) => "12.1.1",
                _ => throw new Exception($"Unsupported Framework `{outputTarget.GetMaxNetAppVersion().Major}` for NuGet package 'MediatR'")
            });

        public static NugetPackageInfo ManDaprSidekickAspNetCore(IOutputTarget outputTarget) => new NugetPackageInfo(
            name: "Man.Dapr.Sidekick.AspNetCore",
            version: outputTarget.GetMaxNetAppVersion() switch
            {
                (>= 2, 0) => "1.2.2",
                _ => throw new Exception($"Unsupported Framework `{outputTarget.GetMaxNetAppVersion().Major}` for NuGet package 'Man.Dapr.Sidekick.AspNetCore'")
            });

        public static NugetPackageInfo DaprExtensionsConfiguration(IOutputTarget outputTarget) => new NugetPackageInfo(
            name: "Dapr.Extensions.Configuration",
            version: outputTarget.GetMaxNetAppVersion() switch
            {
                (>= 8, 0) => "1.13.1",
                (>= 6, 0) => "1.13.1",
                _ => throw new Exception($"Unsupported Framework `{outputTarget.GetMaxNetAppVersion().Major}` for NuGet package 'Dapr.Extensions.Configuration'")
            });
    }
}
