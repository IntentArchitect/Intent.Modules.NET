using Intent.Modules.Common.VisualStudio;

namespace Intent.Modules.Dapr.AspNetCore;

public static class NuGetPackages
{
    public static readonly INugetPackageInfo DaprAspNetCore = new NugetPackageInfo("Dapr.AspNetCore", "1.9.0");
    public static readonly INugetPackageInfo DaprClient = new NugetPackageInfo("Dapr.Client", "1.9.0");
    public static readonly INugetPackageInfo MediatR = new NugetPackageInfo("MediatR", "12.1.1");
    public static readonly INugetPackageInfo ManDaprSidekickAspNetCore = new NugetPackageInfo("Man.Dapr.Sidekick.AspNetCore", "1.1.0");
    public static readonly INugetPackageInfo DaprExtensionsConfiguration = new NugetPackageInfo("Dapr.Extensions.Configuration", "1.9.0");
    
}