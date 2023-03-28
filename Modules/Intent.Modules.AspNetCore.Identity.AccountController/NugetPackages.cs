using Intent.Modules.Common.VisualStudio;

namespace Intent.Modules.AspNetCore.Identity.AccountController
{
    public static class NugetPackages
    {
        public static readonly NugetPackageInfo MicrosoftAspNetCoreAuthenticationJwtBearer = new NugetPackageInfo("Microsoft.AspNetCore.Authentication.JwtBearer", "3.1.9");
        public static readonly NugetPackageInfo IdentityModel = new NugetPackageInfo("IdentityModel", "6.0.0");
    }
}
