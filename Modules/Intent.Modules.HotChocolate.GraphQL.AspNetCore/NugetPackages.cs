using System;
using Intent.Engine;
using Intent.Modules.Common.VisualStudio;

namespace Intent.Modules.HotChocolate.GraphQL.AspNetCore
{
    public static class NugetPackages
    {

        public static NugetPackageInfo HotChocolateAspNetCore(IOutputTarget outputTarget) => new NugetPackageInfo(
            name: "HotChocolate.AspNetCore",
            version: outputTarget.GetMaxNetAppVersion() switch
            {
                (>= 8, 0) => "13.9.9",
                (>= 7, 0) => "13.9.9",
                (>= 6, 0) => "13.9.9",
                _ => throw new Exception($"Unsupported Framework `{outputTarget.GetMaxNetAppVersion().Major}` for NuGet package 'HotChocolate.AspNetCore'")
            });

        public static NugetPackageInfo HotChocolateAspNetCoreAuthorization(IOutputTarget outputTarget) => new NugetPackageInfo(
            name: "HotChocolate.AspNetCore.Authorization",
            version: outputTarget.GetMaxNetAppVersion() switch
            {
                (>= 8, 0) => "13.9.9",
                (>= 7, 0) => "13.9.9",
                (>= 6, 0) => "13.9.9",
                _ => throw new Exception($"Unsupported Framework `{outputTarget.GetMaxNetAppVersion().Major}` for NuGet package 'HotChocolate.AspNetCore.Authorization'")
            });
    }
}
