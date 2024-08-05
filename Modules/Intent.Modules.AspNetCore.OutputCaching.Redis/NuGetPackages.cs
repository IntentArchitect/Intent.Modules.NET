using Intent.Engine;
using Intent.Modules.Common.VisualStudio;

namespace Intent.Modules.AspNetCore.OutputCaching.Redis
{
    public static class NugetPackages
    {

        public static NugetPackageInfo MicrosoftAspNetCoreOutputCachingStackExchangeRedis(IOutputTarget outputTarget) => new(
            name: "Microsoft.AspNetCore.OutputCaching.StackExchangeRedis",
            version: outputTarget.GetMaxNetAppVersion() switch
            {
                _ => "8.0.7",
            });
    }
}
