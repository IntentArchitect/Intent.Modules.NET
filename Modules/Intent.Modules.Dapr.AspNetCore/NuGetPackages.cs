using System;
using Intent.Engine;
using Intent.Modules.Common.CSharp.Nuget;
using Intent.Modules.Common.VisualStudio;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.NugetPackages", Version = "1.0")]

namespace Intent.Modules.Dapr.AspNetCore
{
    public class NugetPackages : INugetPackages
    {
        public const string DaprAspNetCorePackageName = "Dapr.AspNetCore";
        public const string DaprClientPackageName = "Dapr.Client";
        public const string DaprExtensionsConfigurationPackageName = "Dapr.Extensions.Configuration";
        public const string ManDaprSidekickAspNetCorePackageName = "Man.Dapr.Sidekick.AspNetCore";
        public const string MediatRPackageName = "MediatR";

        public void RegisterPackages()
        {
            NugetRegistry.Register(DaprAspNetCorePackageName,
                (framework) => framework switch
                    {
                        ( >= 8, 0) => new PackageVersion("1.13.1"),
                        ( >= 6, 0) => new PackageVersion("1.13.1"),
                        _ => throw new Exception($"Unsupported Framework `{framework.Major}` for NuGet package '{DaprAspNetCorePackageName}'"),
                    }
                );
            NugetRegistry.Register(DaprClientPackageName,
                (framework) => framework switch
                    {
                        ( >= 8, 0) => new PackageVersion("1.13.1"),
                        ( >= 6, 0) => new PackageVersion("1.13.1"),
                        _ => throw new Exception($"Unsupported Framework `{framework.Major}` for NuGet package '{DaprClientPackageName}'"),
                    }
                );
            NugetRegistry.Register(DaprExtensionsConfigurationPackageName,
                (framework) => framework switch
                    {
                        ( >= 8, 0) => new PackageVersion("1.13.1"),
                        ( >= 6, 0) => new PackageVersion("1.13.1"),
                        _ => throw new Exception($"Unsupported Framework `{framework.Major}` for NuGet package '{DaprExtensionsConfigurationPackageName}'"),
                    }
                );
            NugetRegistry.Register(ManDaprSidekickAspNetCorePackageName,
                (framework) => framework switch
                    {
                        ( >= 2, 0) => new PackageVersion("1.2.2"),
                        _ => throw new Exception($"Unsupported Framework `{framework.Major}` for NuGet package '{ManDaprSidekickAspNetCorePackageName}'"),
                    }
                );
            NugetRegistry.Register(MediatRPackageName,
                (framework) => framework switch
                    {
                        ( >= 7, 0) => new PackageVersion("12.4.0"),
                        ( >= 6, 0) => new PackageVersion("12.1.1", locked: true),
                        _ => throw new Exception($"Unsupported Framework `{framework.Major}` for NuGet package '{MediatRPackageName}'"),
                    }
                );
        }

        public static NugetPackageInfo DaprAspNetCore(IOutputTarget outputTarget) => NugetRegistry.GetVersion(DaprAspNetCorePackageName, outputTarget.GetMaxNetAppVersion());

        public static NugetPackageInfo DaprClient(IOutputTarget outputTarget) => NugetRegistry.GetVersion(DaprClientPackageName, outputTarget.GetMaxNetAppVersion());

        public static NugetPackageInfo MediatR(IOutputTarget outputTarget) => NugetRegistry.GetVersion(MediatRPackageName, outputTarget.GetMaxNetAppVersion());

        public static NugetPackageInfo ManDaprSidekickAspNetCore(IOutputTarget outputTarget) => NugetRegistry.GetVersion(ManDaprSidekickAspNetCorePackageName, outputTarget.GetMaxNetAppVersion());

        public static NugetPackageInfo DaprExtensionsConfiguration(IOutputTarget outputTarget) => NugetRegistry.GetVersion(DaprExtensionsConfigurationPackageName, outputTarget.GetMaxNetAppVersion());
    }
}
