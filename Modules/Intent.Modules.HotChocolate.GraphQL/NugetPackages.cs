using Intent.Engine;
using Intent.Modules.Common.VisualStudio;

namespace Intent.Modules.HotChocolate.GraphQL
{
    public static class NugetPackages
    {

        public static NugetPackageInfo HotChocolate(IOutputTarget outputTarget) => new(
            name: "HotChocolate",
            version: outputTarget.GetMaxNetAppVersion() switch
            {
                (6, 0) => "13.9.9",
                (7, 0) => "13.9.9",
                _ => "13.9.9",
            });
    }
}
