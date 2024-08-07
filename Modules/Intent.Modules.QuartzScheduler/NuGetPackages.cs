using System;
using Intent.Engine;
using Intent.Modules.Common.VisualStudio;

namespace Intent.Modules.QuartzScheduler
{
    public static class NugetPackages
    {

        public static NugetPackageInfo QuartzExtensionsHosting(IOutputTarget outputTarget) => new NugetPackageInfo(
            name: "Quartz.Extensions.Hosting",
            version: outputTarget.GetMaxNetAppVersion() switch
            {
                (>= 8, 0) => "3.12.0",
                (>= 6, 0) => "3.12.0",
                _ => throw new Exception($"Unsupported Framework `{outputTarget.GetMaxNetAppVersion().Major}` for NuGet package 'Quartz.Extensions.Hosting'")
            });

        public static NugetPackageInfo QuartzAspNetCore(IOutputTarget outputTarget) => new NugetPackageInfo(
            name: "Quartz.AspNetCore",
            version: outputTarget.GetMaxNetAppVersion() switch
            {
                (>= 8, 0) => "3.12.0",
                (>= 6, 0) => "3.12.0",
                _ => throw new Exception($"Unsupported Framework `{outputTarget.GetMaxNetAppVersion().Major}` for NuGet package 'Quartz.AspNetCore'")
            });
    }
}
