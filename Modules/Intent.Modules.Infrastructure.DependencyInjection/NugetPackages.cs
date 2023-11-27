using Intent.Engine;
using Intent.Modules.Common.VisualStudio;

namespace Intent.Modules.Infrastructure.DependencyInjection;

public static class NugetPackages
{
    public static INugetPackageInfo MicrosoftExtensionsConfigurationAbstractions(IOutputTarget outputTarget) => new NugetPackageInfo(
        name: "Microsoft.Extensions.Configuration.Abstractions",
        version: outputTarget.GetMaxNetAppVersion() switch
        {
            (5, 0) => "5.0.0",
            (6, 0) => "6.0.0",
            (7, 0) => "7.0.0",
            _ => "8.0.0"
        });

    public static NugetPackageInfo MicrosoftExtensionsDependencyInjection(IOutputTarget outputTarget) => new(
        name: "Microsoft.Extensions.DependencyInjection",
        version: outputTarget.GetMaxNetAppVersion() switch
        {
            (5, 0) => "5.0.2",
            (6, 0) => "6.0.1",
            (7, 0) => "7.0.0",
            _ => "8.0.0"
        });

    public static NugetPackageInfo MicrosoftExtensionsConfigurationBinder(IOutputTarget outputTarget) => new(
        name: "Microsoft.Extensions.Configuration.Binder",
        version: outputTarget.GetMaxNetAppVersion() switch
        {
            (5, 0) => "5.0.0",
            (6, 0) => "6.0.0",
            (7, 0) => "7.0.0",
            _ => "8.0.0"
        });
}