using System;
using Intent.Engine;
using Intent.Modules.Common.CSharp.Nuget;
using Intent.Modules.Common.VisualStudio;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.NugetPackages", Version = "1.0")]

namespace Intent.Modules.Google.CloudStorage
{
    public class NugetPackages : INugetPackages
    {
        public const string GoogleCloudStorageV1PackageName = "Google.Cloud.Storage.V1";

        public void RegisterPackages()
        {
            NugetRegistry.Register(GoogleCloudStorageV1PackageName,
                (framework) => framework switch
                    {
                        ( >= 0, 0) => new PackageVersion("4.10.0"),
                        _ => throw new Exception($"Unsupported Framework `{framework.Major}` for NuGet package '{GoogleCloudStorageV1PackageName}'"),
                    }
                );
        }

        public static NugetPackageInfo GoogleCloudStorageV1(IOutputTarget outputTarget) => NugetRegistry.GetVersion(GoogleCloudStorageV1PackageName, outputTarget.GetMaxNetAppVersion());
    }
}