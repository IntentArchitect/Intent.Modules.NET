using System;
using Intent.Engine;
using Intent.Modules.Common.CSharp.Nuget;
using Intent.Modules.Common.VisualStudio;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.NugetPackages", Version = "1.0")]

namespace Intent.Modules.Aws.Common
{
    public class NugetPackages : INugetPackages
    {
        public const string AWSSDKExtensionsNETCoreSetupPackageName = "AWSSDK.Extensions.NETCore.Setup";

        public void RegisterPackages()
        {
            NugetRegistry.Register(AWSSDKExtensionsNETCoreSetupPackageName,
                (framework) => (framework.Major, framework.Minor) switch
                    {
                        ( >= 8, >= 0) => new PackageVersion("4.0.2.2")
                            .WithNugetDependency("AWSSDK.Core", "4.0.0.18")
                            .WithNugetDependency("Microsoft.Extensions.Configuration.Abstractions", "2.0.0")
                            .WithNugetDependency("Microsoft.Extensions.DependencyInjection.Abstractions", "8.0.0")
                            .WithNugetDependency("Microsoft.Extensions.Logging.Abstractions", "2.0.0"),
                        ( >= 2, >= 0) => new PackageVersion("4.0.2.2")
                            .WithNugetDependency("AWSSDK.Core", "4.0.0.18")
                            .WithNugetDependency("Microsoft.Extensions.Configuration.Abstractions", "2.0.0")
                            .WithNugetDependency("Microsoft.Extensions.DependencyInjection.Abstractions", "2.0.0")
                            .WithNugetDependency("Microsoft.Extensions.Logging.Abstractions", "2.0.0"),
                        _ => throw new Exception($"Unsupported Framework `{framework.Major}` for NuGet package '{AWSSDKExtensionsNETCoreSetupPackageName}'"),
                    }
                );
        }

        public static NugetPackageInfo AWSSDKExtensionsNETCoreSetup(IOutputTarget outputTarget) => NugetRegistry.GetVersion(AWSSDKExtensionsNETCoreSetupPackageName, outputTarget.GetMaxNetAppVersion());
    }
}