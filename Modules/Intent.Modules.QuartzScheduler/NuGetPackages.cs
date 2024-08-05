using Intent.Engine;
using Intent.Modules.Common.VisualStudio;

namespace Intent.Modules.QuartzScheduler
{
    public static class NugetPackages
    {

        public static NugetPackageInfo QuartzExtensionsHosting(IOutputTarget outputTarget) => new(
            name: "Quartz.Extensions.Hosting",
            version: outputTarget.GetMaxNetAppVersion() switch
            {
                (6, 0) => "3.12.0",
                _ => "3.12.0",
            });

        public static NugetPackageInfo QuartzAspNetCore(IOutputTarget outputTarget) => new(
            name: "Quartz.AspNetCore",
            version: outputTarget.GetMaxNetAppVersion() switch
            {
                (6, 0) => "3.12.0",
                _ => "3.12.0",
            });
    }
}
