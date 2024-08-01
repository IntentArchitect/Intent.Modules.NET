using Intent.Engine;
using Intent.Modules.Common.VisualStudio;

namespace Intent.Modules.Application.DependencyInjection.MediatR
{
    public static class NugetPackages
    {

        public static NugetPackageInfo MediatR(IOutputTarget outputTarget) => new(
            name: "MediatR",
            version: outputTarget.GetMaxNetAppVersion() switch
            {
                _ => "12.4.0",
            });
    }
}
