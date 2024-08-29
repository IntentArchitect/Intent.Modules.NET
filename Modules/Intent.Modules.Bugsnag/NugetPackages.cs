using System;
using Intent.Engine;
using Intent.Modules.Common.CSharp.Nuget;
using Intent.Modules.Common.VisualStudio;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.NugetPackages", Version = "1.0")]

namespace Intent.Modules.Bugsnag
{
    public class NugetPackages : INugetPackages
    {
        public const string BugsnagAspNetCorePackageName = "Bugsnag.AspNet.Core";

        public void RegisterPackages()
        {
            NugetRegistry.Register(BugsnagAspNetCorePackageName,
                (framework) => framework switch
                    {
                        ( >= 2, 0) => new PackageVersion("3.1.0"),
                        _ => throw new Exception($"Unsupported Framework `{framework.Major}` for NuGet package '{BugsnagAspNetCorePackageName}'"),
                    }
                );
        }

        public static NugetPackageInfo BugsnagAspNetCore(IOutputTarget outputTarget) => NugetRegistry.GetVersion(BugsnagAspNetCorePackageName, outputTarget.GetMaxNetAppVersion());
    }
}
