using System;
using Intent.Engine;
using Intent.Modules.Common.CSharp.Nuget;
using Intent.Modules.Common.VisualStudio;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.NugetPackages", Version = "1.0")]

namespace Intent.Modules.AspNetCore.Identity
{
    public class NugetPackages : INugetPackages
    {
        public const string MicrosoftAspNetCoreIdentityEntityFrameworkCorePackageName = "Microsoft.AspNetCore.Identity.EntityFrameworkCore";
        public const string MicrosoftExtensionsIdentityStoresPackageName = "Microsoft.Extensions.Identity.Stores";

        public void RegisterPackages()
        {
            NugetRegistry.Register(MicrosoftAspNetCoreIdentityEntityFrameworkCorePackageName,
                (framework) => framework switch
                    {
                        ( >= 8, 0) => new PackageVersion("8.0.7"),
                        ( >= 7, 0) => new PackageVersion("7.0.20"),
                        ( >= 6, 0) => new PackageVersion("6.0.32"),
                        _ => throw new Exception($"Unsupported Framework `{framework.Major}` for NuGet package '{MicrosoftAspNetCoreIdentityEntityFrameworkCorePackageName}'"),
                    }
                );
            NugetRegistry.Register(MicrosoftExtensionsIdentityStoresPackageName,
                (framework) => framework switch
                    {
                        ( >= 8, 0) => new PackageVersion("8.0.7"),
                        ( >= 7, 0) => new PackageVersion("7.0.20"),
                        ( >= 6, 0) => new PackageVersion("6.0.32"),
                        _ => throw new Exception($"Unsupported Framework `{framework.Major}` for NuGet package '{MicrosoftExtensionsIdentityStoresPackageName}'"),
                    }
                );
        }

        public static NugetPackageInfo MicrosoftAspNetCoreIdentityEntityFrameworkCore(IOutputTarget outputTarget) => NugetRegistry.GetVersion(MicrosoftAspNetCoreIdentityEntityFrameworkCorePackageName, outputTarget.GetMaxNetAppVersion());

        public static NugetPackageInfo MicrosoftExtensionsIdentityStores(IOutputTarget outputTarget) => NugetRegistry.GetVersion(MicrosoftExtensionsIdentityStoresPackageName, outputTarget.GetMaxNetAppVersion());
    }
}
