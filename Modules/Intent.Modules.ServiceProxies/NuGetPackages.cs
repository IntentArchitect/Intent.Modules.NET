using Intent.Modules.Common.VisualStudio;

namespace Intent.Modules.ServiceProxies;

public static class NuGetPackages
{
    public static readonly INugetPackageInfo MicrosoftExtensionsHttp = new NugetPackageInfo("Microsoft.Extensions.Http", "6.0.0");
    public static readonly INugetPackageInfo MicrosoftAspNetCoreWebUtilities = new NugetPackageInfo("Microsoft.AspNetCore.WebUtilities", "2.2.0");
    public static readonly INugetPackageInfo IdentityModelAspNetCore = new NugetPackageInfo("IdentityModel.AspNetCore", "4.3.0");
}