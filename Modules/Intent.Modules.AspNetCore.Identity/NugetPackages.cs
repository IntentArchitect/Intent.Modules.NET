using Intent.Engine;
using Intent.Modules.Common.VisualStudio;

namespace Intent.Modules.AspNetCore.Identity
{
    public static class NugetPackages
    {

        public static NugetPackageInfo MicrosoftAspNetCoreIdentityEntityFrameworkCore(IOutputTarget outputTarget) => new(
            name: "Microsoft.AspNetCore.Identity.EntityFrameworkCore",
            version: outputTarget.GetMaxNetAppVersion() switch
            {
                (6, 0) => "6.0.32",
                (7, 0) => "7.0.20",
                _ => "8.0.7",
            });

        public static NugetPackageInfo MicrosoftExtensionsIdentityStores(IOutputTarget outputTarget) => new(
            name: "Microsoft.Extensions.Identity.Stores",
            version: outputTarget.GetMaxNetAppVersion() switch
            {
                (6, 0) => "6.0.32",
                (7, 0) => "7.0.20",
                _ => "8.0.7",
            });
    }
}
