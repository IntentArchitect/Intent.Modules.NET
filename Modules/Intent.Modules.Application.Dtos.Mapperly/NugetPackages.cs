using System;
using Intent.Engine;
using Intent.Modules.Common.CSharp.Nuget;
using Intent.Modules.Common.VisualStudio;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.NugetPackages", Version = "1.0")]

namespace Intent.Modules.Application.Dtos.Mapperly
{
    public class NugetPackages : INugetPackages
    {
        public const string RiokMapperlyPackageName = "Riok.Mapperly";

        public void RegisterPackages()
        {
            NugetRegistry.Register(RiokMapperlyPackageName,
                (framework) => (framework.Major, framework.Minor) switch
                    {
                        ( >= 2, >= 0) => new PackageVersion("4.2.1"),
                        _ => throw new Exception($"Unsupported Framework `{framework.Major}` for NuGet package '{RiokMapperlyPackageName}'"),
                    }
                );
        }

        public static NugetPackageInfo RiokMapperly(IOutputTarget outputTarget) => NugetRegistry.GetVersion(RiokMapperlyPackageName, outputTarget.GetMaxNetAppVersion());
    }
}