using System;
using Intent.Engine;
using Intent.Modules.Common.CSharp.Nuget;
using Intent.Modules.Common.VisualStudio;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.NugetPackages", Version = "1.0")]

namespace Intent.Modules.AmazonS3.ObjectStorage
{
    public class NugetPackages : INugetPackages
    {
        public const string AWSSDKExtensionsNETCoreSetupPackageName = "AWSSDK.Extensions.NETCore.Setup";
        public const string AWSSDKS3PackageName = "AWSSDK.S3";

        public void RegisterPackages()
        {
            NugetRegistry.Register(AWSSDKExtensionsNETCoreSetupPackageName,
                (framework) => framework switch
                    {
                        ( >= 2, 0) => new PackageVersion("3.7.7"),
                        ( >= 0, 0) => new PackageVersion("3.7.301"),
                        _ => throw new Exception($"Unsupported Framework `{framework.Major}` for NuGet package '{AWSSDKExtensionsNETCoreSetupPackageName}'"),
                    }
                );
            NugetRegistry.Register(AWSSDKS3PackageName,
                (framework) => framework switch
                    {
                        ( >= 0, 0) => new PackageVersion("3.7.400.2"),
                        _ => throw new Exception($"Unsupported Framework `{framework.Major}` for NuGet package '{AWSSDKS3PackageName}'"),
                    }
                );
        }

        public static NugetPackageInfo AWSSDKS3(IOutputTarget outputTarget) => NugetRegistry.GetVersion(AWSSDKS3PackageName, outputTarget.GetMaxNetAppVersion());

        public static NugetPackageInfo AWSSDKExtensionsNETCoreSetup(IOutputTarget outputTarget) => NugetRegistry.GetVersion(AWSSDKExtensionsNETCoreSetupPackageName, outputTarget.GetMaxNetAppVersion());
    }
}
