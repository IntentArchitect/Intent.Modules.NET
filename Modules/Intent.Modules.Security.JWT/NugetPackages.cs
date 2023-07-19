using Intent.Modules.Common.VisualStudio;
using System;
using System.Collections.Generic;
using System.Text;
using Intent.Engine;
using Intent.Modules.Common.CSharp.VisualStudio;

namespace Intent.Modules.Security.JWT
{
    public static class NugetPackages
    {
        public static NugetPackageInfo MicrosoftAspNetCoreAuthenticationJwtBearer(IOutputTarget outputTarget) => new ("Microsoft.AspNetCore.Authentication.JwtBearer", GetJwtVersion(outputTarget.GetProject()));
        public static readonly NugetPackageInfo IdentityModel = new NugetPackageInfo("IdentityModel", "6.0.0");
        
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
