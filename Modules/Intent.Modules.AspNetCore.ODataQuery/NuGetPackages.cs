using Intent.Engine;
using Intent.Modules.Common.VisualStudio;

namespace Intent.Modules.AspNetCore.ODataQuery
{
    public static class NugetPackages
    {

        public static NugetPackageInfo MicrosoftAspNetCoreOData(IOutputTarget outputTarget) => new(
            name: "Microsoft.AspNetCore.OData",
            version: outputTarget.GetMaxNetAppVersion() switch
            {
                _ => "8.2.5",
            });
    }
}
