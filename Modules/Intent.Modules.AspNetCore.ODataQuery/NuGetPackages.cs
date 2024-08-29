using System;
using Intent.Engine;
using Intent.Modules.Common.CSharp.Nuget;
using Intent.Modules.Common.VisualStudio;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.NugetPackages", Version = "1.0")]

namespace Intent.Modules.AspNetCore.ODataQuery
{
    public class NugetPackages : INugetPackages
    {
        public const string MicrosoftAspNetCoreODataPackageName = "Microsoft.AspNetCore.OData";

        public void RegisterPackages()
        {
            NugetRegistry.Register(MicrosoftAspNetCoreODataPackageName,
                (framework) => framework switch
                    {
                        ( >= 6, 0) => new PackageVersion("8.2.5"),
                        _ => throw new Exception($"Unsupported Framework `{framework.Major}` for NuGet package '{MicrosoftAspNetCoreODataPackageName}'"),
                    }
                );
        }

        public static NugetPackageInfo MicrosoftAspNetCoreOData(IOutputTarget outputTarget) => NugetRegistry.GetVersion(MicrosoftAspNetCoreODataPackageName, outputTarget.GetMaxNetAppVersion());
    }
}
