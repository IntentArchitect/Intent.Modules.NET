using System;
using Intent.Engine;
using Intent.Modules.Common.VisualStudio;

namespace Intent.Modules.AmazonS3.ObjectStorage
{
    public static class NugetPackages
    {

        public static NugetPackageInfo AWSSDKS3(IOutputTarget outputTarget) => new NugetPackageInfo(
            name: "AWSSDK.S3",
            version: outputTarget.GetMaxNetAppVersion() switch
            {
                (>= 0, 0) => "3.7.400.2",
                _ => throw new Exception($"Unsupported Framework `{outputTarget.GetMaxNetAppVersion().Major}` for NuGet package 'AWSSDK.S3'")
            });

        public static NugetPackageInfo AWSSDKExtensionsNETCoreSetup(IOutputTarget outputTarget) => new NugetPackageInfo(
            name: "AWSSDK.Extensions.NETCore.Setup",
            version: outputTarget.GetMaxNetAppVersion() switch
            {
                (>= 2, 0) => "3.7.7",
                (>= 0, 0) => "3.7.301",
                _ => throw new Exception($"Unsupported Framework `{outputTarget.GetMaxNetAppVersion().Major}` for NuGet package 'AWSSDK.Extensions.NETCore.Setup'")
            });
    }
}
