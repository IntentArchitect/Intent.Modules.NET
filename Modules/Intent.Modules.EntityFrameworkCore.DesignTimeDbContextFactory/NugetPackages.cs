using Intent.Engine;
using Intent.Modules.Common.CSharp.VisualStudio;
using Intent.Modules.Common.VisualStudio;

namespace Intent.Modules.EntityFrameworkCore.DesignTimeDbContextFactory;

public static class NugetPackages
{
    public static INugetPackageInfo MicrosoftExtensionsConfigurationFileExtensions(IOutputTarget outputTarget) =>
        new NugetPackageInfo("Microsoft.Extensions.Configuration.FileExtensions", GetVersion(outputTarget.GetProject()));
    public static INugetPackageInfo MicrosoftExtensionsConfigurationJson(IOutputTarget outputTarget) =>
        new NugetPackageInfo("Microsoft.Extensions.Configuration.Json", GetVersion(outputTarget.GetProject()));

    private static string GetVersion(ICSharpProject project)
    {
        return project switch
        {
            _ when project.IsNetApp(5) => "5.0.0",
            _ when project.IsNetApp(6) => "6.0.0",
            _ when project.IsNetApp(7) => "7.0.0",
            _ when project.IsNetApp(8) => "8.0.0",
            _ => "6.0.0"
        };
    }
}