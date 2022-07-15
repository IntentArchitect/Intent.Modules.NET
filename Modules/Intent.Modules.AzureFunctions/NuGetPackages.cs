using Intent.Modules.Common.VisualStudio;

namespace Intent.Modules.AzureFunctions;

static class NuGetPackages
{
    public static readonly INugetPackageInfo MicrosoftNETSdkFunctions =
        new NugetPackageInfo("Microsoft.NET.Sdk.Functions", "4.1.1");

    public static readonly INugetPackageInfo MicrosoftExtensionsDependencyInjection =
        new NugetPackageInfo("Microsoft.Extensions.DependencyInjection", "6.0.0");

    public static readonly INugetPackageInfo MicrosoftExtensionsHttp =
        new NugetPackageInfo("Microsoft.Extensions.Http", "6.0.0");

    public static readonly INugetPackageInfo MicrosoftAzureFunctionsExtensions =
        new NugetPackageInfo("Microsoft.Azure.Functions.Extensions", "1.1.0");
}