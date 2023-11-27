using System;
using Intent.Engine;
using Intent.Modules.Common.CSharp.VisualStudio;
using Intent.Modules.Common.VisualStudio;

namespace Intent.Modules.AspNetCore.Versioning;

public static class NugetPackage
{
    public static NugetPackageInfo AspVersioningMvc(IOutputTarget outputTarget) => new(
        name: "Asp.Versioning.Mvc",
        version: outputTarget.GetMaxNetAppVersion() switch
        {
            (5, 0) or (6, 0) => "6.0.0",
            _ => "7.0.0"
        });

    public static NugetPackageInfo AspVersioningMvcApiExplorer(IOutputTarget outputTarget) => new(
        name: "Asp.Versioning.Mvc.ApiExplorer",
        version: outputTarget.GetMaxNetAppVersion() switch
        {
            (5, 0) or (6, 0) => "6.0.0",
            _ => "7.0.0"
        });
}