using Intent.Modules.Common.VisualStudio;

namespace Intent.Modules.Dapr.AspNetCore.ServiceInvocation;

public static class NuGetPackages
{
    public static readonly INugetPackageInfo DaprAspNetCore = new NugetPackageInfo("Dapr.AspNetCore", "1.9.0");
    public static readonly INugetPackageInfo ManDaprSidekickAspNetCore = new NugetPackageInfo("Man.Dapr.Sidekick.AspNetCore", "1.1.0");
}