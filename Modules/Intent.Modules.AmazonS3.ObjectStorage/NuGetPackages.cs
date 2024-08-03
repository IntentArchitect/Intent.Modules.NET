using Intent.Engine;
using Intent.Modules.Common.VisualStudio;

namespace Intent.Modules.AmazonS3.ObjectStorage
{
    public static class NugetPackages
    {

        public static NugetPackageInfo AWSSDKS3(IOutputTarget outputTarget) => new(
            name: "AWSSDK.S3",
            version: outputTarget.GetMaxNetAppVersion() switch
            {
                _ => "3.7.400.2",
            });

        public static NugetPackageInfo AWSSDKExtensionsNETCoreSetup(IOutputTarget outputTarget) => new(
            name: "AWSSDK.Extensions.NETCore.Setup",
            version: outputTarget.GetMaxNetAppVersion() switch
            {
                (0, 0) => "3.7.301",
                _ => "3.7.7",
            });
    }
}
