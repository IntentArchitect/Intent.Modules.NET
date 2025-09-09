using System;
using Intent.Engine;
using Intent.Modules.Common.CSharp.Nuget;
using Intent.Modules.Common.VisualStudio;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.NugetPackages", Version = "1.0")]

namespace Intent.Modules.AspNetCore.Scalar
{
    public class NugetPackages : INugetPackages
    {
        public const string MicrosoftAspNetCoreOpenApiPackageName = "Microsoft.AspNetCore.OpenApi";
        public const string ScalarAspNetCorePackageName = "Scalar.AspNetCore";

        public void RegisterPackages()
        {
            NugetRegistry.Register(MicrosoftAspNetCoreOpenApiPackageName,
                (framework) => (framework.Major, framework.Minor) switch
                    {
                        ( >= 9, >= 0) => new PackageVersion("9.0.8")
                            .WithNugetDependency("Microsoft.OpenApi", "1.6.17"),
                        ( >= 8, >= 0) => new PackageVersion("8.0.19")
                            .WithNugetDependency("Microsoft.OpenApi", "1.4.3"),
                        ( >= 7, >= 0) => new PackageVersion("7.0.20")
                            .WithNugetDependency("Microsoft.OpenApi", "1.4.3"),
                        _ => throw new Exception($"Unsupported Framework `{framework.Major}` for NuGet package '{MicrosoftAspNetCoreOpenApiPackageName}'"),
                    }
                );
            NugetRegistry.Register(ScalarAspNetCorePackageName,
                (framework) => (framework.Major, framework.Minor) switch
                    {
                        ( >= 9, >= 0) => new PackageVersion("2.7.2"),
                        ( >= 8, >= 0) => new PackageVersion("2.7.2"),
                        _ => throw new Exception($"Unsupported Framework `{framework.Major}` for NuGet package '{ScalarAspNetCorePackageName}'"),
                    }
                );
        }

        public static NugetPackageInfo MicrosoftAspNetCoreOpenApi(IOutputTarget outputTarget) => NugetRegistry.GetVersion(MicrosoftAspNetCoreOpenApiPackageName, outputTarget.GetMaxNetAppVersion());

        public static NugetPackageInfo ScalarAspNetCore(IOutputTarget outputTarget) => NugetRegistry.GetVersion(ScalarAspNetCorePackageName, outputTarget.GetMaxNetAppVersion());
    }
}