using System;
using Intent.Engine;
using Intent.Modules.Common.CSharp.Nuget;
using Intent.Modules.Common.VisualStudio;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.NugetPackages", Version = "1.0")]

namespace Intent.Modules.Application.MediatR.FluentValidation
{
    public class NugetPackages : INugetPackages
    {
        public const string FluentValidationPackageName = "FluentValidation";

        public void RegisterPackages()
        {
            NugetRegistry.Register(FluentValidationPackageName,
                (framework) => framework switch
                    {
                        ( >= 8, 0) => new PackageVersion("12.0.0"),
                        ( >= 7, 0) => new PackageVersion("11.11.0"),
                        ( >= 6, 0) => new PackageVersion("11.11.0"),
                        ( >= 2, 1) => new PackageVersion("11.11.0"),
                        ( >= 2, 0) => new PackageVersion("11.11.0")
                            .WithNugetDependency("System.Threading.Tasks.Extensions", "4.5.4"),
                        _ => throw new Exception($"Unsupported Framework `{framework.Major}` for NuGet package '{FluentValidationPackageName}'"),
                    }
                );
        }

        public static NugetPackageInfo FluentValidation(IOutputTarget outputTarget) => NugetRegistry.GetVersion(FluentValidationPackageName, outputTarget.GetMaxNetAppVersion());
    }
}
