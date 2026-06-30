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
                        ( >= 2, >= 0) => new PackageVersion("10.0.2")
                            .WithNugetDependency("Microsoft.Extensions.Configuration", "10.0.2")
                            .WithNugetDependency("Microsoft.Extensions.Configuration.Abstractions", "10.0.2"),
                        _ => throw new Exception($"Unsupported Framework `{framework.Major}` for NuGet package '{MicrosoftExtensionsConfigurationBinderPackageName}'"),
                    }
                );
            NugetRegistry.Register(MicrosoftExtensionsDependencyInjectionAbstractionsPackageName,
                (framework) => (framework.Major, framework.Minor) switch
                    {
                        ( >= 2, >= 0) => new PackageVersion("10.0.2")
                            .WithNugetDependency("Microsoft.Extensions.Primitives", "10.0.2"),
                        _ => throw new Exception($"Unsupported Framework `{framework.Major}` for NuGet package '{MicrosoftExtensionsDependencyInjectionAbstractionsPackageName}'"),
                    }
                );
        }

        public static NugetPackageInfo MicrosoftExtensionsConfigurationBinder(IOutputTarget outputTarget) => NugetRegistry.GetVersion(MicrosoftExtensionsConfigurationBinderPackageName, outputTarget.GetMaxNetAppVersion());

        public static NugetPackageInfo MicrosoftExtensionsDependencyInjectionAbstractions(IOutputTarget outputTarget) => NugetRegistry.GetVersion(MicrosoftExtensionsDependencyInjectionAbstractionsPackageName, outputTarget.GetMaxNetAppVersion());
    }
}
