using Intent.Modules.Common.VisualStudio;

namespace Intent.Modules.AmazonS3.ObjectStorage;

public static class NuGetPackages
{
    public static readonly INugetPackageInfo AWSSDKS3 = new NugetPackageInfo("AWSSDK.S3", "3.7.307.21");
    public static readonly INugetPackageInfo AWSSDKExtensionsNETCoreSetup = new NugetPackageInfo("AWSSDK.Extensions.NETCore.Setup", "3.7.300");
}