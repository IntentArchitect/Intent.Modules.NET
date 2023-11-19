using System;
using Intent.Engine;
using Intent.Modules.Common.CSharp.VisualStudio;
using Intent.Modules.Common.VisualStudio;

namespace Intent.Modules.AspNetCore.Versioning;

public static class NugetPackage
{
    public static NugetPackageInfo AspVersioningMvc(IOutputTarget outputTarget) => new NugetPackageInfo("Asp.Versioning.Mvc", GetVersion(outputTarget.GetProject()));
    public static NugetPackageInfo AspVersioningMvcApiExplorer(IOutputTarget outputTarget) => new NugetPackageInfo("Asp.Versioning.Mvc.ApiExplorer", GetVersion(outputTarget.GetProject()));

    private static string GetVersion(ICSharpProject project)
    {
        return project switch
        {
            _ when project.IsNetApp(6) => "6.0.0",
            _ when project.IsNetApp(7) => "7.0.0",
            _ when project.IsNetApp(8) => "7.0.0",
            _ => throw new Exception("Not supported version of .NET Core") 
        };
    }
}