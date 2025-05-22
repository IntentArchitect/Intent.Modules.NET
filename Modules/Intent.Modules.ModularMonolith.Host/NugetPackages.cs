using System;
using Intent.Engine;
using Intent.Modules.Common.CSharp.Nuget;
using Intent.Modules.Common.VisualStudio;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.NugetPackages", Version = "1.0")]

namespace Intent.Modules.ModularMonolith.Host
{
    public class NugetPackages : INugetPackages
    {
        public const string FluentValidationPackageName = "FluentValidation";
        public const string MassTransitPackageName = "MassTransit";
        public const string SwashbuckleAspNetCorePackageName = "Swashbuckle.AspNetCore";

        public void RegisterPackages()
        {
            NugetRegistry.Register(FluentValidationPackageName,
                (framework) => (framework.Major, framework.Minor) switch
                    {
                        ( >= 8, >= 0) => new PackageVersion("12.0.0"),
                        ( >= 7, >= 0) => new PackageVersion("11.11.0"),
                        ( >= 6, >= 0) => new PackageVersion("11.11.0"),
                        ( >= 2, >= 1) => new PackageVersion("11.11.0"),
                        ( >= 2, >= 0) => new PackageVersion("11.11.0")
                            .WithNugetDependency("System.Threading.Tasks.Extensions", "4.5.4"),
                        _ => throw new Exception($"Unsupported Framework `{framework.Major}` for NuGet package '{FluentValidationPackageName}'"),
                    }
                );
            NugetRegistry.Register(MassTransitPackageName,
                (framework) => (framework.Major, framework.Minor) switch
                    {
                        ( >= 9, >= 0) => new PackageVersion("8.4.1")
                            .WithNugetDependency("MassTransit.Abstractions", "8.4.1")
                            .WithNugetDependency("Microsoft.Extensions.DependencyInjection.Abstractions", "9.0.0")
                            .WithNugetDependency("Microsoft.Extensions.Diagnostics.HealthChecks", "9.0.0")
                            .WithNugetDependency("Microsoft.Extensions.Hosting.Abstractions", "9.0.0")
                            .WithNugetDependency("Microsoft.Extensions.Logging.Abstractions", "9.0.0")
                            .WithNugetDependency("Microsoft.Extensions.Options", "9.0.0"),
                        ( >= 8, >= 0) => new PackageVersion("8.4.1")
                            .WithNugetDependency("MassTransit.Abstractions", "8.4.1")
                            .WithNugetDependency("Microsoft.Extensions.DependencyInjection.Abstractions", "8.0.0")
                            .WithNugetDependency("Microsoft.Extensions.Diagnostics.HealthChecks", "8.0.0")
                            .WithNugetDependency("Microsoft.Extensions.Hosting.Abstractions", "8.0.0")
                            .WithNugetDependency("Microsoft.Extensions.Logging.Abstractions", "8.0.0")
                            .WithNugetDependency("Microsoft.Extensions.Options", "8.0.0"),
                        ( >= 2, >= 0) => new PackageVersion("8.4.1")
                            .WithNugetDependency("MassTransit.Abstractions", "8.4.1")
                            .WithNugetDependency("Microsoft.Bcl.AsyncInterfaces", "8.0.0")
                            .WithNugetDependency("Microsoft.Extensions.DependencyInjection.Abstractions", "8.0.0")
                            .WithNugetDependency("Microsoft.Extensions.Diagnostics.HealthChecks", "8.0.0")
                            .WithNugetDependency("Microsoft.Extensions.Hosting.Abstractions", "8.0.0")
                            .WithNugetDependency("Microsoft.Extensions.Logging.Abstractions", "8.0.0")
                            .WithNugetDependency("Microsoft.Extensions.Options", "8.0.0")
                            .WithNugetDependency("System.Diagnostics.DiagnosticSource", "8.0.0")
                            .WithNugetDependency("System.Memory", "4.6.3")
                            .WithNugetDependency("System.Reflection.Emit", "4.7.0")
                            .WithNugetDependency("System.Reflection.Emit.Lightweight", "4.7.0")
                            .WithNugetDependency("System.Text.Json", "8.0.5")
                            .WithNugetDependency("System.Threading.Channels", "8.0.0")
                            .WithNugetDependency("System.Threading.Tasks.Extensions", "4.6.3"),
                        _ => throw new Exception($"Unsupported Framework `{framework.Major}` for NuGet package '{MassTransitPackageName}'"),
                    }
                );
            NugetRegistry.Register(SwashbuckleAspNetCorePackageName,
                (framework) => (framework.Major, framework.Minor) switch
                    {
                        ( >= 2, >= 0) => new PackageVersion("8.1.1")
                            .WithNugetDependency("Microsoft.Extensions.ApiDescription.Server", "6.0.5")
                            .WithNugetDependency("Swashbuckle.AspNetCore.Swagger", "8.1.1")
                            .WithNugetDependency("Swashbuckle.AspNetCore.SwaggerGen", "8.1.1")
                            .WithNugetDependency("Swashbuckle.AspNetCore.SwaggerUI", "8.1.1"),
                        _ => throw new Exception($"Unsupported Framework `{framework.Major}` for NuGet package '{SwashbuckleAspNetCorePackageName}'"),
                    }
                );
        }

        public static NugetPackageInfo FluentValidation(IOutputTarget outputTarget) => NugetRegistry.GetVersion(FluentValidationPackageName, outputTarget.GetMaxNetAppVersion());

        public static NugetPackageInfo MassTransit(IOutputTarget outputTarget) => NugetRegistry.GetVersion(MassTransitPackageName, outputTarget.GetMaxNetAppVersion());

        public static NugetPackageInfo SwashbuckleAspNetCore(IOutputTarget outputTarget) => NugetRegistry.GetVersion(SwashbuckleAspNetCorePackageName, outputTarget.GetMaxNetAppVersion());
    }
}