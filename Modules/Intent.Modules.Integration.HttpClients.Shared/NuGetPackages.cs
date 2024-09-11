using Intent.Engine;
using Intent.Modules.Common.CSharp.Nuget;
using Intent.Modules.Common.VisualStudio;

namespace Intent.Modules.Integration.HttpClients.Shared;

public static class NuGetPackages
{
    public const string IdentityModelAspNetCorePackageName = "IdentityModel.AspNetCore";
    public const string MicrosoftAspNetCoreWebUtilitiesPackageName = "Microsoft.AspNetCore.WebUtilities";
    public const string MicrosoftExtensionsHttpPackageName = "Microsoft.Extensions.Http";
    public const string SystemTextJsonPackageName = "System.Text.Json";

    public static NugetPackageInfo IdentityModelAspNetCore(IOutputTarget outputTarget) => NugetRegistry.GetVersion(IdentityModelAspNetCorePackageName, outputTarget.GetMaxNetAppVersion());

    public static NugetPackageInfo MicrosoftAspNetCoreWebUtilities(IOutputTarget outputTarget) => NugetRegistry.GetVersion(MicrosoftAspNetCoreWebUtilitiesPackageName, outputTarget.GetMaxNetAppVersion());

    public static NugetPackageInfo MicrosoftExtensionsHttp(IOutputTarget outputTarget) => NugetRegistry.GetVersion(MicrosoftExtensionsHttpPackageName, outputTarget.GetMaxNetAppVersion());

    public static NugetPackageInfo SystemTextJson(IOutputTarget outputTarget) => NugetRegistry.GetVersion(SystemTextJsonPackageName, outputTarget.GetMaxNetAppVersion());
}