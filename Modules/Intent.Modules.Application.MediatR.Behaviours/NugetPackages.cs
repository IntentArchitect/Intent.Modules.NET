using System;
using Intent.Engine;
using Intent.Modules.Common.VisualStudio;

namespace Intent.Modules.Application.MediatR.Behaviours
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

        public static NugetPackageInfo MicrosoftExtensionsLogging(IOutputTarget outputTarget) => new NugetPackageInfo(
            name: "Microsoft.Extensions.Logging",
            version: outputTarget.GetMaxNetAppVersion() switch
            {
                (>= 8, 0) => "8.0.0",
                (>= 7, 0) => "8.0.0",
                (>= 6, 0) => "6.0.0",
                _ => throw new Exception($"Unsupported Framework `{outputTarget.GetMaxNetAppVersion().Major}` for NuGet package 'Microsoft.Extensions.Logging'")
            });
    }
}
