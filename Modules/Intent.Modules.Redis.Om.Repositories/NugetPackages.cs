using System;
using Intent.Engine;
using Intent.Modules.Common.VisualStudio;

namespace Intent.Modules.Redis.Om.Repositories
{
    public static class NugetPackages
    {

        public static NugetPackageInfo RedisOM(IOutputTarget outputTarget) => new NugetPackageInfo(
            name: "Redis.OM",
            version: outputTarget.GetMaxNetAppVersion() switch
            {
                (>= 2, 0) => "0.7.4",
                _ => throw new Exception($"Unsupported Framework `{outputTarget.GetMaxNetAppVersion().Major}` for NuGet package 'Redis.OM'")
            });

        public static NugetPackageInfo MicrosoftExtensionsHostingAbstractions(IOutputTarget outputTarget) => new NugetPackageInfo(
            name: "Microsoft.Extensions.Hosting.Abstractions",
            version: outputTarget.GetMaxNetAppVersion() switch
            {
                (>= 8, 0) => "8.0.0",
                (>= 7, 0) => "8.0.0",
                (>= 6, 0) => "8.0.0",
                _ => throw new Exception($"Unsupported Framework `{outputTarget.GetMaxNetAppVersion().Major}` for NuGet package 'Microsoft.Extensions.Hosting.Abstractions'")
            });
    }
}
