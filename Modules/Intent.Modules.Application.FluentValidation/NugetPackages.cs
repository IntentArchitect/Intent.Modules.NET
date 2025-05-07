using System;
using Intent.Engine;
using Intent.Modules.Common.CSharp.Nuget;
using Intent.Modules.Common.VisualStudio;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.NugetPackages", Version = "1.0")]

namespace Intent.Modules.Application.FluentValidation
{
    public class NugetPackages : INugetPackages
    {
        public const string FluentValidationDependencyInjectionExtensionsPackageName = "FluentValidation.DependencyInjectionExtensions";

        public void RegisterPackages()
        {
            NugetRegistry.Register(FluentValidationDependencyInjectionExtensionsPackageName,
                (framework) => framework switch
                    {
                        ( >= 8, 0) => new PackageVersion("12.0.0")
                            .WithNugetDependency("FluentValidation", "12.0.0")
                            .WithNugetDependency("Microsoft.Extensions.Dependencyinjection.Abstractions", "2.1.0"),
                        ( >= 2, 1) => new PackageVersion("11.11.0")
                            .WithNugetDependency("FluentValidation", "11.11.0")
                            .WithNugetDependency("Microsoft.Extensions.Dependencyinjection.Abstractions", "2.1.0"),
                        ( >= 2, 0) => new PackageVersion("11.11.0")
                            .WithNugetDependency("FluentValidation", "11.11.0")
                            .WithNugetDependency("Microsoft.Extensions.Dependencyinjection.Abstractions", "2.1.0"),
                        _ => throw new Exception($"Unsupported Framework `{framework.Major}` for NuGet package '{FluentValidationDependencyInjectionExtensionsPackageName}'"),
                    }
                );
        }

        public static NugetPackageInfo FluentValidationDependencyInjectionExtensions(IOutputTarget outputTarget) => NugetRegistry.GetVersion(FluentValidationDependencyInjectionExtensionsPackageName, outputTarget.GetMaxNetAppVersion());
    }
}
