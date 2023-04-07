using Intent.Engine;
using Intent.Modules.Common.CSharp.VisualStudio;
using Intent.Modules.Common.VisualStudio;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Intent.Modules.Infrastructure.DependencyInjection
{
    public static class NugetPackages
    {
        public static INugetPackageInfo MicrosoftExtensionsConfigurationAbstractions(IOutputTarget outputTarget) => new NugetPackageInfo("Microsoft.Extensions.Configuration.Abstractions", GetVersion(outputTarget.GetProject()));

        private static string GetVersion(ICSharpProject project)
        {
            return project switch
            {
                _ when project.IsNetCore2App() => throw new Exception(".NET Core 2.x is no longer supported."),
                _ when project.IsNetCore3App() => "3.0.0",
                _ when project.IsNetApp(5) => "5.0.0",
                _ when project.IsNetApp(6) => "6.0.0",
                _ when project.IsNetApp(7) => "7.0.0",
                _ => "6.0.0"
            };
        }

    }
}
