using System;
using Intent.Engine;
using Intent.Modules.Common.VisualStudio;

namespace Intent.Modules.Bugsnag
{
    public static class NugetPackages
    {

        public static NugetPackageInfo BugsnagAspNetCore(IOutputTarget outputTarget) => new NugetPackageInfo(
            name: "Bugsnag.AspNet.Core",
            version: outputTarget.GetMaxNetAppVersion() switch
            {
                (>= 2, 0) => "3.1.0",
                _ => throw new Exception($"Unsupported Framework `{outputTarget.GetMaxNetAppVersion().Major}` for NuGet package 'Bugsnag.AspNet.Core'")
            });
    }
}
