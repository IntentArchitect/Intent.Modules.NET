using Intent.Modules.Common.VisualStudio;
using System;
using System.Collections.Generic;
using Intent.Modules.Common.CSharp.VisualStudio;

namespace Intent.Modules.IdentityServer4.Identity.EFCore
{
    public static class NugetPackages
    {
        public static readonly NugetPackageInfo IdentityServer4EntityFramework = new NugetPackageInfo("IdentityServer4.EntityFramework", "4.1.2");
        public static readonly NugetPackageInfo IdentityServer4AspNetIdentity = new NugetPackageInfo("IdentityServer4.AspNetIdentity", "4.1.2");
        public static readonly NugetPackageInfo Automapper = new NugetPackageInfo("Automapper", "12.0.0");
        public static NugetPackageInfo MicrosoftAspNetCoreIdentityEntityFrameworkCore(ICSharpProject project) => new NugetPackageInfo("Microsoft.AspNetCore.Identity.EntityFrameworkCore", GetVersion(project));

        private static string GetVersion(ICSharpProject project)
        {
            if (project.IsNetCore2App())
            {
                return "2.1.14";
            }
            if (project.IsNetCore3App())
            {
                return "3.1.15";
            }
            if (project.IsNet5App())
            {
                return "5.0.6";
            }
            if (project.IsNet6App())
            {
                return "6.0.1";
            }
            return "5.0.6";
        }
    }
} 
