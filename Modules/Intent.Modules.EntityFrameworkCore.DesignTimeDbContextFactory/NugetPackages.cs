using Intent.Engine;
using Intent.Modules.Common.VisualStudio;

namespace Intent.Modules.EntityFrameworkCore.DesignTimeDbContextFactory;

public static class NugetPackages
{
    public static INugetPackageInfo MicrosoftExtensionsConfigurationFileExtensions(IOutputTarget outputTarget) => new NugetPackageInfo(
        name: "Microsoft.Extensions.Configuration.FileExtensions",
        version: outputTarget.GetMaxNetAppVersion() switch
        {
            (5, 0) => "5.0.0",
            (6, 0) => "6.0.0",
            (7, 0) => "7.0.0",
            _ => "8.0.0"
        });

    public static INugetPackageInfo MicrosoftExtensionsConfigurationJson(IOutputTarget outputTarget) => new NugetPackageInfo(
        name: "Microsoft.Extensions.Configuration.Json",
        version: outputTarget.GetMaxNetAppVersion() switch
        {
            (5, 0) => "5.0.0",
            (6, 0) => "6.0.0",
            (7, 0) => "7.0.0",
            _ => "8.0.0"
        });
}