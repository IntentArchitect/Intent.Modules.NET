using Intent.Modules.Common.VisualStudio;
using System;
using System.Collections.Generic;

namespace Intent.Modules.IdentityServer4.Identity.EntityFrameworkCore
{
    public static class NugetPackages
    {
        public static readonly NugetPackageInfo IdentityServer4EntityFramework = new NugetPackageInfo("IdentityServer4.EntityFramework", "4.1.0");
        public static readonly NugetPackageInfo MicrosoftAspNetCoreIdentityEntityFrameworkCore = new NugetPackageInfo("Microsoft.AspNetCore.Identity.EntityFrameworkCore", "3.1.13");
    }
}
