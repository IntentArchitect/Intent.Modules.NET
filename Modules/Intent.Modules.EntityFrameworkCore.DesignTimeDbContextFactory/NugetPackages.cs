using Intent.Engine;
using Intent.Modules.Common.VisualStudio;

namespace Intent.Modules.EntityFrameworkCore.DesignTimeDbContextFactory
{
    public static class NugetPackages
    {

        public static NugetPackageInfo MicrosoftExtensionsConfigurationFileExtensions(IOutputTarget outputTarget) => new(
            name: "Microsoft.Extensions.Configuration.FileExtensions",
            version: outputTarget.GetMaxNetAppVersion() switch
            {
                (6, 0) => "6.0.0",
                (7, 0) => "8.0.1",
                _ => "8.0.1",
            });

        public static NugetPackageInfo MicrosoftExtensionsConfigurationJson(IOutputTarget outputTarget) => new(
            name: "Microsoft.Extensions.Configuration.Json",
            version: outputTarget.GetMaxNetAppVersion() switch
            {
                (6, 0) => "6.0.0",
                (7, 0) => "8.0.0",
                _ => "8.0.0",
            });

        public static NugetPackageInfo MicrosoftExtensionsConfigurationUserSecrets(IOutputTarget outputTarget) => new(
            name: "Microsoft.Extensions.Configuration.UserSecrets",
            version: outputTarget.GetMaxNetAppVersion() switch
            {
                (6, 0) => "6.0.1",
                (7, 0) => "8.0.0",
                _ => "8.0.0",
            });

        public static NugetPackageInfo MicrosoftExtensionsConfigurationEnvironmentVariables(IOutputTarget outputTarget) => new(
            name: "Microsoft.Extensions.Configuration.EnvironmentVariables",
            version: outputTarget.GetMaxNetAppVersion() switch
            {
                (6, 0) => "6.0.1",
                (7, 0) => "8.0.0",
                _ => "8.0.0",
            });
    }
}
