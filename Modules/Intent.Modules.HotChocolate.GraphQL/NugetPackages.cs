using System;
using Intent.Engine;
using Intent.Modules.Common.CSharp.Nuget;
using Intent.Modules.Common.VisualStudio;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.NugetPackages", Version = "1.0")]

namespace Intent.Modules.HotChocolate.GraphQL
{
    public class NugetPackages : INugetPackages
    {
        public const string HotChocolatePackageName = "HotChocolate";

        public void RegisterPackages()
        {
            NugetRegistry.Register(HotChocolatePackageName,
                (framework) => framework switch
                    {
                        ( >= 8, 0) => new PackageVersion("13.9.9"),
                        ( >= 7, 0) => new PackageVersion("13.9.9"),
                        ( >= 6, 0) => new PackageVersion("13.9.9"),
                        _ => throw new Exception($"Unsupported Framework `{framework.Major}` for NuGet package '{HotChocolatePackageName}'"),
                    }
                );
        }

        public static NugetPackageInfo HotChocolate(IOutputTarget outputTarget) => NugetRegistry.GetVersion(HotChocolatePackageName, outputTarget.GetMaxNetAppVersion());
    }
}
