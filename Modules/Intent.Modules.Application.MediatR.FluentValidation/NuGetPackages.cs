using System;
using Intent.Engine;
using Intent.Modules.Common.VisualStudio;

namespace Intent.Modules.Application.MediatR.FluentValidation
{
    public static class NugetPackages
    {

        public static NugetPackageInfo FluentValidation(IOutputTarget outputTarget) => new NugetPackageInfo(
            name: "FluentValidation",
            version: outputTarget.GetMaxNetAppVersion() switch
            {
                (>= 8, 0) => "11.9.2",
                (>= 7, 0) => "11.9.2",
                (>= 6, 0) => "11.9.2",
                _ => throw new Exception($"Unsupported Framework `{outputTarget.GetMaxNetAppVersion().Major}` for NuGet package 'FluentValidation'")
            });
    }
}
