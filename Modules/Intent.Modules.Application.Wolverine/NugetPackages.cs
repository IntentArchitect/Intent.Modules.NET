using System;
using Intent.Engine;
using Intent.Modules.Common.CSharp.Nuget;
using Intent.Modules.Common.VisualStudio;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.NugetPackages", Version = "1.0")]

namespace Intent.Modules.Application.Wolverine
{
    public class NugetPackages : INugetPackages
    {
        public const string WolverineFxPackageName = "WolverineFx";

        public void RegisterPackages()
        {
            NugetRegistry.Register(WolverineFxPackageName,
                (framework) => (framework.Major, framework.Minor) switch
                    {
                        ( >= 8, >= 0) => new PackageVersion("5.39.5"),
                        _ => throw new Exception($"Unsupported Framework `{framework.Major}` for NuGet package '{WolverineFxPackageName}'"),
                    }
                );
        }

        public static NugetPackageInfo WolverineFx(IOutputTarget outputTarget) => NugetRegistry.GetVersion(WolverineFxPackageName, outputTarget.GetMaxNetAppVersion());
    }
}