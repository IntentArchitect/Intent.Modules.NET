using System;
using Intent.Engine;
using Intent.Modules.Common.CSharp.Nuget;
using Intent.Modules.Common.VisualStudio;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.NugetPackages", Version = "1.0")]

namespace Intent.Modules.FastEndpoints
{
    public class NugetPackages : INugetPackages
    {
        public const string FastEndpointsPackageName = "FastEndpoints";
        public const string FastEndpointsSwaggerSwashbucklePackageName = "FastEndpoints.Swagger.Swashbuckle";

        public void RegisterPackages()
        {
            NugetRegistry.Register(FastEndpointsPackageName,
                (framework) => framework switch
                    {
                        ( >= 6, 0) => new PackageVersion("5.30.0")
                            .WithNugetDependency("FastEndpoints.Attributes", "5.30.0")
                            .WithNugetDependency("FastEndpoints.Messaging.Core", "5.30.0")
                            .WithNugetDependency("FluentValidation", "11.10.0"),
                        _ => throw new Exception($"Unsupported Framework `{framework.Major}` for NuGet package '{FastEndpointsPackageName}'"),
                    }
                );
            NugetRegistry.Register(FastEndpointsSwaggerSwashbucklePackageName,
                (framework) => framework switch
                    {
                        ( >= 6, 0) => new PackageVersion("2.3.0")
                            .WithNugetDependency("FastEndpoints", "5.22.0")
                            .WithNugetDependency("FastEndpoints.ApiExplorer", "2.3.0")
                            .WithNugetDependency("Swashbuckle.AspNetCore.SwaggerGen", "6.5.0"),
                        _ => throw new Exception($"Unsupported Framework `{framework.Major}` for NuGet package '{FastEndpointsSwaggerSwashbucklePackageName}'"),
                    }
                );
        }

        public static NugetPackageInfo FastEndpoints(IOutputTarget outputTarget) => NugetRegistry.GetVersion(FastEndpointsPackageName, outputTarget.GetMaxNetAppVersion());

        public static NugetPackageInfo FastEndpointsSwaggerSwashbuckle(IOutputTarget outputTarget) => NugetRegistry.GetVersion(FastEndpointsSwaggerSwashbucklePackageName, outputTarget.GetMaxNetAppVersion());
    }
}