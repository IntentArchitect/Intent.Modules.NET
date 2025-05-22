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
        public const string FastEndpointsAspVersioningPackageName = "FastEndpoints.AspVersioning";
        public const string FastEndpointsAttributesPackageName = "FastEndpoints.Attributes";
        public const string FastEndpointsSwaggerSwashbucklePackageName = "FastEndpoints.Swagger.Swashbuckle";

        public void RegisterPackages()
        {
            NugetRegistry.Register(FastEndpointsPackageName,
                (framework) => (framework.Major, framework.Minor) switch
                    {
                        ( >= 9, >= 0) => new PackageVersion("6.0.0")
                            .WithNugetDependency("FastEndpoints.Attributes", "6.0.0")
                            .WithNugetDependency("FastEndpoints.Messaging.Core", "6.0.0")
                            .WithNugetDependency("FluentValidation", "11.11.0"),
                        ( >= 8, >= 0) => new PackageVersion("6.0.0")
                            .WithNugetDependency("FastEndpoints.Attributes", "6.0.0")
                            .WithNugetDependency("FastEndpoints.Messaging.Core", "6.0.0")
                            .WithNugetDependency("FluentValidation", "11.11.0"),
                        ( >= 7, >= 0) => new PackageVersion("5.35.0")
                            .WithNugetDependency("FastEndpoints.Attributes", "5.35.0")
                            .WithNugetDependency("FastEndpoints.Messaging.Core", "5.35.0")
                            .WithNugetDependency("FluentValidation", "11.11.0"),
                        ( >= 6, >= 0) => new PackageVersion("5.35.0")
                            .WithNugetDependency("FastEndpoints.Attributes", "5.35.0")
                            .WithNugetDependency("FastEndpoints.Messaging.Core", "5.35.0")
                            .WithNugetDependency("FluentValidation", "11.11.0"),
                        _ => throw new Exception($"Unsupported Framework `{framework.Major}` for NuGet package '{FastEndpointsPackageName}'"),
                    }
                );
            NugetRegistry.Register(FastEndpointsAspVersioningPackageName,
                (framework) => (framework.Major, framework.Minor) switch
                    {
                        ( >= 9, >= 0) => new PackageVersion("6.0.0")
                            .WithNugetDependency("Asp.Versioning.Http", "8.1.0")
                            .WithNugetDependency("Asp.Versioning.Mvc.ApiExplorer", "8.1.0")
                            .WithNugetDependency("FastEndpoints", "6.0.0")
                            .WithNugetDependency("NSwag.Generation.AspNetCore", "14.3.0"),
                        ( >= 8, >= 0) => new PackageVersion("6.0.0")
                            .WithNugetDependency("Asp.Versioning.Http", "8.1.0")
                            .WithNugetDependency("Asp.Versioning.Mvc.ApiExplorer", "8.1.0")
                            .WithNugetDependency("FastEndpoints", "6.0.0")
                            .WithNugetDependency("NSwag.Generation.AspNetCore", "14.3.0"),
                        ( >= 7, >= 0) => new PackageVersion("5.35.0")
                            .WithNugetDependency("Asp.Versioning.Http", "7.1.0")
                            .WithNugetDependency("Asp.Versioning.Mvc.ApiExplorer", "7.1.0")
                            .WithNugetDependency("FastEndpoints", "5.35.0")
                            .WithNugetDependency("NSwag.Generation.AspNetCore", "14.2.0"),
                        _ => throw new Exception($"Unsupported Framework `{framework.Major}` for NuGet package '{FastEndpointsAspVersioningPackageName}'"),
                    }
                );
            NugetRegistry.Register(FastEndpointsAttributesPackageName,
                (framework) => (framework.Major, framework.Minor) switch
                    {
                        ( >= 2, >= 0) => new PackageVersion("6.0.0")
                            .WithNugetDependency("Microsoft.Extensions.Primitives", "8.0.0"),
                        _ => throw new Exception($"Unsupported Framework `{framework.Major}` for NuGet package '{FastEndpointsAttributesPackageName}'"),
                    }
                );
            NugetRegistry.Register(FastEndpointsSwaggerSwashbucklePackageName,
                (framework) => (framework.Major, framework.Minor) switch
                    {
                        ( >= 8, >= 0) => new PackageVersion("2.3.0")
                            .WithNugetDependency("FastEndpoints", "5.22.0")
                            .WithNugetDependency("FastEndpoints.ApiExplorer", "2.3.0")
                            .WithNugetDependency("Swashbuckle.AspNetCore.SwaggerGen", "6.5.0"),
                        ( >= 6, >= 0) => new PackageVersion("2.3.0")
                            .WithNugetDependency("FastEndpoints", "5.22.0")
                            .WithNugetDependency("FastEndpoints.ApiExplorer", "2.3.0")
                            .WithNugetDependency("Swashbuckle.AspNetCore.SwaggerGen", "6.5.0"),
                        _ => throw new Exception($"Unsupported Framework `{framework.Major}` for NuGet package '{FastEndpointsSwaggerSwashbucklePackageName}'"),
                    }
                );
        }

        public static NugetPackageInfo FastEndpoints(IOutputTarget outputTarget) => NugetRegistry.GetVersion(FastEndpointsPackageName, outputTarget.GetMaxNetAppVersion());

        public static NugetPackageInfo FastEndpointsAspVersioning(IOutputTarget outputTarget) => NugetRegistry.GetVersion(FastEndpointsAspVersioningPackageName, outputTarget.GetMaxNetAppVersion());

        public static NugetPackageInfo FastEndpointsAttributes(IOutputTarget outputTarget) => NugetRegistry.GetVersion(FastEndpointsAttributesPackageName, outputTarget.GetMaxNetAppVersion());

        public static NugetPackageInfo FastEndpointsSwaggerSwashbuckle(IOutputTarget outputTarget) => NugetRegistry.GetVersion(FastEndpointsSwaggerSwashbucklePackageName, outputTarget.GetMaxNetAppVersion());
    }
}