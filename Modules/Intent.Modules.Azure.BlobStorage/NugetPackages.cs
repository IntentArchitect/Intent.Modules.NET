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
                (framework) => (framework.Major, framework.Minor) switch
                    {
                        ( >= 8, >= 0) => new PackageVersion("12.26.0")
                            .WithNugetDependency("Azure.Storage.Common", "12.25.0")
                            .WithNugetDependency("Azure.Core", "1.47.3"),
                        ( >= 2, >= 1) => new PackageVersion("12.26.0")
                            .WithNugetDependency("Azure.Core", "1.47.3")
                            .WithNugetDependency("Azure.Storage.Common", "12.25.0"),
                        ( >= 2, >= 0) => new PackageVersion("12.26.0")
                            .WithNugetDependency("Azure.Core", "1.47.3")
                            .WithNugetDependency("Azure.Storage.Common", "12.25.0"),
                        _ => throw new Exception($"Unsupported Framework `{framework.Major}` for NuGet package '{AzureStorageBlobsPackageName}'"),
                    }
                );
        }

        public static NugetPackageInfo AzureStorageBlobs(IOutputTarget outputTarget) => NugetRegistry.GetVersion(AzureStorageBlobsPackageName, outputTarget.GetMaxNetAppVersion());
    }
}
