using System;
using Intent.Engine;
using Intent.Modules.Common.CSharp.Nuget;
using Intent.Modules.Common.VisualStudio;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.NugetPackages", Version = "1.0")]

namespace Intent.Modules.Ardalis.GuardClauses
{
    public class NugetPackages : INugetPackages
    {
        public const string ArdalisGuardClausesPackageName = "Ardalis.GuardClauses";

        public void RegisterPackages()
        {
            NugetRegistry.Register(ArdalisGuardClausesPackageName,
                (framework) => framework switch
                    {
                        ( >= 8, 0) => new PackageVersion("5.0.0"),
                        ( >= 2, 1) => new PackageVersion("5.0.0"),
                        ( >= 2, 0) => new PackageVersion("5.0.0"),
                        _ => throw new Exception($"Unsupported Framework `{framework.Major}` for NuGet package '{ArdalisGuardClausesPackageName}'"),
                    }
                );
        }

        public static NugetPackageInfo ArdalisGuardClauses(IOutputTarget outputTarget) => NugetRegistry.GetVersion(ArdalisGuardClausesPackageName, outputTarget.GetMaxNetAppVersion());
    }
}
