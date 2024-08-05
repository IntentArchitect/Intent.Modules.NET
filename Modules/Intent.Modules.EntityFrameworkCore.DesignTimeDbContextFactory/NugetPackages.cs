using System;
using Intent.Engine;
using Intent.Modules.Common.VisualStudio;

namespace Intent.Modules.EntityFrameworkCore.DesignTimeDbContextFactory
{
    public static class NugetPackages
    {

        public static NugetPackageInfo MicrosoftExtensionsConfigurationFileExtensions(IOutputTarget outputTarget) => new NugetPackageInfo(
            name: "Microsoft.Extensions.Configuration.FileExtensions",
            version: outputTarget.GetMaxNetAppVersion() switch
            {
                (>= 8, 0) => "8.0.1",
                (>= 7, 0) => "8.0.1",
                (>= 6, 0) => "6.0.0",
                _ => throw new Exception($"Unsupported Framework `{outputTarget.GetMaxNetAppVersion().Major}` for NuGet package 'Microsoft.Extensions.Configuration.FileExtensions'")
            });

        public static NugetPackageInfo MicrosoftExtensionsConfigurationJson(IOutputTarget outputTarget) => new NugetPackageInfo(
            name: "Microsoft.Extensions.Configuration.Json",
            version: outputTarget.GetMaxNetAppVersion() switch
            {
                (>= 8, 0) => "8.0.0",
                (>= 7, 0) => "8.0.0",
                (>= 6, 0) => "6.0.0",
                _ => throw new Exception($"Unsupported Framework `{outputTarget.GetMaxNetAppVersion().Major}` for NuGet package 'Microsoft.Extensions.Configuration.Json'")
            });

        public static NugetPackageInfo MicrosoftExtensionsConfigurationUserSecrets(IOutputTarget outputTarget) => new NugetPackageInfo(
            name: "Microsoft.Extensions.Configuration.UserSecrets",
            version: outputTarget.GetMaxNetAppVersion() switch
            {
                (>= 8, 0) => "8.0.0",
                (>= 7, 0) => "8.0.0",
                (>= 6, 0) => "6.0.1",
                _ => throw new Exception($"Unsupported Framework `{outputTarget.GetMaxNetAppVersion().Major}` for NuGet package 'Microsoft.Extensions.Configuration.UserSecrets'")
            });

        public static NugetPackageInfo MicrosoftExtensionsConfigurationEnvironmentVariables(IOutputTarget outputTarget) => new NugetPackageInfo(
            name: "Microsoft.Extensions.Configuration.EnvironmentVariables",
            version: outputTarget.GetMaxNetAppVersion() switch
            {
                (>= 8, 0) => "8.0.0",
                (>= 7, 0) => "8.0.0",
                (>= 6, 0) => "6.0.1",
                _ => throw new Exception($"Unsupported Framework `{outputTarget.GetMaxNetAppVersion().Major}` for NuGet package 'Microsoft.Extensions.Configuration.EnvironmentVariables'")
            });
    }
}
