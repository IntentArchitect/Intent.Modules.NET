using Intent.Engine;
using Intent.Modules.Common.VisualStudio;

namespace Intent.Modules.Integration.HttpClients.Shared;

public static class NuGetPackages
{
    /// <summary>
    /// I have not configured different output targets because version 8.0.0 of this package supports .NET 6.0, .NET 7.0, and .NET 8.0, along with netstandard2.0.
    /// </summary>
    public static readonly INugetPackageInfo MicrosoftExtensionsHttp = new NugetPackageInfo("Microsoft.Extensions.Http", "8.0.0");
    public static readonly INugetPackageInfo MicrosoftAspNetCoreWebUtilities = new NugetPackageInfo("Microsoft.AspNetCore.WebUtilities", "2.2.0");
    public static readonly INugetPackageInfo IdentityModelAspNetCore = new NugetPackageInfo("IdentityModel.AspNetCore", "4.3.0");
}