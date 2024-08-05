using System;
using Intent.Engine;
using Intent.Modules.Common.VisualStudio;

namespace Intent.Modules.HotChocolate.GraphQL.AzureFunctions
{
    public static class NugetPackages
    {

        public static NugetPackageInfo HotChocolateAzureFunctions(IOutputTarget outputTarget) => new NugetPackageInfo(
            name: "HotChocolate.AzureFunctions",
            version: outputTarget.GetMaxNetAppVersion() switch
            {
                (>= 7, 0) => "13.9.9",
                (>= 6, 0) => "13.9.9",
                _ => throw new Exception($"Unsupported Framework `{outputTarget.GetMaxNetAppVersion().Major}` for NuGet package 'HotChocolate.AzureFunctions'")
            });
    }
}
