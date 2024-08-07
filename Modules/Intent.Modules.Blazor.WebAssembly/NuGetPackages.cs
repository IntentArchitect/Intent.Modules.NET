using System;
using Intent.Engine;
using Intent.Modules.Common.VisualStudio;

namespace Intent.Modules.Blazor.WebAssembly
{
    public static class NugetPackages
    {

        public static NugetPackageInfo MicrosoftAspNetCoreComponentsWebAssembly(IOutputTarget outputTarget) => new NugetPackageInfo(
            name: "Microsoft.AspNetCore.Components.WebAssembly",
            version: outputTarget.GetMaxNetAppVersion() switch
            {
                (>= 8, 0) => "8.0.7",
                (>= 7, 0) => "7.0.20",
                (>= 6, 0) => "6.0.32",
                _ => throw new Exception($"Unsupported Framework `{outputTarget.GetMaxNetAppVersion().Major}` for NuGet package 'Microsoft.AspNetCore.Components.WebAssembly'")
            });
        //This assembly does not have dependency info, also 8.0.7 & 8.0.8 seem problematic

        public static NugetPackageInfo MicrosoftAspNetCoreComponentsWebAssemblyDevServer(IOutputTarget outputTarget) => new NugetPackageInfo(
            name: "Microsoft.AspNetCore.Components.WebAssembly.DevServer",
            version: outputTarget.GetMaxNetAppVersion() switch
            {
                (>= 8, 0) => "8.0.0",
                (>= 7, 0) => "7.0.14",
                _ => throw new Exception($"Unsupported Framework `{outputTarget.GetMaxNetAppVersion().Major}` for NuGet package 'Microsoft.AspNetCore.Components.WebAssembly.DevServer'")
            });
    }
}
