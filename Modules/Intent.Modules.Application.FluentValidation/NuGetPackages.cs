using System;
using Intent.Engine;
using Intent.Modules.Common.VisualStudio;

namespace Intent.Modules.Application.FluentValidation
{
    public static class NugetPackages
    {

        public static NugetPackageInfo FluentValidationDependencyInjectionExtensions(IOutputTarget outputTarget) => new NugetPackageInfo(
            name: "FluentValidation.DependencyInjectionExtensions",
            version: outputTarget.GetMaxNetAppVersion() switch
            {
                (>= 2, 0) => "11.9.2",
                _ => throw new Exception($"Unsupported Framework `{outputTarget.GetMaxNetAppVersion().Major}` for NuGet package 'FluentValidation.DependencyInjectionExtensions'")
            });
    }
}
