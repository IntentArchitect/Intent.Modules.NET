using System;
using Intent.Engine;
using Intent.Modules.Common.VisualStudio;

namespace Intent.Modules.AspNetCore.Versioning
{
    public static class NugetPackages
    {

        public static NugetPackageInfo AspVersioningMvc(IOutputTarget outputTarget) => new NugetPackageInfo(
            name: "Asp.Versioning.Mvc",
            version: outputTarget.GetMaxNetAppVersion() switch
            {
                (>= 8, 0) => "8.1.0",
                (>= 7, 0) => "7.1.1",
                (>= 6, 0) => "6.4.1",
                _ => throw new Exception($"Unsupported Framework `{outputTarget.GetMaxNetAppVersion().Major}` for NuGet package 'Asp.Versioning.Mvc'")
            });

        public static NugetPackageInfo AspVersioningMvcApiExplorer(IOutputTarget outputTarget) => new NugetPackageInfo(
            name: "Asp.Versioning.Mvc.ApiExplorer",
            version: outputTarget.GetMaxNetAppVersion() switch
            {
                (>= 8, 0) => "8.1.0",
                (>= 7, 0) => "7.1.0",
                (>= 6, 0) => "6.4.0",
                _ => throw new Exception($"Unsupported Framework `{outputTarget.GetMaxNetAppVersion().Major}` for NuGet package 'Asp.Versioning.Mvc.ApiExplorer'")
            });
    }
}
