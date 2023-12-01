using Intent.Engine;
using Intent.Modules.Common.VisualStudio;

namespace Intent.Modules.Security.MSAL;

public static class NugetPackages
{
    public static NugetPackageInfo MicrosoftAspNetCoreAuthenticationOpenIdConnect(IOutputTarget outputTarget) => new(
        name: "Microsoft.AspNetCore.Authentication.OpenIdConnect",
        version: outputTarget.GetMaxNetAppVersion() switch
        {
            (5, 0) => "5.0.17",
            (6, 0) => "6.0.25",
            (7, 0) => "7.0.14",
            _ => "8.0.0"
        });

    public static NugetPackageInfo MicrosoftAspNetCoreAuthenticationJwtBearer(IOutputTarget outputTarget) => new(
        name: "Microsoft.AspNetCore.Authentication.JwtBearer",
        version: outputTarget.GetMaxNetAppVersion() switch
        {
            (5, 0) => "5.0.17",
            (6, 0) => "6.0.25",
            (7, 0) => "7.0.14",
            _ => "8.0.0"
        });

    public static readonly NugetPackageInfo MicrosoftIdentityWeb = new("Microsoft.Identity.Web", "1.16.0");
    public static readonly NugetPackageInfo MicrosoftIdentityWebMicrosoftGraph = new("Microsoft.Identity.Web.MicrosoftGraph", "1.16.0");
    public static readonly NugetPackageInfo MicrosoftIdentityWebUI = new("Microsoft.Identity.Web.UI", "1.16.0");
    public static readonly NugetPackageInfo IdentityModel = new("IdentityModel", "6.0.0");
}