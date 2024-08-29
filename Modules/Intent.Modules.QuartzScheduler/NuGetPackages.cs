using System;
using Intent.Engine;
using Intent.Modules.Common.CSharp.Nuget;
using Intent.Modules.Common.VisualStudio;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.NugetPackages", Version = "1.0")]

namespace Intent.Modules.QuartzScheduler
{
    public class NugetPackages : INugetPackages
    {
        public const string QuartzAspNetCorePackageName = "Quartz.AspNetCore";
        public const string QuartzExtensionsHostingPackageName = "Quartz.Extensions.Hosting";

        public void RegisterPackages()
        {
            NugetRegistry.Register(QuartzAspNetCorePackageName,
                (framework) => framework switch
                    {
                        ( >= 8, 0) => new PackageVersion("3.12.0"),
                        ( >= 6, 0) => new PackageVersion("3.12.0"),
                        _ => throw new Exception($"Unsupported Framework `{framework.Major}` for NuGet package '{QuartzAspNetCorePackageName}'"),
                    }
                );
            NugetRegistry.Register(QuartzExtensionsHostingPackageName,
                (framework) => framework switch
                    {
                        ( >= 8, 0) => new PackageVersion("3.12.0"),
                        ( >= 6, 0) => new PackageVersion("3.12.0"),
                        _ => throw new Exception($"Unsupported Framework `{framework.Major}` for NuGet package '{QuartzExtensionsHostingPackageName}'"),
                    }
                );
        }

        public static NugetPackageInfo QuartzExtensionsHosting(IOutputTarget outputTarget) => NugetRegistry.GetVersion(QuartzExtensionsHostingPackageName, outputTarget.GetMaxNetAppVersion());

        public static NugetPackageInfo QuartzAspNetCore(IOutputTarget outputTarget) => NugetRegistry.GetVersion(QuartzAspNetCorePackageName, outputTarget.GetMaxNetAppVersion());
    }
}
