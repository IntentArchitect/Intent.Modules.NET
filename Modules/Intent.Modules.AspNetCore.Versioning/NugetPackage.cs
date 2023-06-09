using Intent.Modules.Common.VisualStudio;

namespace Intent.Modules.AspNetCore.Versioning;

public static class NugetPackage
{
    public static readonly INugetPackageInfo MicrosoftAspNetCoreMvcVersioning = new NugetPackageInfo("Microsoft.AspNetCore.Mvc.Versioning", "5.1.0");
    public static readonly INugetPackageInfo MicrosoftAspNetCoreMvcVersioningApiExplorer = new NugetPackageInfo("Microsoft.AspNetCore.Mvc.Versioning.ApiExplorer", "5.1.0");
}