using Intent.Engine;
using Intent.Modules.Common.VisualStudio;

namespace Intent.Modules.Application.FluentValidation
{
    public static class NugetPackages
    {

        public static NugetPackageInfo FluentValidationDependencyInjectionExtensions(IOutputTarget outputTarget) => new(
            name: "FluentValidation.DependencyInjectionExtensions",
            version: outputTarget.GetMaxNetAppVersion() switch
            {
                _ => "11.9.2",
            });
    }
}
