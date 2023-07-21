using Intent.Modules.Common.VisualStudio;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Intent.Modules.Blazor.HttpClients.AccountController.Templates
{
    public static class NuGetPackages
    {
        public static readonly INugetPackageInfo MicrosoftExtensionsHttp = new NugetPackageInfo("Microsoft.Extensions.Http", "6.0.0");
        public static readonly INugetPackageInfo MicrosoftAspNetCoreWebUtilities = new NugetPackageInfo("Microsoft.AspNetCore.WebUtilities", "2.2.0");
        public static readonly INugetPackageInfo MicrosoftAspNetCoreComponentsWebAssemblyAuthentication = new NugetPackageInfo("Microsoft.AspNetCore.Components.WebAssembly.Authentication", "6.0.20");
        
    }
}
