using Intent.Engine;
using Intent.Modules.Common.CSharp.VisualStudio;
using Intent.Modules.Common.VisualStudio;

namespace Intent.Modules.Application.DependencyInjection;

public static class NugetPackages
{
    public static NugetPackageInfo MicrosoftExtensionsConfigurationAbstractions(IOutputTarget outputTarget) => new(
        name: "Microsoft.Extensions.Configuration.Abstractions",
        version: outputTarget.GetProject().GetMaxNetAppVersion() switch
        {
            (5, 0) => "5.0.0",
            (6, 0) => "6.0.0",
            (7, 0) => "7.0.0",
            _ => "8.0.0"
        });

    public static NugetPackageInfo MicrosoftExtensionsDependencyInjection(IOutputTarget outputTarget) => new(
        name: "Microsoft.Extensions.DependencyInjection",
        version: outputTarget.GetProject().GetMaxNetAppVersion() switch
        {
            (5, 0) => "5.0.2",
            (6, 0) => "6.0.1",
            (7, 0) => "7.0.0",
            _ => "8.0.0"
        });
    public static NugetPackageInfo MicrosoftExtensionsConfigurationBinder(IOutputTarget outputTarget) => new(
        name: "Microsoft.Extensions.Configuration.Binder",
        version: outputTarget.GetProject().GetMaxNetAppVersion() switch
        {
            (5, 0) => "5.0.0",
            (6, 0) => "6.0.0",
            (7, 0) => "7.0.0",
            _ => "8.0.0"
        });
}