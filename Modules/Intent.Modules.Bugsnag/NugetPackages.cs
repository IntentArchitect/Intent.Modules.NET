using Intent.Engine;
using Intent.Modules.Common.VisualStudio;

namespace Intent.Modules.Bugsnag
{
    public static class NugetPackages
    {

        public static NugetPackageInfo BugsnagAspNetCore(IOutputTarget outputTarget) => new(
            name: "Bugsnag.AspNet.Core",
            version: outputTarget.GetMaxNetAppVersion() switch
            {
                _ => "3.1.0",
            });
    }
}
