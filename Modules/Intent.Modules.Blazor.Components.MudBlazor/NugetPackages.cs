using System;
using Intent.Engine;
using Intent.Modules.Common.CSharp.Nuget;
using Intent.Modules.Common.VisualStudio;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.NugetPackages", Version = "1.0")]

namespace Intent.Modules.Blazor.Components.MudBlazor
{
    public class NugetPackages : INugetPackages
    {
        public const string MicrosoftAspNetCoreHttpAbstractionsPackageName = "Microsoft.AspNetCore.Http.Abstractions";
        public const string MudBlazorPackageName = "MudBlazor";

        public void RegisterPackages()
        {
            NugetRegistry.Register(MicrosoftAspNetCoreHttpAbstractionsPackageName,
                (framework) => (framework.Major, framework.Minor) switch
                    {
                        ( >= 2, >= 0) => new PackageVersion("2.3.11")
                            .WithNugetDependency("Microsoft.AspNetCore.Http.Features", "2.3.10")
                            .WithNugetDependency("System.Text.Encodings.Web", "8.0.0"),
                        _ => throw new Exception($"Unsupported Framework `{framework.Major}` for NuGet package '{MicrosoftAspNetCoreHttpAbstractionsPackageName}'"),
                    }
                );
            NugetRegistry.Register(MudBlazorPackageName,
                (framework) => (framework.Major, framework.Minor) switch
                    {
                        ( >= 9, >= 0) => new PackageVersion("8.15.0", locked: true)
                            .WithNugetDependency("Microsoft.AspNetCore.Components", "9.0.1")
                            .WithNugetDependency("Microsoft.AspNetCore.Components.Web", "9.0.1")
                            .WithNugetDependency("Microsoft.Extensions.Localization", "9.0.1"),
                        ( >= 8, >= 0) => new PackageVersion("8.15.0", locked: true)
                            .WithNugetDependency("Microsoft.AspNetCore.Components", "8.0.12")
                            .WithNugetDependency("Microsoft.AspNetCore.Components.Web", "8.0.12")
                            .WithNugetDependency("Microsoft.Extensions.Localization", "8.0.12"),
                        ( >= 7, >= 0) => new PackageVersion("7.16.0", locked: true)
                            .WithNugetDependency("Microsoft.AspNetCore.Components", "7.0.20")
                            .WithNugetDependency("Microsoft.AspNetCore.Components.Web", "7.0.20")
                            .WithNugetDependency("Microsoft.Extensions.Localization", "7.0.20"),
                        ( >= 6, >= 0) => new PackageVersion("6.21.0", locked: true)
                            .WithNugetDependency("Microsoft.AspNetCore.Components", "6.0.28")
                            .WithNugetDependency("Microsoft.AspNetCore.Components.Web", "6.0.28")
                            .WithNugetDependency("Microsoft.Extensions.Localization", "6.0.28"),
                        ( >= 2, >= 1) => new PackageVersion("2.0.7", locked: true)
                            .WithNugetDependency("Microsoft.AspNetCore.Components", "3.1.12")
                            .WithNugetDependency("Microsoft.AspNetCore.Components.Web", "3.1.12"),
                        _ => throw new Exception($"Unsupported Framework `{framework.Major}` for NuGet package '{MudBlazorPackageName}'"),
                    }
                );
        }

        public static NugetPackageInfo MicrosoftAspNetCoreHttpAbstractions(IOutputTarget outputTarget) => NugetRegistry.GetVersion(MicrosoftAspNetCoreHttpAbstractionsPackageName, outputTarget.GetMaxNetAppVersion());

        public static NugetPackageInfo MudBlazor(IOutputTarget outputTarget) => NugetRegistry.GetVersion(MudBlazorPackageName, outputTarget.GetMaxNetAppVersion());
    }
}