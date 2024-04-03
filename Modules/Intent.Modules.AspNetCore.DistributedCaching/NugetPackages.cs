using Intent.Engine;
using Intent.Modules.Common.VisualStudio;

namespace Intent.Modules.AspNetCore.DistributedCaching
{
    internal class NugetPackages
    {
        public static INugetPackageInfo MicrosoftExtensionsCachingAbstractions(IOutputTarget outputTarget) => new NugetPackageInfo(
            name: "Microsoft.Extensions.Caching.Abstractions",
            version: outputTarget.GetMaxNetAppVersion() switch
            {
                (5, 0) => "5.0.0",
                (6, 0) => "6.0.0",
                (7, 0) => "7.0.0",
                _ => "8.0.0"
            });

        public static INugetPackageInfo MicrosoftExtensionsCachingStackExchangeRedis(IOutputTarget outputTarget) => new NugetPackageInfo(
            name: "Microsoft.Extensions.Caching.StackExchangeRedis",
            version: outputTarget.GetMaxNetAppVersion() switch
            {
                (5, 0) => "5.0.1",
                (6, 0) => "6.0.28",
                (7, 0) => "7.0.17",
                _ => "8.0.3"
            });
    }
}
