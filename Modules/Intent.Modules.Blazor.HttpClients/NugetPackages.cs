using System;
using Intent.Engine;
using Intent.Modules.Common.CSharp.Nuget;
using Intent.Modules.Common.VisualStudio;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.NugetPackages", Version = "1.0")]

namespace Intent.Modules.Blazor.HttpClients
{
    public class NugetPackages : INugetPackages
    {
        public const string IdentityModelAspNetCorePackageName = "IdentityModel.AspNetCore";
        public const string MicrosoftAspNetCoreWebUtilitiesPackageName = "Microsoft.AspNetCore.WebUtilities";
        public const string MicrosoftExtensionsHttpPackageName = "Microsoft.Extensions.Http";
        public const string SystemTextJsonPackageName = "System.Text.Json";

        public void RegisterPackages()
        {
            NugetRegistry.Register(IdentityModelAspNetCorePackageName,
                (framework) => (framework.Major, framework.Minor) switch
                    {
                        ( >= 6, >= 0) => new PackageVersion("4.3.0", locked: true),
                        ( >= 2, >= 0) => new PackageVersion("2.0.0", locked: true),
                        _ => throw new Exception($"Unsupported Framework `{framework.Major}` for NuGet package '{IdentityModelAspNetCorePackageName}'"),
                    }
                );
            NugetRegistry.Register(MicrosoftAspNetCoreWebUtilitiesPackageName,
                (framework) => (framework.Major, framework.Minor) switch
                    {
                        ( >= 9, >= 0) => new PackageVersion("9.0.9")
                            .WithNugetDependency("Microsoft.Net.Http.Headers", "9.0.9"),
                        ( >= 8, >= 0) => new PackageVersion("8.0.20")
                            .WithNugetDependency("Microsoft.Net.Http.Headers", "8.0.20")
                            .WithNugetDependency("System.IO.Pipelines", "8.0.0"),
                        ( >= 2, >= 0) => new PackageVersion("2.3.0")
                            .WithNugetDependency("Microsoft.Net.Http.Headers", "2.3.0")
                            .WithNugetDependency("System.Text.Encodings.Web", "8.0.0"),
                        _ => throw new Exception($"Unsupported Framework `{framework.Major}` for NuGet package '{MicrosoftAspNetCoreWebUtilitiesPackageName}'"),
                    }
                );
            NugetRegistry.Register(MicrosoftExtensionsHttpPackageName,
                (framework) => (framework.Major, framework.Minor) switch
                    {
                        ( >= 9, >= 0) => new PackageVersion("9.0.9")
                            .WithNugetDependency("Microsoft.Extensions.Configuration.Abstractions", "9.0.9")
                            .WithNugetDependency("Microsoft.Extensions.DependencyInjection.Abstractions", "9.0.9")
                            .WithNugetDependency("Microsoft.Extensions.Diagnostics", "9.0.9")
                            .WithNugetDependency("Microsoft.Extensions.Logging", "9.0.9")
                            .WithNugetDependency("Microsoft.Extensions.Logging.Abstractions", "9.0.9")
                            .WithNugetDependency("Microsoft.Extensions.Options", "9.0.9"),
                        ( >= 8, >= 0) => new PackageVersion("9.0.9")
                            .WithNugetDependency("Microsoft.Extensions.Configuration.Abstractions", "9.0.9")
                            .WithNugetDependency("Microsoft.Extensions.DependencyInjection.Abstractions", "9.0.9")
                            .WithNugetDependency("Microsoft.Extensions.Diagnostics", "9.0.9")
                            .WithNugetDependency("Microsoft.Extensions.Logging", "9.0.9")
                            .WithNugetDependency("Microsoft.Extensions.Logging.Abstractions", "9.0.9")
                            .WithNugetDependency("Microsoft.Extensions.Options", "9.0.9"),
                        ( >= 2, >= 0) => new PackageVersion("9.0.9")
                            .WithNugetDependency("Microsoft.Extensions.Configuration.Abstractions", "9.0.9")
                            .WithNugetDependency("Microsoft.Extensions.DependencyInjection.Abstractions", "9.0.9")
                            .WithNugetDependency("Microsoft.Extensions.Logging", "9.0.9")
                            .WithNugetDependency("Microsoft.Extensions.Logging.Abstractions", "9.0.9")
                            .WithNugetDependency("Microsoft.Extensions.Options", "9.0.9"),
                        _ => throw new Exception($"Unsupported Framework `{framework.Major}` for NuGet package '{MicrosoftExtensionsHttpPackageName}'"),
                    }
                );
            NugetRegistry.Register(SystemTextJsonPackageName,
                (framework) => (framework.Major, framework.Minor) switch
                    {
                        ( >= 9, >= 0) => new PackageVersion("9.0.9"),
                        ( >= 8, >= 0) => new PackageVersion("9.0.9")
                            .WithNugetDependency("System.IO.Pipelines", "9.0.9")
                            .WithNugetDependency("System.Text.Encodings.Web", "9.0.9"),
                        ( >= 2, >= 0) => new PackageVersion("9.0.9")
                            .WithNugetDependency("Microsoft.Bcl.AsyncInterfaces", "9.0.9")
                            .WithNugetDependency("System.Buffers", "4.5.1")
                            .WithNugetDependency("System.IO.Pipelines", "9.0.9")
                            .WithNugetDependency("System.Memory", "4.5.5")
                            .WithNugetDependency("System.Runtime.CompilerServices.Unsafe", "6.0.0")
                            .WithNugetDependency("System.Text.Encodings.Web", "9.0.9")
                            .WithNugetDependency("System.Threading.Tasks.Extensions", "4.5.4"),
                        _ => throw new Exception($"Unsupported Framework `{framework.Major}` for NuGet package '{SystemTextJsonPackageName}'"),
                    }
                );
        }

        public static NugetPackageInfo IdentityModelAspNetCore(IOutputTarget outputTarget) => NugetRegistry.GetVersion(IdentityModelAspNetCorePackageName, outputTarget.GetMaxNetAppVersion());

        public static NugetPackageInfo MicrosoftAspNetCoreWebUtilities(IOutputTarget outputTarget) => NugetRegistry.GetVersion(MicrosoftAspNetCoreWebUtilitiesPackageName, outputTarget.GetMaxNetAppVersion());

        public static NugetPackageInfo MicrosoftExtensionsHttp(IOutputTarget outputTarget) => NugetRegistry.GetVersion(MicrosoftExtensionsHttpPackageName, outputTarget.GetMaxNetAppVersion());

        public static NugetPackageInfo SystemTextJson(IOutputTarget outputTarget) => NugetRegistry.GetVersion(SystemTextJsonPackageName, outputTarget.GetMaxNetAppVersion());
    }
}