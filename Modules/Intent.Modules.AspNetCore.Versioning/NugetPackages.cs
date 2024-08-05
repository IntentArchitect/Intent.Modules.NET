using Intent.Engine;
using Intent.Modules.Common.VisualStudio;

namespace Intent.Modules.AspNetCore.Versioning
{
    public static class NugetPackages
    {

        public static NugetPackageInfo AspVersioningMvc(IOutputTarget outputTarget) => new(
            name: "Asp.Versioning.Mvc",
            version: outputTarget.GetMaxNetAppVersion() switch
            {
                (6, 0) => "6.4.1",
                (7, 0) => "7.1.1",
                _ => "8.1.0",
            });

        public static NugetPackageInfo AspVersioningMvcApiExplorer(IOutputTarget outputTarget) => new(
            name: "Asp.Versioning.Mvc.ApiExplorer",
            version: outputTarget.GetMaxNetAppVersion() switch
            {
                (6, 0) => "6.4.0",
                (7, 0) => "7.1.0",
                _ => "8.1.0",
            });
    }
}
