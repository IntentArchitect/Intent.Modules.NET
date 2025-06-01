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
                (framework) => (framework.Major, framework.Minor) switch
                    {
                        ( >= 2, >= 0) => new PackageVersion("4.13.0")
                            .WithNugetDependency("Google.Api.Gax.Rest", "4.9.0")
                            .WithNugetDependency("Google.Apis.Storage.v1", "1.69.0.3707"),
                        _ => throw new Exception($"Unsupported Framework `{framework.Major}` for NuGet package '{GoogleCloudStorageV1PackageName}'"),
                    }
                );
        }

        public static NugetPackageInfo GoogleCloudStorageV1(IOutputTarget outputTarget) => NugetRegistry.GetVersion(GoogleCloudStorageV1PackageName, outputTarget.GetMaxNetAppVersion());
    }
}