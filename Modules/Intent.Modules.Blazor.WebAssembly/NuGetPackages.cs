using System;
using Intent.Engine;
using Intent.Modules.Common.CSharp.Nuget;
using Intent.Modules.Common.VisualStudio;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.NugetPackages", Version = "1.0")]

namespace Intent.Modules.Blazor.WebAssembly
{
    public class NugetPackages : INugetPackages
    {
        public const string MicrosoftAspNetCoreComponentsWebAssemblyPackageName = "Microsoft.AspNetCore.Components.WebAssembly";
        public const string MicrosoftAspNetCoreComponentsWebAssemblyDevServerPackageName = "Microsoft.AspNetCore.Components.WebAssembly.DevServer";

        public void RegisterPackages()
        {
            NugetRegistry.Register(MicrosoftAspNetCoreComponentsWebAssemblyPackageName,
                (framework) => framework switch
                    {
                        ( >= 8, 0) => new PackageVersion("8.0.7"),
                        ( >= 7, 0) => new PackageVersion("7.0.20"),
                        ( >= 6, 0) => new PackageVersion("6.0.32"),
                        _ => throw new Exception($"Unsupported Framework `{framework.Major}` for NuGet package '{MicrosoftAspNetCoreComponentsWebAssemblyPackageName}'"),
                    }
                );
            NugetRegistry.Register(MicrosoftAspNetCoreComponentsWebAssemblyDevServerPackageName,
                (framework) => framework switch
                    {
                        ( >= 8, 0) => new PackageVersion("8.0.0", locked: true),
                        ( >= 7, 0) => new PackageVersion("7.0.14", locked: true),
                        _ => throw new Exception($"Unsupported Framework `{framework.Major}` for NuGet package '{MicrosoftAspNetCoreComponentsWebAssemblyDevServerPackageName}'"),
                    }
                );
        }

        public static NugetPackageInfo MicrosoftAspNetCoreComponentsWebAssembly(IOutputTarget outputTarget) => NugetRegistry.GetVersion(MicrosoftAspNetCoreComponentsWebAssemblyPackageName, outputTarget.GetMaxNetAppVersion());
        public static NugetPackageInfo MicrosoftAspNetCoreComponentsWebAssemblyDevServer(IOutputTarget outputTarget) => NugetRegistry.GetVersion(MicrosoftAspNetCoreComponentsWebAssemblyDevServerPackageName, outputTarget.GetMaxNetAppVersion());
    }
}
