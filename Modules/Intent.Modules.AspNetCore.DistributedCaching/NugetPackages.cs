using Intent.Engine;
using Intent.Modules.Common.VisualStudio;

namespace Intent.Modules.AspNetCore.DistributedCaching
{
    public static class NugetPackages
    {

        public static NugetPackageInfo MicrosoftExtensionsCachingAbstractions(IOutputTarget outputTarget) => new(
            name: "Microsoft.Extensions.Caching.Abstractions",
            version: outputTarget.GetMaxNetAppVersion() switch
            {
                (6, 0) => "8.0.0",
                (7, 0) => "8.0.0",
                _ => "8.0.0",
            });

        public static NugetPackageInfo MicrosoftExtensionsCachingStackExchangeRedis(IOutputTarget outputTarget) => new(
            name: "Microsoft.Extensions.Caching.StackExchangeRedis",
            version: outputTarget.GetMaxNetAppVersion() switch
            {
                (6, 0) => "6.0.32",
                (7, 0) => "7.0.20",
                _ => "8.0.7",
            });
    }
}
