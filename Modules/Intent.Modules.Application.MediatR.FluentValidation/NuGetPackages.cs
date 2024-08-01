using Intent.Engine;
using Intent.Modules.Common.VisualStudio;

namespace Intent.Modules.Application.MediatR.FluentValidation
{
    public static class NugetPackages
    {

        public static NugetPackageInfo FluentValidation(IOutputTarget outputTarget) => new(
            name: "FluentValidation",
            version: outputTarget.GetMaxNetAppVersion() switch
            {
                (6, 0) => "11.9.2",
                (7, 0) => "11.9.2",
                _ => "11.9.2",
            });
    }
}
