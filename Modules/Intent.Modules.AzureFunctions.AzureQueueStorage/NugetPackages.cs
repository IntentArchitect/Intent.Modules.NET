using System;
using Intent.Engine;
using Intent.Modules.Common.CSharp.Nuget;
using Intent.Modules.Common.VisualStudio;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.NugetPackages", Version = "1.0")]

namespace Intent.Modules.AzureFunctions.AzureQueueStorage
{
    public class NugetPackages : INugetPackages
    {
        public const string MicrosoftAzureFunctionsWorkerExtensionsStorageQueuesPackageName = "Microsoft.Azure.Functions.Worker.Extensions.Storage.Queues";

        public void RegisterPackages()
        {
            NugetRegistry.Register(MicrosoftAzureFunctionsWorkerExtensionsStorageQueuesPackageName,
                (framework) => (framework.Major, framework.Minor) switch
                    {
                        ( >= 2, >= 0) => new PackageVersion("5.5.3")
                            .WithNugetDependency("Azure.Storage.Queues", "12.21.0")
                            .WithNugetDependency("Microsoft.Azure.Functions.Worker.Core", "1.20.0")
                            .WithNugetDependency("Microsoft.Azure.Functions.Worker.Extensions.Abstractions", "1.3.0"),
                        _ => throw new Exception($"Unsupported Framework `{framework.Major}` for NuGet package '{MicrosoftAzureFunctionsWorkerExtensionsStorageQueuesPackageName}'"),
                    }
                );
        }

        public static NugetPackageInfo MicrosoftAzureFunctionsWorkerExtensionsStorageQueues(IOutputTarget outputTarget) => NugetRegistry.GetVersion(MicrosoftAzureFunctionsWorkerExtensionsStorageQueuesPackageName, outputTarget.GetMaxNetAppVersion());
    }
}