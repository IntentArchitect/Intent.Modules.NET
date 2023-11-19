using System;
using Intent.Engine;
using Intent.Modules.Common.CSharp.VisualStudio;
using Intent.Modules.Common.VisualStudio;

namespace Intent.Modules.Infrastructure.DependencyInjection
{
    public static class NugetPackages
    {
        public static INugetPackageInfo MicrosoftExtensionsConfigurationAbstractions(IOutputTarget outputTarget)
        {
            var project = outputTarget.GetProject();
            var version = project switch
            {
                _ when project.IsNetApp(5) => "5.0.0",
                _ when project.IsNetApp(6) => "6.0.0",
                _ when project.IsNetApp(7) => "7.0.0",
                _ when project.IsNetApp(8) => "8.0.0",
                _ => "6.0.0"
            };

            return new NugetPackageInfo("Microsoft.Extensions.Configuration.Abstractions", version);
        }

        public static NugetPackageInfo MicrosoftExtensionsDependencyInjection(IOutputTarget outputTarget)
        {
            var project = outputTarget.GetProject();
            var version = project switch
            {
                _ when project.IsNetApp(5) => "5.0.2",
                _ when project.IsNetApp(6) => "6.0.1",
                _ when project.IsNetApp(7) => "7.0.0",
                _ when project.IsNetApp(8) => "8.0.0",
                _ => "6.0.1"
            };

            return new NugetPackageInfo("Microsoft.Extensions.DependencyInjection", version);
        }
        
        public static NugetPackageInfo MicrosoftExtensionsConfigurationBinder(IOutputTarget outputTarget)
        {
            var project = outputTarget.GetProject();
            var version = project switch
            {
                _ when project.IsNetApp(5) => "5.0.0",
                _ when project.IsNetApp(6) => "6.0.0",
                _ when project.IsNetApp(7) => "7.0.0",
                _ when project.IsNetApp(8) => "8.0.0",
                _ => "6.0.0"
            };

            return new NugetPackageInfo("Microsoft.Extensions.Configuration.Binder", version);
        }
    }
}
