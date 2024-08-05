using Intent.Engine;
using Intent.Modules.Common.VisualStudio;

namespace Intent.Modules.Azure.BlobStorage
{
    public static class NugetPackages
    {

        public static NugetPackageInfo AzureStorageBlobs(IOutputTarget outputTarget) => new(
            name: "Azure.Storage.Blobs",
            version: outputTarget.GetMaxNetAppVersion() switch
            {
                _ => "12.21.1",
            });
    }
}
