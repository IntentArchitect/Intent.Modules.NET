using Intent.Modules.Common.CSharp.VisualStudio;
using Intent.Modules.Common.VisualStudio;

namespace Intent.Modules.AspNetCore.Identity;

public static class NugetPackages
{
    public static NugetPackageInfo MicrosoftAspNetCoreIdentityEntityFrameworkCore(ICSharpProject project) => new NugetPackageInfo("Microsoft.AspNetCore.Identity.EntityFrameworkCore", GetVersion(project));
    public static NugetPackageInfo MicrosoftExtensionsIdentityStores(ICSharpProject project) => new NugetPackageInfo("Microsoft.Extensions.Identity.Stores", GetVersion(project));
    
    private static string GetVersion(ICSharpProject project)
    {
        return project switch
        {
            _ when project.IsNetCore2App() => "2.1.14",
            _ when project.IsNetCore3App() => "3.1.15",
            _ when project.IsNetApp(5) => "5.0.17",
            _ when project.IsNetApp(6) => "6.0.20",
            _ when project.IsNetApp(7) => "7.0.9",
            _ when project.IsNetApp(8) => "8.0.0-preview.6.23329.11",
            _ => "6.0.20"
        };
    }
    
    
}