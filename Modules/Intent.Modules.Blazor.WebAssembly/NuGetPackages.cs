using Intent.Engine;
using Intent.Modules.Common.VisualStudio;

namespace Intent.Modules.Blazor.WebAssembly
{
    public static class NugetPackages
    {

        public static NugetPackageInfo MicrosoftAspNetCoreComponentsWebAssembly(IOutputTarget outputTarget) => new(
            name: "Microsoft.AspNetCore.Components.WebAssembly",
            version: outputTarget.GetMaxNetAppVersion() switch
            {
                (6, 0) => "6.0.32",
                (7, 0) => "7.0.20",
                _ => "8.0.7",
            });

        public static NugetPackageInfo MicrosoftAspNetCoreComponentsWebAssemblyDevServer(IOutputTarget outputTarget) => new(
            name: "Microsoft.AspNetCore.Components.WebAssembly.DevServer",
            version: outputTarget.GetMaxNetAppVersion() switch
            {
                (7, 0) => "7.0.14",
                _ => "8.0.8",
            });
    }
}
