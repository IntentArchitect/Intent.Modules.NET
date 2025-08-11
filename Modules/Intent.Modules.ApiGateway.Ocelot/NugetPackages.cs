using System;
using Intent.Engine;
using Intent.Modules.Common.CSharp.Nuget;
using Intent.Modules.Common.VisualStudio;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.NugetPackages", Version = "1.0")]

namespace Intent.Modules.ApiGateway.Ocelot
{
    public class NugetPackages : INugetPackages
    {
        public const string OcelotPackageName = "Ocelot";

        public void RegisterPackages()
        {
            NugetRegistry.Register(OcelotPackageName,
                (framework) => (framework.Major, framework.Minor) switch
                    {
                        ( >= 9, >= 0) => new PackageVersion("24.0.1")
                            .WithNugetDependency("FluentValidation", "11.11.0")
                            .WithNugetDependency("IPAddressRange", "6.2.0")
                            .WithNugetDependency("Microsoft.AspNetCore.MiddlewareAnalysis", "9.0.4")
                            .WithNugetDependency("Microsoft.AspNetCore.Mvc.NewtonsoftJson", "9.0.4")
                            .WithNugetDependency("Microsoft.Extensions.DiagnosticAdapter", "3.1.32"),
                        ( >= 8, >= 0) => new PackageVersion("24.0.1")
                            .WithNugetDependency("FluentValidation", "11.11.0")
                            .WithNugetDependency("IPAddressRange", "6.2.0")
                            .WithNugetDependency("Microsoft.AspNetCore.MiddlewareAnalysis", "8.0.15")
                            .WithNugetDependency("Microsoft.AspNetCore.Mvc.NewtonsoftJson", "8.0.15")
                            .WithNugetDependency("Microsoft.Extensions.DiagnosticAdapter", "3.1.32"),
                        ( >= 7, >= 0) => new PackageVersion("23.4.3")
                            .WithNugetDependency("FluentValidation", "11.11.0")
                            .WithNugetDependency("IPAddressRange", "6.1.0")
                            .WithNugetDependency("Microsoft.AspNetCore.MiddlewareAnalysis", "7.0.20")
                            .WithNugetDependency("Microsoft.AspNetCore.Mvc.NewtonsoftJson", "7.0.20")
                            .WithNugetDependency("Microsoft.Extensions.DiagnosticAdapter", "3.1.32"),
                        ( >= 6, >= 0) => new PackageVersion("23.4.3")
                            .WithNugetDependency("FluentValidation", "11.11.0")
                            .WithNugetDependency("IPAddressRange", "6.1.0")
                            .WithNugetDependency("Microsoft.AspNetCore.MiddlewareAnalysis", "6.0.36")
                            .WithNugetDependency("Microsoft.AspNetCore.Mvc.NewtonsoftJson", "6.0.36")
                            .WithNugetDependency("Microsoft.Extensions.DiagnosticAdapter", "3.1.32"),
                        _ => throw new Exception($"Unsupported Framework `{framework.Major}` for NuGet package '{OcelotPackageName}'"),
                    }
                );
        }

        public static NugetPackageInfo Ocelot(IOutputTarget outputTarget) => NugetRegistry.GetVersion(OcelotPackageName, outputTarget.GetMaxNetAppVersion());
    }
}