using System;
using Intent.Engine;
using Intent.Modules.Common.CSharp.Nuget;
using Intent.Modules.Common.VisualStudio;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.NugetPackages", Version = "1.0")]

namespace Intent.Modules.AspNetCore.Swashbuckle
{
    public class NugetPackages : INugetPackages
    {
        public const string SwashbuckleAspNetCorePackageName = "Swashbuckle.AspNetCore";

        public void RegisterPackages()
        {
            NugetRegistry.Register(SwashbuckleAspNetCorePackageName,
                (framework) => framework switch
                    {
                        ( >= 2, 0) => new PackageVersion("6.7.0"),
                        _ => throw new Exception($"Unsupported Framework `{framework.Major}` for NuGet package '{SwashbuckleAspNetCorePackageName}'"),
                    }
                );
        }

        public static NugetPackageInfo SwashbuckleAspNetCore(IOutputTarget outputTarget) => NugetRegistry.GetVersion(SwashbuckleAspNetCorePackageName, outputTarget.GetMaxNetAppVersion());
    }
}
