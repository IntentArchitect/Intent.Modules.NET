using System;
using Intent.Engine;
using Intent.Modules.Common.CSharp.Nuget;
using Intent.Modules.Common.VisualStudio;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.NugetPackages", Version = "1.0")]

namespace Intent.Modules.Application.DependencyInjection
{
    public class NugetPackages : INugetPackages
    {
        public const string MicrosoftExtensionsConfigurationAbstractionsPackageName = "Microsoft.Extensions.Configuration.Abstractions";
        public const string MicrosoftExtensionsConfigurationBinderPackageName = "Microsoft.Extensions.Configuration.Binder";
        public const string MicrosoftExtensionsDependencyInjectionPackageName = "Microsoft.Extensions.DependencyInjection";

        public void RegisterPackages()
        {
            NugetRegistry.Register(MicrosoftExtensionsConfigurationAbstractionsPackageName,
                (framework) => (framework.Major, framework.Minor) switch
                    {
                        ( >= 9, >= 0) => new PackageVersion("9.0.6")
                            .WithNugetDependency("Microsoft.Extensions.Primitives", "9.0.6"),
                        ( >= 8, >= 0) => new PackageVersion("9.0.6")
                            .WithNugetDependency("Microsoft.Extensions.Primitives", "9.0.6"),
                        ( >= 2, >= 0) => new PackageVersion("9.0.6")
                            .WithNugetDependency("Microsoft.Extensions.Primitives", "9.0.6"),
                        _ => throw new Exception($"Unsupported Framework `{framework.Major}` for NuGet package '{MicrosoftExtensionsConfigurationAbstractionsPackageName}'"),
                    }
                );
            NugetRegistry.Register(MicrosoftExtensionsConfigurationBinderPackageName,
                (framework) => (framework.Major, framework.Minor) switch
                    {
                        ( >= 9, >= 0) => new PackageVersion("9.0.6")
                            .WithNugetDependency("Microsoft.Extensions.Configuration.Abstractions", "9.0.6"),
                        ( >= 8, >= 0) => new PackageVersion("9.0.6")
                            .WithNugetDependency("Microsoft.Extensions.Configuration.Abstractions", "9.0.6"),
                        ( >= 2, >= 0) => new PackageVersion("9.0.6")
                            .WithNugetDependency("Microsoft.Extensions.Configuration.Abstractions", "9.0.6"),
                        _ => throw new Exception($"Unsupported Framework `{framework.Major}` for NuGet package '{MicrosoftExtensionsConfigurationBinderPackageName}'"),
                    }
                );
            NugetRegistry.Register(MicrosoftExtensionsDependencyInjectionPackageName,
                (framework) => (framework.Major, framework.Minor) switch
                    {
                        ( >= 9, >= 0) => new PackageVersion("9.0.6")
                            .WithNugetDependency("Microsoft.Extensions.DependencyInjection.Abstractions", "9.0.6"),
                        ( >= 8, >= 0) => new PackageVersion("9.0.6")
                            .WithNugetDependency("Microsoft.Extensions.DependencyInjection.Abstractions", "9.0.6"),
                        ( >= 2, >= 1) => new PackageVersion("9.0.6")
                            .WithNugetDependency("Microsoft.Extensions.DependencyInjection.Abstractions", "9.0.6"),
                        ( >= 2, >= 0) => new PackageVersion("9.0.6")
                            .WithNugetDependency("Microsoft.Bcl.AsyncInterfaces", "9.0.6")
                            .WithNugetDependency("Microsoft.Extensions.DependencyInjection.Abstractions", "9.0.6")
                            .WithNugetDependency("System.Threading.Tasks.Extensions", "4.5.4"),
                        _ => throw new Exception($"Unsupported Framework `{framework.Major}` for NuGet package '{MicrosoftExtensionsDependencyInjectionPackageName}'"),
                    }
                );
        }

        public static NugetPackageInfo MicrosoftExtensionsConfigurationAbstractions(IOutputTarget outputTarget) => NugetRegistry.GetVersion(MicrosoftExtensionsConfigurationAbstractionsPackageName, outputTarget.GetMaxNetAppVersion());

        public static NugetPackageInfo MicrosoftExtensionsDependencyInjection(IOutputTarget outputTarget) => NugetRegistry.GetVersion(MicrosoftExtensionsDependencyInjectionPackageName, outputTarget.GetMaxNetAppVersion());

        public static NugetPackageInfo MicrosoftExtensionsConfigurationBinder(IOutputTarget outputTarget) => NugetRegistry.GetVersion(MicrosoftExtensionsConfigurationBinderPackageName, outputTarget.GetMaxNetAppVersion());
    }
}
