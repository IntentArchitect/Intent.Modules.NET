using Intent.Engine;
using Intent.Modules.Common.VisualStudio;

namespace Intent.Modules.Integration.HttpClients.Shared;

public static class NuGetPackages
{
    public static INugetPackageInfo MicrosoftExtensionsHttp(IOutputTarget outputTarget) => new NugetPackageInfo(
        name: "Microsoft.Extensions.Http",
        version: outputTarget.GetMaxNetAppVersion() switch
        {
            (5, 0) => "5.0.0",
            (6, 0) => "6.0.0",
            (7, 0) => "7.0.0",
            _ => "8.0.0"
        });
    public static INugetPackageInfo SystemTextJson(IOutputTarget outputTarget) => new NugetPackageInfo(
        name: "System.Text.Json",
        version: outputTarget.GetMaxNetAppVersion() switch
        {
            _ => "8.0.4"
        });
    
    public static readonly INugetPackageInfo MicrosoftAspNetCoreWebUtilities = new NugetPackageInfo("Microsoft.AspNetCore.WebUtilities", "2.2.0");
    public static readonly INugetPackageInfo IdentityModelAspNetCore = new NugetPackageInfo("IdentityModel.AspNetCore", "4.3.0");
}