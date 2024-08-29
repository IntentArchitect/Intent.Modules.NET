using System;
using Intent.Engine;
using Intent.Modules.Common.CSharp.Nuget;
using Intent.Modules.Common.VisualStudio;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.NugetPackages", Version = "1.0")]

namespace Intent.Modules.WindowsServiceHost
{
    public class NugetPackages : INugetPackages
    {
        public const string MicrosoftExtensionsConfigurationAbstractionsPackageName = "Microsoft.Extensions.Configuration.Abstractions";
        public const string MicrosoftExtensionsConfigurationBinderPackageName = "Microsoft.Extensions.Configuration.Binder";
        public const string MicrosoftExtensionsDependencyInjectionPackageName = "Microsoft.Extensions.DependencyInjection";
        public const string MicrosoftExtensionsHostingPackageName = "Microsoft.Extensions.Hosting";
        public const string MicrosoftExtensionsHostingWindowsServicesPackageName = "Microsoft.Extensions.Hosting.WindowsServices";

        public void RegisterPackages()
        {
            NugetRegistry.Register(MicrosoftExtensionsConfigurationAbstractionsPackageName,
                (framework) => framework switch
                    {
                        ( >= 8, 0) => new PackageVersion("8.0.0"),
                        ( >= 7, 0) => new PackageVersion("8.0.0"),
                        ( >= 6, 0) => new PackageVersion("6.0.0", locked: true),
                        _ => throw new Exception($"Unsupported Framework `{framework.Major}` for NuGet package '{MicrosoftExtensionsConfigurationAbstractionsPackageName}'"),
                    }
                );
            NugetRegistry.Register(MicrosoftExtensionsConfigurationBinderPackageName,
                (framework) => framework switch
                    {
                        ( >= 8, 0) => new PackageVersion("8.0.2"),
                        ( >= 7, 0) => new PackageVersion("8.0.2"),
                        ( >= 6, 0) => new PackageVersion("6.0.0", locked: true),
                        _ => throw new Exception($"Unsupported Framework `{framework.Major}` for NuGet package '{MicrosoftExtensionsConfigurationBinderPackageName}'"),
                    }
                );
            NugetRegistry.Register(MicrosoftExtensionsDependencyInjectionPackageName,
                (framework) => framework switch
                    {
                        ( >= 8, 0) => new PackageVersion("8.0.0"),
                        ( >= 7, 0) => new PackageVersion("8.0.0"),
                        ( >= 6, 0) => new PackageVersion("6.0.1", locked: true),
                        _ => throw new Exception($"Unsupported Framework `{framework.Major}` for NuGet package '{MicrosoftExtensionsDependencyInjectionPackageName}'"),
                    }
                );
            NugetRegistry.Register(MicrosoftExtensionsHostingPackageName,
                (framework) => framework switch
                    {
                        ( >= 8, 0) => new PackageVersion("8.0.0"),
                        ( >= 7, 0) => new PackageVersion("8.0.0"),
                        ( >= 6, 0) => new PackageVersion("8.0.0"),
                        _ => throw new Exception($"Unsupported Framework `{framework.Major}` for NuGet package '{MicrosoftExtensionsHostingPackageName}'"),
                    }
                );
            NugetRegistry.Register(MicrosoftExtensionsHostingWindowsServicesPackageName,
                (framework) => framework switch
                    {
                        ( >= 8, 0) => new PackageVersion("8.0.0"),
                        ( >= 7, 0) => new PackageVersion("8.0.0"),
                        ( >= 6, 0) => new PackageVersion("8.0.0"),
                        _ => throw new Exception($"Unsupported Framework `{framework.Major}` for NuGet package '{MicrosoftExtensionsHostingWindowsServicesPackageName}'"),
                    }
                );
        }

        public static NugetPackageInfo MicrosoftExtensionsHosting(IOutputTarget outputTarget) => NugetRegistry.GetVersion(MicrosoftExtensionsHostingPackageName, outputTarget.GetMaxNetAppVersion());

        public static NugetPackageInfo MicrosoftExtensionsHostingWindowsServices(IOutputTarget outputTarget) => NugetRegistry.GetVersion(MicrosoftExtensionsHostingWindowsServicesPackageName, outputTarget.GetMaxNetAppVersion());

        public static NugetPackageInfo MicrosoftExtensionsConfigurationAbstractions(IOutputTarget outputTarget) => NugetRegistry.GetVersion(MicrosoftExtensionsConfigurationAbstractionsPackageName, outputTarget.GetMaxNetAppVersion());

        public static NugetPackageInfo MicrosoftExtensionsDependencyInjection(IOutputTarget outputTarget) => NugetRegistry.GetVersion(MicrosoftExtensionsDependencyInjectionPackageName, outputTarget.GetMaxNetAppVersion());

        public static NugetPackageInfo MicrosoftExtensionsConfigurationBinder(IOutputTarget outputTarget) => NugetRegistry.GetVersion(MicrosoftExtensionsConfigurationBinderPackageName, outputTarget.GetMaxNetAppVersion());
    }
}
