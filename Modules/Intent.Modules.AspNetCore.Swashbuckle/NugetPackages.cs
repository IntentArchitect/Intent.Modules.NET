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
                (framework) => (framework.Major, framework.Minor) switch
                    {
                        ( >= 9, >= 0) => new PackageVersion("9.0.6")
                            .WithNugetDependency("Swashbuckle.AspNetCore.Swagger", "9.0.6")
                            .WithNugetDependency("Swashbuckle.AspNetCore.SwaggerGen", "9.0.6")
                            .WithNugetDependency("Swashbuckle.AspNetCore.SwaggerUI", "9.0.6")
                            .WithNugetDependency("Microsoft.Extensions.ApiDescription.Server", "9.0.0"),
                        ( >= 8, >= 0) => new PackageVersion("9.0.6")
                            .WithNugetDependency("Swashbuckle.AspNetCore.Swagger", "9.0.6")
                            .WithNugetDependency("Swashbuckle.AspNetCore.SwaggerGen", "9.0.6")
                            .WithNugetDependency("Swashbuckle.AspNetCore.SwaggerUI", "9.0.6")
                            .WithNugetDependency("Microsoft.Extensions.ApiDescription.Server", "8.0.0"),
                        ( >= 2, >= 0) => new PackageVersion("8.1.4")
                            .WithNugetDependency("Microsoft.Extensions.ApiDescription.Server", "6.0.5")
                            .WithNugetDependency("Swashbuckle.AspNetCore.Swagger", "8.1.4")
                            .WithNugetDependency("Swashbuckle.AspNetCore.SwaggerGen", "8.1.4")
                            .WithNugetDependency("Swashbuckle.AspNetCore.SwaggerUI", "8.1.4"),
                        _ => throw new Exception($"Unsupported Framework `{framework.Major}` for NuGet package '{SwashbuckleAspNetCorePackageName}'"),
                    }
                );
        }

        public static NugetPackageInfo SwashbuckleAspNetCore(IOutputTarget outputTarget) => NugetRegistry.GetVersion(SwashbuckleAspNetCorePackageName, outputTarget.GetMaxNetAppVersion());
    }
}
