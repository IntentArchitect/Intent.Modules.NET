using System;
using Intent.Engine;
using Intent.Modules.Common.CSharp.Nuget;
using Intent.Modules.Common.VisualStudio;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.NugetPackages", Version = "1.0")]

namespace Intent.Modules.Integration.HttpClients.Stubs
{
    public class NugetPackages : INugetPackages
    {
        public const string MicrosoftExtensionsConfigurationBinderPackageName = "Microsoft.Extensions.Configuration.Binder";
        public const string MicrosoftExtensionsDependencyInjectionAbstractionsPackageName = "Microsoft.Extensions.DependencyInjection.Abstractions";

        public void RegisterPackages()
        {
            NugetRegistry.Register(MicrosoftExtensionsConfigurationBinderPackageName,
                (framework) => (framework.Major, framework.Minor) switch
                    {
                        ( >= 10, >= 0) => new PackageVersion("11.0.0")
                            .WithNugetDependency("Microsoft.Extensions.Configuration", "11.0.0")
                            .WithNugetDependency("Microsoft.Extensions.Configuration.Abstractions", "11.0.0"),
                        ( >= 2, >= 0) => new PackageVersion("10.0.15")
                            .WithNugetDependency("Microsoft.Extensions.Configuration", "10.0.15")
                            .WithNugetDependency("Microsoft.Extensions.Configuration.Abstractions", "10.0.15"),
                        _ => throw new Exception($"Unsupported Framework `{framework.Major}` for NuGet package '{MicrosoftExtensionsConfigurationBinderPackageName}'"),
                    }
                );
            NugetRegistry.Register(MicrosoftExtensionsDependencyInjectionAbstractionsPackageName,
                (framework) => (framework.Major, framework.Minor) switch
                    {
                        ( >= 10, >= 0) => new PackageVersion("11.0.0"),
                        ( >= 2, >= 0) => new PackageVersion("10.0.15")
                            .WithNugetDependency("Microsoft.Bcl.AsyncInterfaces", "10.0.15")
                            .WithNugetDependency("System.Threading.Tasks.Extensions", "4.6.3"),
                        _ => throw new Exception($"Unsupported Framework `{framework.Major}` for NuGet package '{MicrosoftExtensionsDependencyInjectionAbstractionsPackageName}'"),
                    }
                );
        }

        public static NugetPackageInfo MicrosoftExtensionsConfigurationBinder(IOutputTarget outputTarget) => NugetRegistry.GetVersion(MicrosoftExtensionsConfigurationBinderPackageName, outputTarget.GetMaxNetAppVersion());

        public static NugetPackageInfo MicrosoftExtensionsDependencyInjectionAbstractions(IOutputTarget outputTarget) => NugetRegistry.GetVersion(MicrosoftExtensionsDependencyInjectionAbstractionsPackageName, outputTarget.GetMaxNetAppVersion());
    }
}
