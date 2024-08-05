using System;
using Intent.Engine;
using Intent.Modules.Common.VisualStudio;

namespace Intent.Modules.Azure.BlobStorage
{
    public static class NugetPackages
    {

        public static NugetPackageInfo AzureStorageBlobs(IOutputTarget outputTarget) => new NugetPackageInfo(
            name: "Azure.Storage.Blobs",
            version: outputTarget.GetMaxNetAppVersion() switch
            {
                (>= 6, 0) => "12.21.1",
                _ => throw new Exception($"Unsupported Framework `{outputTarget.GetMaxNetAppVersion().Major}` for NuGet package 'Azure.Storage.Blobs'")
            });
    }
}
