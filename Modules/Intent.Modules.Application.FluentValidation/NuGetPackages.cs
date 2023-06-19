using Intent.Modules.Common.VisualStudio;

namespace Intent.Modules.Application.FluentValidation
{
    static class NuGetPackages
    {
        public static readonly INugetPackageInfo FluentValidation = new NugetPackageInfo("FluentValidation", "9.3.0");
        public static readonly INugetPackageInfo FluentValidationDependencyInjectionExtensions = new NugetPackageInfo("FluentValidation.DependencyInjectionExtensions", "9.3.0");
    }
}
