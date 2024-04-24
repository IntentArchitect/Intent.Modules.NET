using Intent.Engine;
using Intent.Modules.Common.VisualStudio;

namespace Intent.Modules.Blazor.Templates;

public static class NuGetPackages
{
    public static NugetPackageInfo MicrosoftAspNetCoreComponentsWebAssembly(IOutputTarget outputTarget) => new(
        name: "Microsoft.AspNetCore.Components.WebAssembly",
        version: outputTarget.GetMaxNetAppVersion() switch
        {
            (5, 0) => "5.0.17",
            (6, 0) => "6.0.25",
            (7, 0) => "7.0.14",
            _ => "8.0.3"
        });
}