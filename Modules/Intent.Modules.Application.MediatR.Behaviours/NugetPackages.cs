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
        public const string MediatRPackageName = "MediatR";
        public const string MicrosoftExtensionsLoggingPackageName = "Microsoft.Extensions.Logging";

        public void RegisterPackages()
        {
            NugetRegistry.Register(MediatRPackageName,
                (framework) => (framework.Major, framework.Minor) switch
                    {
                        ( >= 6, >= 0) => new PackageVersion("12.5.0")
                            .WithNugetDependency("MediatR.Contracts", "2.0.1")
                            .WithNugetDependency("Microsoft.Extensions.DependencyInjection.Abstractions", "8.0.0"),
                        ( >= 2, >= 0) => new PackageVersion("12.5.0")
                            .WithNugetDependency("MediatR.Contracts", "2.0.1")
                            .WithNugetDependency("Microsoft.Bcl.AsyncInterfaces", "8.0.0")
                            .WithNugetDependency("Microsoft.Extensions.DependencyInjection.Abstractions", "8.0.0"),
                        _ => throw new Exception($"Unsupported Framework `{framework.Major}` for NuGet package '{MediatRPackageName}'"),
                    }
                );
            NugetRegistry.Register(MicrosoftExtensionsLoggingPackageName,
                (framework) => (framework.Major, framework.Minor) switch
                    {
                        ( >= 9, >= 0) => new PackageVersion("9.0.5")
                            .WithNugetDependency("Microsoft.Extensions.DependencyInjection", "9.0.5")
                            .WithNugetDependency("Microsoft.Extensions.Logging.Abstractions", "9.0.5")
                            .WithNugetDependency("Microsoft.Extensions.Options", "9.0.5"),
                        ( >= 8, >= 0) => new PackageVersion("9.0.5")
                            .WithNugetDependency("Microsoft.Extensions.DependencyInjection", "9.0.5")
                            .WithNugetDependency("Microsoft.Extensions.Logging.Abstractions", "9.0.5")
                            .WithNugetDependency("Microsoft.Extensions.Options", "9.0.5"),
                        ( >= 2, >= 1) => new PackageVersion("9.0.5")
                            .WithNugetDependency("Microsoft.Extensions.DependencyInjection", "9.0.5")
                            .WithNugetDependency("Microsoft.Extensions.Logging.Abstractions", "9.0.5")
                            .WithNugetDependency("Microsoft.Extensions.Options", "9.0.5")
                            .WithNugetDependency("System.Diagnostics.DiagnosticSource", "9.0.5"),
                        ( >= 2, >= 0) => new PackageVersion("9.0.5")
                            .WithNugetDependency("Microsoft.Bcl.AsyncInterfaces", "9.0.5")
                            .WithNugetDependency("Microsoft.Extensions.DependencyInjection", "9.0.5")
                            .WithNugetDependency("Microsoft.Extensions.Logging.Abstractions", "9.0.5")
                            .WithNugetDependency("Microsoft.Extensions.Options", "9.0.5")
                            .WithNugetDependency("System.Diagnostics.DiagnosticSource", "9.0.5"),
                        _ => throw new Exception($"Unsupported Framework `{framework.Major}` for NuGet package '{MicrosoftExtensionsLoggingPackageName}'"),
                    }
                );
        }

        public static NugetPackageInfo MediatR(IOutputTarget outputTarget) => NugetRegistry.GetVersion(MediatRPackageName, outputTarget.GetMaxNetAppVersion());

        public static NugetPackageInfo MicrosoftExtensionsLogging(IOutputTarget outputTarget) => NugetRegistry.GetVersion(MicrosoftExtensionsLoggingPackageName, outputTarget.GetMaxNetAppVersion());
    }
}
