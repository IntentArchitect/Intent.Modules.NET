using System;
using Intent.Engine;
using Intent.Modules.Common.CSharp.Nuget;
using Intent.Modules.Common.VisualStudio;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.NugetPackages", Version = "1.0")]

namespace Intent.Modules.Application.MediatR.Behaviours
{
    public class NugetPackages : INugetPackages
    {
        public const string MicrosoftExtensionsLoggingPackageName = "Microsoft.Extensions.Logging";

        public void RegisterPackages()
        {
            NugetRegistry.Register(MicrosoftExtensionsLoggingPackageName,
                (framework) => (framework.Major, framework.Minor) switch
                    {
                        ( >= 9, >= 0) => new PackageVersion("10.0.0")
                            .WithNugetDependency("Microsoft.Extensions.DependencyInjection", "10.0.0")
                            .WithNugetDependency("Microsoft.Extensions.Logging.Abstractions", "10.0.0")
                            .WithNugetDependency("Microsoft.Extensions.Options", "10.0.0"),
                        ( >= 8, >= 0) => new PackageVersion("10.0.0")
                            .WithNugetDependency("Microsoft.Extensions.DependencyInjection", "10.0.0")
                            .WithNugetDependency("Microsoft.Extensions.Logging.Abstractions", "10.0.0")
                            .WithNugetDependency("Microsoft.Extensions.Options", "10.0.0"),
                        ( >= 2, >= 1) => new PackageVersion("10.0.0")
                            .WithNugetDependency("Microsoft.Extensions.DependencyInjection", "10.0.0")
                            .WithNugetDependency("Microsoft.Extensions.Logging.Abstractions", "10.0.0")
                            .WithNugetDependency("Microsoft.Extensions.Options", "10.0.0")
                            .WithNugetDependency("System.Diagnostics.DiagnosticSource", "10.0.0"),
                        ( >= 2, >= 0) => new PackageVersion("10.0.0")
                            .WithNugetDependency("Microsoft.Bcl.AsyncInterfaces", "10.0.0")
                            .WithNugetDependency("Microsoft.Extensions.DependencyInjection", "10.0.0")
                            .WithNugetDependency("Microsoft.Extensions.Logging.Abstractions", "10.0.0")
                            .WithNugetDependency("Microsoft.Extensions.Options", "10.0.0")
                            .WithNugetDependency("System.Diagnostics.DiagnosticSource", "10.0.0"),
                        _ => throw new Exception($"Unsupported Framework `{framework.Major}` for NuGet package '{MicrosoftExtensionsLoggingPackageName}'"),
                    }
                );
        }

        public static NugetPackageInfo MicrosoftExtensionsLogging(IOutputTarget outputTarget) => NugetRegistry.GetVersion(MicrosoftExtensionsLoggingPackageName, outputTarget.GetMaxNetAppVersion());
    }
}
