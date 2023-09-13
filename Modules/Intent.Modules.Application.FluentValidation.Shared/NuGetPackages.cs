using Intent.Modules.Common.VisualStudio;

namespace Intent.Modules.Application.FluentValidation.Shared;

public static class NuGetPackages
{
    public static readonly INugetPackageInfo FluentValidation = new NugetPackageInfo("FluentValidation", "11.6.0");
    public static readonly INugetPackageInfo FluentValidationDependencyInjectionExtensions = new NugetPackageInfo("FluentValidation.DependencyInjectionExtensions", "11.6.0");
}