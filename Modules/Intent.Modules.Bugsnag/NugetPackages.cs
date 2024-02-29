using Intent.Modules.Common.VisualStudio;

namespace Intent.Modules.Bugsnag;

public static class NugetPackages
{
    public static readonly INugetPackageInfo BugsnagAspNetCore = new NugetPackageInfo("Bugsnag.AspNet.Core", "3.1.0");
}