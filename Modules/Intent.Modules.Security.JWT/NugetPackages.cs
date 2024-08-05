using Intent.Engine;
using Intent.Modules.Common.VisualStudio;

namespace Intent.Modules.Security.JWT
{
    public static class NugetPackages
    {

        public static NugetPackageInfo MicrosoftAspNetCoreAuthenticationJwtBearer(IOutputTarget outputTarget) => new(
            name: "Microsoft.AspNetCore.Authentication.JwtBearer",
            version: outputTarget.GetMaxNetAppVersion() switch
            {
                (6, 0) => "6.0.32",
                (7, 0) => "7.0.20",
                _ => "8.0.7",
            });

        public static NugetPackageInfo IdentityModel(IOutputTarget outputTarget) => new(
            name: "IdentityModel",
            version: outputTarget.GetMaxNetAppVersion() switch
            {
                _ => "7.0.0",
            });
    }
}
