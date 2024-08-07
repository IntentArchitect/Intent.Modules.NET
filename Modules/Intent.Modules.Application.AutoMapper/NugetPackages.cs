using System;
using Intent.Engine;
using Intent.Modules.Common.VisualStudio;

namespace Intent.Modules.Application.AutoMapper
{
    public static class NugetPackages
    {

        public static NugetPackageInfo AutoMapper(IOutputTarget outputTarget) => new NugetPackageInfo(
            name: "AutoMapper",
            version: outputTarget.GetMaxNetAppVersion() switch
            {
                (>= 6, 0) => "13.0.1",
                _ => throw new Exception($"Unsupported Framework `{outputTarget.GetMaxNetAppVersion().Major}` for NuGet package 'AutoMapper'")
            });
    }
}
