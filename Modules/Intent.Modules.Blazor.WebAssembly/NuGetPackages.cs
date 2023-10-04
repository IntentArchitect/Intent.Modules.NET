using Intent.Engine;
using Intent.Modules.Common.CSharp.VisualStudio;
using Intent.Modules.Common.VisualStudio;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Intent.Modules.Blazor.WebAssembly
{
    public static class NuGetPackages
    {
        public static INugetPackageInfo MicrosoftAspNetCoreComponentsWebAssembly(IOutputTarget outputTarget)
        {
            var project = outputTarget.GetProject();
            var version = project switch
            {
                _ when project.IsNetApp(5) => "5.0.0",
                _ when project.IsNetApp(6) => "6.0.20",
                _ when project.IsNetApp(7) => "7.0.3",
                _ => "6.0.20"
            };

            return new NugetPackageInfo("Microsoft.AspNetCore.Components.WebAssembly", version);
        }

        public static INugetPackageInfo MicrosoftAspNetCoreComponentsWebAssemblyDevServer(IOutputTarget outputTarget)
        {
            var project = outputTarget.GetProject();
            var version = project switch
            {
                _ when project.IsNetApp(5) => "5.0.0",
                _ when project.IsNetApp(6) => "6.0.20",
                _ when project.IsNetApp(7) => "7.0.3",
                _ => "6.0.20"
            };

            return new NugetPackageInfo("Microsoft.AspNetCore.Components.WebAssembly.DevServer", version)
            .SpecifyAssetsBehaviour(new[] { "all" }, new string[0]);
        }
    }
}
