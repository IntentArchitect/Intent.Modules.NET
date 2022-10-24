using Intent.Modules.Common.VisualStudio;
using System;
using System.Collections.Generic;
using System.Text;

namespace Intent.Modules.Security.MSAL
{
    public static class NugetPackages
    {
        public static readonly NugetPackageInfo MicrosoftAspNetCoreAuthenticationJwtBearer = new NugetPackageInfo("Microsoft.AspNetCore.Authentication.JwtBearer", "6.0.3");
        public static readonly NugetPackageInfo MicrosoftAspNetCoreAuthenticationOpenIdConnect = new NugetPackageInfo("Microsoft.AspNetCore.Authentication.OpenIdConnect", "6.0.3");
        public static readonly NugetPackageInfo MicrosoftIdentityWeb = new NugetPackageInfo("Microsoft.Identity.Web", "1.16.0");
        public static readonly NugetPackageInfo MicrosoftIdentityWebMicrosoftGraph = new NugetPackageInfo("Microsoft.Identity.Web.MicrosoftGraph", "1.16.0");
        public static readonly NugetPackageInfo MicrosoftIdentityWebUI = new NugetPackageInfo("Microsoft.Identity.Web.UI", "1.16.0");
    }
}
