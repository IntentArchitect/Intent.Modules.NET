using Intent.Modules.Common.VisualStudio;
using System;
using System.Collections.Generic;
using System.Text;
using Intent.Engine;
using Intent.Modules.Common.CSharp.VisualStudio;

namespace Intent.Modules.Security.MSAL
{
    public static class NugetPackages
    {
        public static readonly NugetPackageInfo MicrosoftAspNetCoreAuthenticationOpenIdConnect = new NugetPackageInfo("Microsoft.AspNetCore.Authentication.OpenIdConnect", "6.0.3");
        public static readonly NugetPackageInfo MicrosoftIdentityWeb = new NugetPackageInfo("Microsoft.Identity.Web", "1.16.0");
        public static readonly NugetPackageInfo MicrosoftIdentityWebMicrosoftGraph = new NugetPackageInfo("Microsoft.Identity.Web.MicrosoftGraph", "1.16.0");
        public static readonly NugetPackageInfo MicrosoftIdentityWebUI = new NugetPackageInfo("Microsoft.Identity.Web.UI", "1.16.0");
        public static readonly NugetPackageInfo IdentityModel = new NugetPackageInfo("IdentityModel", "6.0.0");
        
        public static NugetPackageInfo MicrosoftAspNetCoreAuthenticationJwtBearer(IOutputTarget outputTarget) => new ("Microsoft.AspNetCore.Authentication.JwtBearer", GetJwtVersion(outputTarget.GetProject()));
        
        private static string GetJwtVersion(ICSharpProject project)
        {
            return project switch
            {
                _ when project.IsNetApp(5) => "5.0.17",
                _ when project.IsNetApp(6) => "6.0.20",
                _ when project.IsNetApp(7) => "7.0.9",
                _ when project.IsNetApp(8) => "8.0.0-preview.6.23329.11",
                _ => throw new Exception("Not supported version of .NET") 
            };
        }
    }
}
