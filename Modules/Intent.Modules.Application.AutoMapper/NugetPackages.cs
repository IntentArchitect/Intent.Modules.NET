using System;
using Intent.Engine;
using Intent.Modules.Common.CSharp.Nuget;
using Intent.Modules.Common.VisualStudio;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.NugetPackages", Version = "1.0")]

namespace Intent.Modules.Application.AutoMapper
{
    public class NugetPackages : INugetPackages
    {
        public const string AutoMapperPackageName = "AutoMapper";

        public void RegisterPackages()
        {
            NugetRegistry.Register(AutoMapperPackageName,
                (framework) => framework switch
                    {
                        ( >= 8, 0) => new PackageVersion("14.0.0")
                            .WithNugetDependency("Microsoft.Extensions.Options", "8.0.0"),
                        ( >= 6, 0) => new PackageVersion("13.0.1")
                            .WithNugetDependency("Microsoft.Extensions.Options", "6.0.0"),
                        ( >= 2, 1) => new PackageVersion("12.0.1")
                            .WithNugetDependency("Microsoft.CSharp", "4.7.0"),
                        ( >= 2, 0) => new PackageVersion("10.1.1")
                            .WithNugetDependency("Microsoft.CSharp", "4.7.0")
                            .WithNugetDependency("System.Reflection.Emit", "4.7.0"),
                        _ => throw new Exception($"Unsupported Framework `{framework.Major}` for NuGet package '{AutoMapperPackageName}'"),
                    }
                );
        }

        public static NugetPackageInfo AutoMapper(IOutputTarget outputTarget) => NugetRegistry.GetVersion(AutoMapperPackageName, outputTarget.GetMaxNetAppVersion());
    }
}
