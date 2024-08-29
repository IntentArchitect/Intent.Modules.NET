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
                        ( >= 6, 0) => new PackageVersion("13.0.1"),
                        _ => throw new Exception($"Unsupported Framework `{framework.Major}` for NuGet package '{AutoMapperPackageName}'"),
                    }
                );
        }

        public static NugetPackageInfo AutoMapper(IOutputTarget outputTarget) => NugetRegistry.GetVersion(AutoMapperPackageName, outputTarget.GetMaxNetAppVersion());
    }
}
