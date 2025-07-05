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
                (framework) => (framework.Major, framework.Minor) switch
                    {
                        ( >= 8, >= 0) => new PackageVersion("9.3.2")
                            .WithNugetDependency("Microsoft.OData.Core", "8.2.3")
                            .WithNugetDependency("Microsoft.OData.Edm", "8.2.3")
                            .WithNugetDependency("Microsoft.OData.ModelBuilder", "2.0.0")
                            .WithNugetDependency("Microsoft.Spatial", "8.2.3"),
                        ( >= 6, >= 0) => new PackageVersion("8.3.1")
                            .WithNugetDependency("Microsoft.OData.Core", "7.21.6")
                            .WithNugetDependency("Microsoft.OData.Edm", "7.21.6")
                            .WithNugetDependency("Microsoft.OData.ModelBuilder", "1.0.9")
                            .WithNugetDependency("Microsoft.Spatial", "7.21.6"),
                        ( >= 2, >= 0) => new PackageVersion("7.7.8")
                            .WithNugetDependency("Microsoft.AspNetCore.Mvc.Core", "2.0.1")
                            .WithNugetDependency("Microsoft.Extensions.DependencyInjection", "2.0.0")
                            .WithNugetDependency("Microsoft.OData.Core", "7.20.0"),
                        _ => throw new Exception($"Unsupported Framework `{framework.Major}` for NuGet package '{MicrosoftAspNetCoreODataPackageName}'"),
                    }
                );
        }

        public static NugetPackageInfo MicrosoftAspNetCoreOData(IOutputTarget outputTarget) => NugetRegistry.GetVersion(MicrosoftAspNetCoreODataPackageName, outputTarget.GetMaxNetAppVersion());
    }
}
