using System;
using Intent.Engine;
using Intent.Modules.Common.CSharp.Nuget;
using Intent.Modules.Common.VisualStudio;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.NugetPackages", Version = "1.0")]

namespace Intent.Modules.Blazor.FluentValidation
{
    public class NugetPackages : INugetPackages
    {
        public const string FluentValidationPackageName = "FluentValidation";
        public const string FluentValidationDependencyInjectionExtensionsPackageName = "FluentValidation.DependencyInjectionExtensions";

        public void RegisterPackages()
        {
            NugetRegistry.Register(FluentValidationPackageName,
                (framework) => framework switch
                    {
                        ( >= 8, 0) => new PackageVersion("11.9.2"),
                        ( >= 7, 0) => new PackageVersion("11.9.2"),
                        ( >= 6, 0) => new PackageVersion("11.9.2"),
                        _ => throw new Exception($"Unsupported Framework `{framework.Major}` for NuGet package '{FluentValidationPackageName}'"),
                    }
                );
            NugetRegistry.Register(FluentValidationDependencyInjectionExtensionsPackageName,
                (framework) => framework switch
                    {
                        ( >= 2, 0) => new PackageVersion("11.9.2"),
                        _ => throw new Exception($"Unsupported Framework `{framework.Major}` for NuGet package '{FluentValidationDependencyInjectionExtensionsPackageName}'"),
                    }
                );
        }

        public static NugetPackageInfo FluentValidation(IOutputTarget outputTarget) => NugetRegistry.GetVersion(FluentValidationPackageName, outputTarget.GetMaxNetAppVersion());

        public static NugetPackageInfo FluentValidationDependencyInjectionExtensions(IOutputTarget outputTarget) => NugetRegistry.GetVersion(FluentValidationDependencyInjectionExtensionsPackageName, outputTarget.GetMaxNetAppVersion());
    }
}