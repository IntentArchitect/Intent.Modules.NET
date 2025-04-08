using System;
using Intent.Engine;
using Intent.Modules.Common.CSharp.Nuget;
using Intent.Modules.Common.VisualStudio;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.NugetPackages", Version = "1.0")]

namespace Intent.Modules.Ardalis.Repositories
{
    public class NugetPackages : INugetPackages
    {
        public const string ArdalisSpecificationPackageName = "Ardalis.Specification";
        public const string ArdalisSpecificationEntityFrameworkCorePackageName = "Ardalis.Specification.EntityFrameworkCore";

        public void RegisterPackages()
        {
            NugetRegistry.Register(ArdalisSpecificationPackageName,
                (framework) => framework switch
                    {
                        ( >= 9, 0) => new PackageVersion("9.0.1"),
                        ( >= 8, 0) => new PackageVersion("9.0.1"),
                        ( >= 2, 0) => new PackageVersion("9.0.1")
                            .WithNugetDependency("System.Buffers", "4.6.0")
                            .WithNugetDependency("System.Memory", "4.6.0"),
                        _ => throw new Exception($"Unsupported Framework `{framework.Major}` for NuGet package '{ArdalisSpecificationPackageName}'"),
                    }
                );
            NugetRegistry.Register(ArdalisSpecificationEntityFrameworkCorePackageName,
                (framework) => framework switch
                    {
                        ( >= 9, 0) => new PackageVersion("9.0.1")
                            .WithNugetDependency("Ardalis.Specification", "9.0.1")
                            .WithNugetDependency("Microsoft.EntityFrameworkCore", "9.0.2")
                            .WithNugetDependency("Microsoft.EntityFrameworkCore.Relational", "9.0.2"),
                        ( >= 8, 0) => new PackageVersion("9.0.1")
                            .WithNugetDependency("Ardalis.Specification", "9.0.1")
                            .WithNugetDependency("Microsoft.EntityFrameworkCore", "8.0.13")
                            .WithNugetDependency("Microsoft.EntityFrameworkCore.Relational", "8.0.13"),
                        ( >= 7, 0) => new PackageVersion("8.0.0"),
                        ( >= 6, 0) => new PackageVersion("8.0.0"),
                        ( >= 2, 1) => new PackageVersion("6.1.0")
                            .WithNugetDependency("Ardalis.Specification", "6.1.0")
                            .WithNugetDependency("Microsoft.EntityFrameworkCore", "5.0.13")
                            .WithNugetDependency("Microsoft.EntityFrameworkCore.Relational", "5.0.13"),
                        ( >= 2, 0) => new PackageVersion("6.1.0")
                            .WithNugetDependency("Ardalis.Specification", "6.1.0")
                            .WithNugetDependency("Microsoft.EntityFrameworkCore", "3.1.18"),
                        _ => throw new Exception($"Unsupported Framework `{framework.Major}` for NuGet package '{ArdalisSpecificationEntityFrameworkCorePackageName}'"),
                    }
                );
        }

        public static NugetPackageInfo ArdalisSpecification(IOutputTarget outputTarget) => NugetRegistry.GetVersion(ArdalisSpecificationPackageName, outputTarget.GetMaxNetAppVersion());

        public static NugetPackageInfo ArdalisSpecificationEntityFrameworkCore(IOutputTarget outputTarget) => NugetRegistry.GetVersion(ArdalisSpecificationEntityFrameworkCorePackageName, outputTarget.GetMaxNetAppVersion());
    }
}
