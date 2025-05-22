using System;
using Intent.Engine;
using Intent.Modules.Common.CSharp.Nuget;
using Intent.Modules.Common.VisualStudio;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.NugetPackages", Version = "1.0")]

namespace Intent.Modules.Blazor
{
    public class NugetPackages : INugetPackages
    {
        public const string MicrosoftAspNetCoreComponentsWebAssemblyPackageName = "Microsoft.AspNetCore.Components.WebAssembly";
        public const string MicrosoftAspNetCoreComponentsWebAssemblyServerPackageName = "Microsoft.AspNetCore.Components.WebAssembly.Server";

        public void RegisterPackages()
        {
            NugetRegistry.Register(MicrosoftAspNetCoreComponentsWebAssemblyPackageName,
                (framework) => (framework.Major, framework.Minor) switch
                    {
                        ( >= 9, >= 0) => new PackageVersion("9.0.5")
                            .WithNugetDependency("Microsoft.AspNetCore.Components.Web", "9.0.5")
                            .WithNugetDependency("Microsoft.Extensions.Configuration.Binder", "9.0.5")
                            .WithNugetDependency("Microsoft.Extensions.Configuration.Json", "9.0.5")
                            .WithNugetDependency("Microsoft.Extensions.Logging", "9.0.5")
                            .WithNugetDependency("Microsoft.JSInterop.WebAssembly", "9.0.5"),
                        ( >= 8, >= 0) => new PackageVersion("8.0.16")
                            .WithNugetDependency("Microsoft.AspNetCore.Components.Web", "8.0.16")
                            .WithNugetDependency("Microsoft.Extensions.Configuration.Binder", "8.0.2")
                            .WithNugetDependency("Microsoft.Extensions.Configuration.Json", "8.0.1")
                            .WithNugetDependency("Microsoft.Extensions.Logging", "8.0.1")
                            .WithNugetDependency("Microsoft.JSInterop.WebAssembly", "8.0.16"),
                        ( >= 7, >= 0) => new PackageVersion("7.0.20")
                            .WithNugetDependency("Microsoft.AspNetCore.Components.Web", "7.0.20")
                            .WithNugetDependency("Microsoft.Extensions.Configuration.Binder", "7.0.4")
                            .WithNugetDependency("Microsoft.Extensions.Configuration.Json", "7.0.0")
                            .WithNugetDependency("Microsoft.Extensions.Logging", "7.0.0")
                            .WithNugetDependency("Microsoft.JSInterop.WebAssembly", "7.0.20"),
                        ( >= 6, >= 0) => new PackageVersion("6.0.36")
                            .WithNugetDependency("Microsoft.AspNetCore.Components.Web", "6.0.36")
                            .WithNugetDependency("Microsoft.Extensions.Configuration.Binder", "6.0.1")
                            .WithNugetDependency("Microsoft.Extensions.Configuration.Json", "6.0.1")
                            .WithNugetDependency("Microsoft.Extensions.Logging", "6.0.1")
                            .WithNugetDependency("Microsoft.JSInterop.WebAssembly", "6.0.36"),
                        _ => throw new Exception($"Unsupported Framework `{framework.Major}` for NuGet package '{MicrosoftAspNetCoreComponentsWebAssemblyPackageName}'"),
                    }
                );
            NugetRegistry.Register(MicrosoftAspNetCoreComponentsWebAssemblyServerPackageName,
                (framework) => (framework.Major, framework.Minor) switch
                    {
                        ( >= 9, >= 0) => new PackageVersion("9.0.5"),
                        ( >= 8, >= 0) => new PackageVersion("8.0.16"),
                        ( >= 7, >= 0) => new PackageVersion("7.0.20"),
                        ( >= 6, >= 0) => new PackageVersion("6.0.36"),
                        _ => throw new Exception($"Unsupported Framework `{framework.Major}` for NuGet package '{MicrosoftAspNetCoreComponentsWebAssemblyServerPackageName}'"),
                    }
                );
        }

        public static NugetPackageInfo MicrosoftAspNetCoreComponentsWebAssembly(IOutputTarget outputTarget) => NugetRegistry.GetVersion(MicrosoftAspNetCoreComponentsWebAssemblyPackageName, outputTarget.GetMaxNetAppVersion());

        public static NugetPackageInfo MicrosoftAspNetCoreComponentsWebAssemblyServer(IOutputTarget outputTarget) => NugetRegistry.GetVersion(MicrosoftAspNetCoreComponentsWebAssemblyServerPackageName, outputTarget.GetMaxNetAppVersion());
    }
}