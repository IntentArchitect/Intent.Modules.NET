using System;
using Intent.Engine;
using Intent.Modules.Common.CSharp.Nuget;
using Intent.Modules.Common.VisualStudio;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.NugetPackages", Version = "1.0")]

namespace Intent.Modules.Azure.BlobStorage
{
    public class NugetPackages : INugetPackages
    {
        public const string AzureStorageBlobsPackageName = "Azure.Storage.Blobs";

        public void RegisterPackages()
        {
            NugetRegistry.Register(AzureStorageBlobsPackageName,
                (framework) => framework switch
                    {
                        ( >= 6, 0) => new PackageVersion("12.21.1"),
                        _ => throw new Exception($"Unsupported Framework `{framework.Major}` for NuGet package '{AzureStorageBlobsPackageName}'"),
                    }
                );
        }

        public static NugetPackageInfo AzureStorageBlobs(IOutputTarget outputTarget) => NugetRegistry.GetVersion(AzureStorageBlobsPackageName, outputTarget.GetMaxNetAppVersion());
    }
}
