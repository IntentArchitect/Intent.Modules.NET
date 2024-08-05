using System;
using Intent.Engine;
using Intent.Modules.Common.VisualStudio;

namespace Intent.Modules.Ardalis.GuardClauses
{
    public static class NugetPackages
    {

        public static NugetPackageInfo ArdalisGuardClauses(IOutputTarget outputTarget) => new NugetPackageInfo(
            name: "Ardalis.GuardClauses",
            version: outputTarget.GetMaxNetAppVersion() switch
            {
                (>= 8, 0) => "4.6.0",
                (>= 7, 0) => "4.5.0",
                (>= 6, 0) => "4.0.1",
                _ => throw new Exception($"Unsupported Framework `{outputTarget.GetMaxNetAppVersion().Major}` for NuGet package 'Ardalis.GuardClauses'")
            });
    }
}
