using Intent.Engine;
using Intent.Modules.Common.VisualStudio;

namespace Intent.Modules.Application.MediatR
{
    public static class NugetPackages
    {

        public static NugetPackageInfo MediatR(IOutputTarget outputTarget) => new(
            name: "MediatR",
            version: outputTarget.GetMaxNetAppVersion() switch
            {
                (6, 0) => "12.1.1",
                _ => "12.4.0",
            });
    }
}
