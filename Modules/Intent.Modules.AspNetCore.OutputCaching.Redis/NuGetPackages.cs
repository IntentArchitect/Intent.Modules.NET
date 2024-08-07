using System;
using Intent.Engine;
using Intent.Modules.Common.VisualStudio;

namespace Intent.Modules.AspNetCore.OutputCaching.Redis
{
    public static class NugetPackages
    {

        public static NugetPackageInfo MicrosoftAspNetCoreOutputCachingStackExchangeRedis(IOutputTarget outputTarget) => new NugetPackageInfo(
            name: "Microsoft.AspNetCore.OutputCaching.StackExchangeRedis",
            version: outputTarget.GetMaxNetAppVersion() switch
            {
                (>= 8, 0) => "8.0.7",
                _ => throw new Exception($"Unsupported Framework `{outputTarget.GetMaxNetAppVersion().Major}` for NuGet package 'Microsoft.AspNetCore.OutputCaching.StackExchangeRedis'")
            });
    }
}
