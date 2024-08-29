using System;
using Intent.Engine;
using Intent.Modules.Common.CSharp.Nuget;
using Intent.Modules.Common.VisualStudio;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.NugetPackages", Version = "1.0")]

namespace Intent.Modules.HotChocolate.GraphQL.AzureFunctions
{
    public class NugetPackages : INugetPackages
    {
        public const string HotChocolateAzureFunctionsPackageName = "HotChocolate.AzureFunctions";

        public void RegisterPackages()
        {
            NugetRegistry.Register(HotChocolateAzureFunctionsPackageName,
                (framework) => framework switch
                    {
                        ( >= 7, 0) => new PackageVersion("13.9.9"),
                        ( >= 6, 0) => new PackageVersion("13.9.9"),
                        _ => throw new Exception($"Unsupported Framework `{framework.Major}` for NuGet package '{HotChocolateAzureFunctionsPackageName}'"),
                    }
                );
        }

        public static NugetPackageInfo HotChocolateAzureFunctions(IOutputTarget outputTarget) => NugetRegistry.GetVersion(HotChocolateAzureFunctionsPackageName, outputTarget.GetMaxNetAppVersion());
    }
}
