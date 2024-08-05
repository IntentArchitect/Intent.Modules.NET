using System;
using Intent.Engine;
using Intent.Modules.Common.VisualStudio;

namespace Intent.Modules.AspNetCore.Swashbuckle
{
    public static class NugetPackages
    {

        public static NugetPackageInfo SwashbuckleAspNetCore(IOutputTarget outputTarget) => new NugetPackageInfo(
            name: "Swashbuckle.AspNetCore",
            version: outputTarget.GetMaxNetAppVersion() switch
            {
                (>= 2, 0) => "6.7.0",
                _ => throw new Exception($"Unsupported Framework `{outputTarget.GetMaxNetAppVersion().Major}` for NuGet package 'Swashbuckle.AspNetCore'")
            });
    }
}
