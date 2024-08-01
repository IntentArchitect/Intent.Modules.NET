using Intent.Engine;
using Intent.Modules.Common.VisualStudio;

namespace Intent.Modules.Application.AutoMapper
{
    public static class NugetPackages
    {

        public static NugetPackageInfo AutoMapper(IOutputTarget outputTarget) => new(
            name: "AutoMapper",
            version: outputTarget.GetMaxNetAppVersion() switch
            {
                _ => "13.0.1",
            });
    }
}
