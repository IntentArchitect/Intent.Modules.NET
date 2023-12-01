using Intent.Modules.Common.CSharp.VisualStudio;
using Intent.Modules.Common.VisualStudio;

namespace Intent.Modules.IdentityServer4.Identity.EFCore;

public static class NugetPackages
{
    public static readonly NugetPackageInfo IdentityServer4EntityFramework = new("IdentityServer4.EntityFramework", "4.1.2");
    public static readonly NugetPackageInfo IdentityServer4AspNetIdentity = new("IdentityServer4.AspNetIdentity", "4.1.2");
    public static readonly NugetPackageInfo Automapper = new("Automapper", "12.0.1");
    public static NugetPackageInfo MicrosoftAspNetCoreIdentityEntityFrameworkCore(ICSharpProject project) => new(
        name: "Microsoft.AspNetCore.Identity.EntityFrameworkCore",
        version: project.GetMaxNetAppVersion() switch
        {
            (5, 0) => "5.0.17",
            (6, 0) => "6.0.25",
            (7, 0) => "7.0.14",
            _ => "8.0.0"
        });
}