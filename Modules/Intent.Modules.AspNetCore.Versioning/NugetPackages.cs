using System;
using Intent.Engine;
using Intent.Modules.Common.CSharp.Nuget;
using Intent.Modules.Common.VisualStudio;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.NugetPackages", Version = "1.0")]

namespace Intent.Modules.AspNetCore.Versioning
{
    public class NugetPackages : INugetPackages
    {
        public const string AspVersioningMvcPackageName = "Asp.Versioning.Mvc";
        public const string AspVersioningMvcApiExplorerPackageName = "Asp.Versioning.Mvc.ApiExplorer";

        public void RegisterPackages()
        {
            NugetRegistry.Register(AspVersioningMvcPackageName,
                (framework) => framework switch
                    {
                        ( >= 8, 0) => new PackageVersion("8.1.0"),
                        ( >= 7, 0) => new PackageVersion("7.1.1"),
                        ( >= 6, 0) => new PackageVersion("6.4.1"),
                        _ => throw new Exception($"Unsupported Framework `{framework.Major}` for NuGet package '{AspVersioningMvcPackageName}'"),
                    }
                );
            NugetRegistry.Register(AspVersioningMvcApiExplorerPackageName,
                (framework) => framework switch
                    {
                        ( >= 8, 0) => new PackageVersion("8.1.0"),
                        ( >= 7, 0) => new PackageVersion("7.1.0"),
                        ( >= 6, 0) => new PackageVersion("6.4.0"),
                        _ => throw new Exception($"Unsupported Framework `{framework.Major}` for NuGet package '{AspVersioningMvcApiExplorerPackageName}'"),
                    }
                );
        }

        public static NugetPackageInfo AspVersioningMvc(IOutputTarget outputTarget) => NugetRegistry.GetVersion(AspVersioningMvcPackageName, outputTarget.GetMaxNetAppVersion());

        public static NugetPackageInfo AspVersioningMvcApiExplorer(IOutputTarget outputTarget) => NugetRegistry.GetVersion(AspVersioningMvcApiExplorerPackageName, outputTarget.GetMaxNetAppVersion());
    }
}
