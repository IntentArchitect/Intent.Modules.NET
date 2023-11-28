using Intent.Modules.Common.CSharp.VisualStudio;
using Intent.Modules.Common.VisualStudio;

namespace Intent.Modules.AspNetCore.Identity;

public static class NugetPackages
{
    public static NugetPackageInfo MicrosoftAspNetCoreIdentityEntityFrameworkCore(ICSharpProject project) => new(
        "Microsoft.AspNetCore.Identity.EntityFrameworkCore",
        version: project.GetMaxNetAppVersion() switch
        {
            (5, 0) => "5.0.17",
            (6, 0) => "6.0.25",
            (7, 0) => "7.0.14",
            _ => "8.0.0"
        });

    public static NugetPackageInfo MicrosoftExtensionsIdentityStores(ICSharpProject project) => new(
        "Microsoft.Extensions.Identity.Stores",
        version: project.GetMaxNetAppVersion() switch
        {
            (5, 0) => "5.0.17",
            (6, 0) => "6.0.25",
            (7, 0) => "7.0.14",
            _ => "8.0.0"
        });
}