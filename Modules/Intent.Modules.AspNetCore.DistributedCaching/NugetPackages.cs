using System;
using Intent.Engine;
using Intent.Modules.Common.VisualStudio;

namespace Intent.Modules.AspNetCore.DistributedCaching
{
    public static class NugetPackages
    {

        public static NugetPackageInfo MicrosoftExtensionsCachingAbstractions(IOutputTarget outputTarget) => new NugetPackageInfo(
            name: "Microsoft.Extensions.Caching.Abstractions",
            version: outputTarget.GetMaxNetAppVersion() switch
            {
                (>= 8, 0) => "8.0.0",
                (>= 7, 0) => "8.0.0",
                (>= 6, 0) => "8.0.0",
                _ => throw new Exception($"Unsupported Framework `{outputTarget.GetMaxNetAppVersion().Major}` for NuGet package 'Microsoft.Extensions.Caching.Abstractions'")
            });

        public static NugetPackageInfo MicrosoftExtensionsCachingStackExchangeRedis(IOutputTarget outputTarget) => new NugetPackageInfo(
            name: "Microsoft.Extensions.Caching.StackExchangeRedis",
            version: outputTarget.GetMaxNetAppVersion() switch
            {
                (>= 8, 0) => "8.0.7",
                (>= 7, 0) => "7.0.20",
                (>= 6, 0) => "6.0.32",
                _ => throw new Exception($"Unsupported Framework `{outputTarget.GetMaxNetAppVersion().Major}` for NuGet package 'Microsoft.Extensions.Caching.StackExchangeRedis'")
            });
    }
}
