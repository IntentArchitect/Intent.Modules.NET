using Intent.Engine;
using Intent.Modules.Common.VisualStudio;

namespace Intent.Modules.HotChocolate.GraphQL.AzureFunctions
{
    public static class NugetPackages
    {

        public static NugetPackageInfo HotChocolateAzureFunctions(IOutputTarget outputTarget) => new(
            name: "HotChocolate.AzureFunctions",
            version: outputTarget.GetMaxNetAppVersion() switch
            {
                (6, 0) => "13.9.9",
                _ => "13.9.9",
            });
    }
}
