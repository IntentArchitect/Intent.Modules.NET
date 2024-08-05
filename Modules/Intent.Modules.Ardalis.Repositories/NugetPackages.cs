using System;
using Intent.Engine;
using Intent.Modules.Common.VisualStudio;

namespace Intent.Modules.Ardalis.Repositories
{
    public static class NugetPackages
    {

        public static NugetPackageInfo ArdalisSpecification(IOutputTarget outputTarget) => new NugetPackageInfo(
            name: "Ardalis.Specification",
            version: outputTarget.GetMaxNetAppVersion() switch
            {
                (>= 8, 0) => "8.0.0",
                (>= 7, 0) => "8.0.0",
                (>= 6, 0) => "8.0.0",
                _ => throw new Exception($"Unsupported Framework `{outputTarget.GetMaxNetAppVersion().Major}` for NuGet package 'Ardalis.Specification'")
            });

        public static NugetPackageInfo ArdalisSpecificationEntityFrameworkCore(IOutputTarget outputTarget) => new NugetPackageInfo(
            name: "Ardalis.Specification.EntityFrameworkCore",
            version: outputTarget.GetMaxNetAppVersion() switch
            {
                (>= 8, 0) => "8.0.0",
                (>= 7, 0) => "8.0.0",
                (>= 6, 0) => "8.0.0",
                _ => throw new Exception($"Unsupported Framework `{outputTarget.GetMaxNetAppVersion().Major}` for NuGet package 'Ardalis.Specification.EntityFrameworkCore'")
            });
    }
}
