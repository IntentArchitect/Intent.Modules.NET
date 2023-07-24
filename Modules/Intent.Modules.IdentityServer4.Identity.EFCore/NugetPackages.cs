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
            return project switch
            {
                _ when project.IsNetCore2App() => "2.1.14",
                _ when project.IsNetCore3App() => "3.1.15",
                _ when project.IsNetApp(5) => "5.0.17",
                _ when project.IsNetApp(6) => "6.0.20",
                _ when project.IsNetApp(7) => "7.0.9",
                _ => "5.0.6"
            };
        }
    }
} 
