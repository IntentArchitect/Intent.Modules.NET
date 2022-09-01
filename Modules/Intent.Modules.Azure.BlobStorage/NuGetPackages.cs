using Intent.Modules.Common.VisualStudio;

namespace Intent.Modules.Azure.BlobStorage;

public static class NuGetPackages
{
    public static readonly INugetPackageInfo AzureStorageBlobs = new NugetPackageInfo("Azure.Storage.Blobs", "12.13.0");
}