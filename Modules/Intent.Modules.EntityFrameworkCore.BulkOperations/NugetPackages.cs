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
                        ( >= 8, 0) => new PackageVersion("8.103.1"),
                        ( >= 6, 0) => new PackageVersion("7.103.1"),
                        _ => throw new Exception($"Unsupported Framework `{framework.Major}` for NuGet package '{ZEntityFrameworkExtensionsEFCorePackageName}'"),
                    }
                );
        }

        public static NugetPackageInfo ZEntityFrameworkExtensionsEFCore(IOutputTarget outputTarget) => NugetRegistry.GetVersion(ZEntityFrameworkExtensionsEFCorePackageName, outputTarget.GetMaxNetAppVersion());
    }
}
