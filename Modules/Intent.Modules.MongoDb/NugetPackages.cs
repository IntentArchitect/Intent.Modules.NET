using System;
using Intent.Engine;
using Intent.Modules.Common.CSharp.Nuget;
using Intent.Modules.Common.VisualStudio;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.NugetPackages", Version = "1.0")]

namespace Intent.Modules.MongoDb
{
    public class NugetPackages : INugetPackages
    {
        public const string MongoFrameworkPackageName = "MongoFramework";

        public void RegisterPackages()
        {
            NugetRegistry.Register(MongoFrameworkPackageName,
                (framework) => framework switch
                    {
                        ( >= 6, 0) => new PackageVersion("0.29.0"),
                        _ => throw new Exception($"Unsupported Framework `{framework.Major}` for NuGet package '{MongoFrameworkPackageName}'"),
                    }
                );
        }

        public static NugetPackageInfo MongoFramework(IOutputTarget outputTarget) => NugetRegistry.GetVersion(MongoFrameworkPackageName, outputTarget.GetMaxNetAppVersion());
    }
}
