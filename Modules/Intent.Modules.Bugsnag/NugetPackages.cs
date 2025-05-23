using System;
using Intent.Engine;
using Intent.Modules.Common.CSharp.Nuget;
using Intent.Modules.Common.VisualStudio;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.NugetPackages", Version = "1.0")]

namespace Intent.Modules.Bugsnag
{
    public class NugetPackages : INugetPackages
    {
        public const string BugsnagAspNetCorePackageName = "Bugsnag.AspNet.Core";

        public void RegisterPackages()
        {
            NugetRegistry.Register(BugsnagAspNetCorePackageName,
                (framework) => (framework.Major, framework.Minor) switch
                    {
                        ( >= 6, >= 0) => new PackageVersion("4.0.0")
                            .WithNugetDependency("Bugsnag", "4.0.0")
                            .WithNugetDependency("Microsoft.Extensions.DiagnosticAdapter", "3.1.32"),
                        ( >= 2, >= 0) => new PackageVersion("4.0.0")
                            .WithNugetDependency("Bugsnag", "4.0.0")
                            .WithNugetDependency("Microsoft.AspNetCore.Diagnostics.Abstractions", "2.3.0")
                            .WithNugetDependency("Microsoft.AspNetCore.Hosting.Abstractions", "2.3.0")
                            .WithNugetDependency("Microsoft.AspNetCore.Http", "2.3.0")
                            .WithNugetDependency("Microsoft.AspNetCore.Http.Abstractions", "2.3.0")
                            .WithNugetDependency("Microsoft.AspNetCore.Http.Extensions", "2.3.0")
                            .WithNugetDependency("Microsoft.Extensions.DependencyInjection.Abstractions", "8.0.2")
                            .WithNugetDependency("Microsoft.Extensions.DiagnosticAdapter", "3.1.32")
                            .WithNugetDependency("Microsoft.Extensions.Options.ConfigurationExtensions", "8.0.0")
                            .WithNugetDependency("System.Diagnostics.DiagnosticSource", "8.0.1"),
                        _ => throw new Exception($"Unsupported Framework `{framework.Major}` for NuGet package '{BugsnagAspNetCorePackageName}'"),
                    }
                );
        }

        public static NugetPackageInfo BugsnagAspNetCore(IOutputTarget outputTarget) => NugetRegistry.GetVersion(BugsnagAspNetCorePackageName, outputTarget.GetMaxNetAppVersion());
    }
}
