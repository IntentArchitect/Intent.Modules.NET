using Intent.Engine;
using Intent.Modules.Common.VisualStudio;

namespace Intent.Modules.AspNetCore.Swashbuckle
{
    public static class NugetPackages
    {

        public static NugetPackageInfo SwashbuckleAspNetCore(IOutputTarget outputTarget) => new(
            name: "Swashbuckle.AspNetCore",
            version: outputTarget.GetMaxNetAppVersion() switch
            {
                _ => "6.7.0",
            });
    }
}
