using System;
using Intent.Engine;
using Intent.Modules.Common.VisualStudio;

namespace Intent.Modules.Application.MediatR
{
    public static class NugetPackages
    {

        public static NugetPackageInfo MediatR(IOutputTarget outputTarget) => new NugetPackageInfo(
            name: "MediatR",
            version: outputTarget.GetMaxNetAppVersion() switch
            {
                (>= 7, 0) => "12.4.0",
                (>= 6, 0) => "12.1.1",
                _ => throw new Exception($"Unsupported Framework `{outputTarget.GetMaxNetAppVersion().Major}` for NuGet package 'MediatR'")
            });
    }
}
