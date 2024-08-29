using System;
using Intent.Engine;
using Intent.Modules.Common.CSharp.Nuget;
using Intent.Modules.Common.VisualStudio;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.NugetPackages", Version = "1.0")]

namespace Intent.Modules.HotChocolate.GraphQL.AspNetCore
{
    public class NugetPackages : INugetPackages
    {
        public const string HotChocolateAspNetCorePackageName = "HotChocolate.AspNetCore";
        public const string HotChocolateAspNetCoreAuthorizationPackageName = "HotChocolate.AspNetCore.Authorization";

        public void RegisterPackages()
        {
            NugetRegistry.Register(HotChocolateAspNetCorePackageName,
                (framework) => framework switch
                    {
                        ( >= 8, 0) => new PackageVersion("13.9.9"),
                        ( >= 7, 0) => new PackageVersion("13.9.9"),
                        ( >= 6, 0) => new PackageVersion("13.9.9"),
                        _ => throw new Exception($"Unsupported Framework `{framework.Major}` for NuGet package '{HotChocolateAspNetCorePackageName}'"),
                    }
                );
            NugetRegistry.Register(HotChocolateAspNetCoreAuthorizationPackageName,
                (framework) => framework switch
                    {
                        ( >= 8, 0) => new PackageVersion("13.9.9"),
                        ( >= 7, 0) => new PackageVersion("13.9.9"),
                        ( >= 6, 0) => new PackageVersion("13.9.9"),
                        _ => throw new Exception($"Unsupported Framework `{framework.Major}` for NuGet package '{HotChocolateAspNetCoreAuthorizationPackageName}'"),
                    }
                );
        }

        public static NugetPackageInfo HotChocolateAspNetCore(IOutputTarget outputTarget) => NugetRegistry.GetVersion(HotChocolateAspNetCorePackageName, outputTarget.GetMaxNetAppVersion());

        public static NugetPackageInfo HotChocolateAspNetCoreAuthorization(IOutputTarget outputTarget) => NugetRegistry.GetVersion(HotChocolateAspNetCoreAuthorizationPackageName, outputTarget.GetMaxNetAppVersion());
    }
}
