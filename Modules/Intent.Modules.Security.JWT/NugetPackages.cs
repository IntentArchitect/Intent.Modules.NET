using Intent.Modules.Common.VisualStudio;
using System;
using System.Collections.Generic;
using System.Text;

namespace Intent.Modules.Security.JWT
{
    public static class NugetPackages
    {
        public static readonly NugetPackageInfo MicrosoftAspNetCoreAuthenticationJwtBearer = new NugetPackageInfo("Microsoft.AspNetCore.Authentication.JwtBearer", "3.1.9");
        public static readonly NugetPackageInfo IdentityModel = new NugetPackageInfo("IdentityModel", "6.0.0");
    }
}
