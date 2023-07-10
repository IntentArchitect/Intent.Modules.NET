using Intent.Modules.Common.VisualStudio;

namespace Intent.Modules.Application.FluentValidation.Dtos;

public static class NuGetPackages
{
    public static readonly INugetPackageInfo FluentValidation = new NugetPackageInfo("FluentValidation", "11.6.0");
}