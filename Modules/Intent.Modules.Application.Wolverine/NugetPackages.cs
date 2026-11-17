using System;
using Intent.Engine;
using Intent.Modules.Common.CSharp.Nuget;
using Intent.Modules.Common.VisualStudio;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.NugetPackages", Version = "1.0")]

namespace Intent.Modules.Application.Wolverine
{
    public class NugetPackages : INugetPackages
    {
        public const string WolverineFxPackageName = "WolverineFx";

        public void RegisterPackages()
        {
            NugetRegistry.Register(WolverineFxPackageName,
                (framework) => (framework.Major, framework.Minor) switch
                    {
                        ( >= 8, >= 0) => new PackageVersion("5.41.0", locked: true)
                            .WithNugetDependency("JasperFx", "1.31.0")
                            .WithNugetDependency("JasperFx.Events", "1.36.0")
                            .WithNugetDependency("JasperFx.RuntimeCompiler", "4.5.0")
                            .WithNugetDependency("JasperFx.SourceGeneration", "1.1.0")
                            .WithNugetDependency("Microsoft.Extensions.Caching.Memory", "8.0.1")
                            .WithNugetDependency("Microsoft.Extensions.Configuration", "8.0.0")
                            .WithNugetDependency("Microsoft.Extensions.Configuration.Abstractions", "8.0.0")
                            .WithNugetDependency("Microsoft.Extensions.Configuration.Binder", "8.0.2")
                            .WithNugetDependency("Microsoft.Extensions.Configuration.Json", "8.0.1")
                            .WithNugetDependency("Microsoft.Extensions.Diagnostics.HealthChecks.Abstractions", "8.0.8")
                            .WithNugetDependency("Microsoft.Extensions.Hosting", "8.0.1")
                            .WithNugetDependency("Microsoft.Extensions.Hosting.Abstractions", "8.0.1")
                            .WithNugetDependency("Microsoft.Extensions.ObjectPool", "8.0.23")
                            .WithNugetDependency("NewId", "4.0.1")
                            .WithNugetDependency("Newtonsoft.Json", "13.0.3")
                            .WithNugetDependency("System.Diagnostics.DiagnosticSource", "9.0.5")
                            .WithNugetDependency("System.Threading.Tasks.Dataflow", "9.0.5"),
                        _ => throw new Exception($"Unsupported Framework `{framework.Major}` for NuGet package '{WolverineFxPackageName}'"),
                    }
                );
        }

        public static NugetPackageInfo WolverineFx(IOutputTarget outputTarget) => NugetRegistry.GetVersion(WolverineFxPackageName, outputTarget.GetMaxNetAppVersion());
    }
}