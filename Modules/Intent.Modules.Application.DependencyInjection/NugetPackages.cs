using System;
using Intent.Engine;
using Intent.Modules.Common.VisualStudio;

namespace Intent.Modules.Application.DependencyInjection
{
    public static class NugetPackages
    {

        public static NugetPackageInfo MicrosoftExtensionsConfigurationAbstractions(IOutputTarget outputTarget) => new NugetPackageInfo(
            name: "Microsoft.Extensions.Configuration.Abstractions",
            version: outputTarget.GetMaxNetAppVersion() switch
            {
                (>= 8, 0) => "8.0.0",
                (>= 7, 0) => "8.0.0",
                (>= 6, 0) => "6.0.0",
                _ => throw new Exception($"Unsupported Framework `{outputTarget.GetMaxNetAppVersion().Major}` for NuGet package 'Microsoft.Extensions.Configuration.Abstractions'")
            });

        public static NugetPackageInfo MicrosoftExtensionsDependencyInjection(IOutputTarget outputTarget) => new NugetPackageInfo(
            name: "Microsoft.Extensions.DependencyInjection",
            version: outputTarget.GetMaxNetAppVersion() switch
            {
                (>= 8, 0) => "8.0.0",
                (>= 7, 0) => "8.0.0",
                (>= 6, 0) => "6.0.1",
                _ => throw new Exception($"Unsupported Framework `{outputTarget.GetMaxNetAppVersion().Major}` for NuGet package 'Microsoft.Extensions.DependencyInjection'")
            });

        public static NugetPackageInfo MicrosoftExtensionsConfigurationBinder(IOutputTarget outputTarget) => new NugetPackageInfo(
            name: "Microsoft.Extensions.Configuration.Binder",
            version: outputTarget.GetMaxNetAppVersion() switch
            {
                (>= 8, 0) => "8.0.2",
                (>= 7, 0) => "8.0.2",
                (>= 6, 0) => "6.0.0",
                _ => throw new Exception($"Unsupported Framework `{outputTarget.GetMaxNetAppVersion().Major}` for NuGet package 'Microsoft.Extensions.Configuration.Binder'")
            });
    }
}
