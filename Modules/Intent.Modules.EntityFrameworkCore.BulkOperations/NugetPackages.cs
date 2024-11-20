using System;
using Intent.Engine;
using Intent.Modules.Common.CSharp.Nuget;
using Intent.Modules.Common.VisualStudio;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.NugetPackages", Version = "1.0")]

namespace Intent.Modules.EntityFrameworkCore.BulkOperations
{
    public class NugetPackages : INugetPackages
    {
        public const string ZEntityFrameworkExtensionsEFCorePackageName = "Z.EntityFramework.Extensions.EFCore";

        public void RegisterPackages()
        {
            NugetRegistry.Register(ZEntityFrameworkExtensionsEFCorePackageName,
                (framework) => framework switch
                    {
                        ( >= 9, 0) => new PackageVersion("9.103.6.2", locked: true)
                            .WithNugetDependency("Microsoft.EntityFrameworkCore.Relational", "9.0.0")
                            .WithNugetDependency("System.Configuration.ConfigurationManager", "9.0.0"),
                        ( >= 8, 0) => new PackageVersion("8.103.6.2", locked: true),
                        ( >= 6, 0) => new PackageVersion("7.103.6.2")
                            .WithNugetDependency("Microsoft.EntityFrameworkCore.Relational", "7.0.0")
                            .WithNugetDependency("System.Configuration.ConfigurationManager", "7.0.0"),
                        ( >= 2, 1) => new PackageVersion("5.103.6.2")
                            .WithNugetDependency("Microsoft.CSharp", "4.7.0")
                            .WithNugetDependency("Microsoft.EntityFrameworkCore.Relational", "5.0.0")
                            .WithNugetDependency("System.Configuration.ConfigurationManager", "5.0.0")
                            .WithNugetDependency("System.Reflection.Emit", "4.7.0")
                            .WithNugetDependency("System.Reflection.TypeExtensions", "4.7.0"),
                        ( >= 2, 0) => new PackageVersion("3.103.6.2")
                            .WithNugetDependency("Microsoft.CSharp", "4.6.0")
                            .WithNugetDependency("Microsoft.EntityFrameworkCore.Relational", "3.1.0")
                            .WithNugetDependency("System.Configuration.ConfigurationManager", "4.6.0")
                            .WithNugetDependency("System.Reflection.Emit", "4.6.0")
                            .WithNugetDependency("System.Reflection.TypeExtensions", "4.6.0"),
                        _ => throw new Exception($"Unsupported Framework `{framework.Major}` for NuGet package '{ZEntityFrameworkExtensionsEFCorePackageName}'"),
                    }
                );
        }

        public static NugetPackageInfo ZEntityFrameworkExtensionsEFCore(IOutputTarget outputTarget) => NugetRegistry.GetVersion(ZEntityFrameworkExtensionsEFCorePackageName, outputTarget.GetMaxNetAppVersion());
    }
}
