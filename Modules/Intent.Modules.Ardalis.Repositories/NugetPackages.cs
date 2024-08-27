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
                        ( >= 8, 0) => new PackageVersion("8.0.0"),
                        ( >= 7, 0) => new PackageVersion("8.0.0"),
                        ( >= 6, 0) => new PackageVersion("8.0.0"),
                        _ => throw new Exception($"Unsupported Framework `{framework.Major}` for NuGet package '{ArdalisSpecificationPackageName}'"),
                    }
                );
            NugetRegistry.Register(ArdalisSpecificationEntityFrameworkCorePackageName,
                (framework) => framework switch
                    {
                        ( >= 8, 0) => new PackageVersion("8.0.0"),
                        ( >= 7, 0) => new PackageVersion("8.0.0"),
                        ( >= 6, 0) => new PackageVersion("8.0.0"),
                        _ => throw new Exception($"Unsupported Framework `{framework.Major}` for NuGet package '{ArdalisSpecificationEntityFrameworkCorePackageName}'"),
                    }
                );
        }

        public static NugetPackageInfo ArdalisSpecification(IOutputTarget outputTarget) => NugetRegistry.GetVersion(ArdalisSpecificationPackageName, outputTarget.GetMaxNetAppVersion());

        public static NugetPackageInfo ArdalisSpecificationEntityFrameworkCore(IOutputTarget outputTarget) => NugetRegistry.GetVersion(ArdalisSpecificationEntityFrameworkCorePackageName, outputTarget.GetMaxNetAppVersion());
    }
}
