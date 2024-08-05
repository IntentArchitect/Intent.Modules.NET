using Intent.Engine;
using Intent.Modules.Common.VisualStudio;

namespace Intent.Modules.Security.MSAL
{
    public static class NugetPackages
    {

        public static NugetPackageInfo MicrosoftAspNetCoreAuthenticationOpenIdConnect(IOutputTarget outputTarget) => new(
            name: "Microsoft.AspNetCore.Authentication.OpenIdConnect",
            version: outputTarget.GetMaxNetAppVersion() switch
            {
                (6, 0) => "6.0.32",
                (7, 0) => "7.0.20",
                _ => "8.0.7",
            });

        public static NugetPackageInfo MicrosoftAspNetCoreAuthenticationJwtBearer(IOutputTarget outputTarget) => new(
            name: "Microsoft.AspNetCore.Authentication.JwtBearer",
            version: outputTarget.GetMaxNetAppVersion() switch
            {
                (6, 0) => "6.0.32",
                (7, 0) => "7.0.20",
                _ => "8.0.7",
            });

        public static NugetPackageInfo MicrosoftIdentityWeb(IOutputTarget outputTarget) => new(
            name: "Microsoft.Identity.Web",
            version: outputTarget.GetMaxNetAppVersion() switch
            {
                (6, 0) => "3.0.1",
                (7, 0) => "3.0.1",
                _ => "3.0.1",
            });

        public static NugetPackageInfo MicrosoftIdentityWebMicrosoftGraph(IOutputTarget outputTarget) => new(
            name: "Microsoft.Identity.Web.MicrosoftGraph",
            version: outputTarget.GetMaxNetAppVersion() switch
            {
                (6, 0) => "3.0.1",
                (7, 0) => "3.0.1",
                _ => "3.0.1",
            });

        public static NugetPackageInfo MicrosoftIdentityWebUI(IOutputTarget outputTarget) => new(
            name: "Microsoft.Identity.Web.UI",
            version: outputTarget.GetMaxNetAppVersion() switch
            {
                (6, 0) => "3.0.1",
                (7, 0) => "3.0.1",
                _ => "3.0.1",
            });

        public static NugetPackageInfo IdentityModel(IOutputTarget outputTarget) => new(
            name: "IdentityModel",
            version: outputTarget.GetMaxNetAppVersion() switch
            {
                _ => "7.0.0",
            });
    }
}
