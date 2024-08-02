using Intent.Engine;
using Intent.Modules.Common.VisualStudio;

namespace Intent.Modules.Dapr.AspNetCore
{
    public static class NugetPackages
    {

        public static NugetPackageInfo DaprAspNetCore(IOutputTarget outputTarget) => new(
            name: "Dapr.AspNetCore",
            version: outputTarget.GetMaxNetAppVersion() switch
            {
                (6, 0) => "1.13.1",
                _ => "1.13.1",
            });

        public static NugetPackageInfo DaprClient(IOutputTarget outputTarget) => new(
            name: "Dapr.Client",
            version: outputTarget.GetMaxNetAppVersion() switch
            {
                (6, 0) => "1.13.1",
                _ => "1.13.1",
            });

        public static NugetPackageInfo MediatR(IOutputTarget outputTarget) => new(
            name: "MediatR",
            version: outputTarget.GetMaxNetAppVersion() switch
            {
                (6, 0) => "12.1.1",
                _ => "12.4.0",
            });

        public static NugetPackageInfo ManDaprSidekickAspNetCore(IOutputTarget outputTarget) => new(
            name: "Man.Dapr.Sidekick.AspNetCore",
            version: outputTarget.GetMaxNetAppVersion() switch
            {
                _ => "1.2.2",
            });

        public static NugetPackageInfo DaprExtensionsConfiguration(IOutputTarget outputTarget) => new(
            name: "Dapr.Extensions.Configuration",
            version: outputTarget.GetMaxNetAppVersion() switch
            {
                (6, 0) => "1.13.1",
                _ => "1.13.1",
            });
    }
}
