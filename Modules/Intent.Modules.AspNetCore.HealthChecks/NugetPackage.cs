using System;
using Intent.Engine;
using Intent.Modules.Common.CSharp.VisualStudio;
using Intent.Modules.Common.VisualStudio;

namespace Intent.Modules.AspNetCore.HealthChecks;

public static class NugetPackage
{
    public static INugetPackageInfo AspNetCoreHealthChecksUIClient(IOutputTarget outputTarget) => new NugetPackageInfo("AspNetCore.HealthChecks.UI.Client", GetHealthChecksVersion(outputTarget.GetProject()));

    private static string GetHealthChecksVersion(ICSharpProject project)
    {
        return project switch
        {
            _ when project.IsNetApp(5) => "5.0.1",
            _ when project.IsNetApp(6) => "6.0.5",
            _ when project.IsNetApp(7) => "7.0.0-rc2.7",
            _ when project.IsNetApp(8) => "7.0.0-rc2.7",
            _ => throw new Exception("Not supported version of .NET Core") 
        };
    }
}