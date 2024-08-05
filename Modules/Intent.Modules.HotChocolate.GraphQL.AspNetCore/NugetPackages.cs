using Intent.Engine;
using Intent.Modules.Common.VisualStudio;

namespace Intent.Modules.HotChocolate.GraphQL.AspNetCore
{
    public static class NugetPackages
    {

        public static NugetPackageInfo HotChocolateAspNetCore(IOutputTarget outputTarget) => new(
            name: "HotChocolate.AspNetCore",
            version: outputTarget.GetMaxNetAppVersion() switch
            {
                (6, 0) => "13.9.9",
                (7, 0) => "13.9.9",
                _ => "13.9.9",
            });

        public static NugetPackageInfo HotChocolateAspNetCoreAuthorization(IOutputTarget outputTarget) => new(
            name: "HotChocolate.AspNetCore.Authorization",
            version: outputTarget.GetMaxNetAppVersion() switch
            {
                (6, 0) => "13.9.9",
                (7, 0) => "13.9.9",
                _ => "13.9.9",
            });
    }
}
