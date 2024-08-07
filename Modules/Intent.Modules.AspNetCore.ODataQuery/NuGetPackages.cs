using System;
using Intent.Engine;
using Intent.Modules.Common.VisualStudio;

namespace Intent.Modules.AspNetCore.ODataQuery
{
    public static class NugetPackages
    {

        public static NugetPackageInfo MicrosoftAspNetCoreOData(IOutputTarget outputTarget) => new NugetPackageInfo(
            name: "Microsoft.AspNetCore.OData",
            version: outputTarget.GetMaxNetAppVersion() switch
            {
                (>= 6, 0) => "8.2.5",
                _ => throw new Exception($"Unsupported Framework `{outputTarget.GetMaxNetAppVersion().Major}` for NuGet package 'Microsoft.AspNetCore.OData'")
            });
    }
}
