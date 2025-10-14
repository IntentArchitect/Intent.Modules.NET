using System;
using Intent.Engine;
using Intent.Modules.Common.CSharp.Nuget;
using Intent.Modules.Common.VisualStudio;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.NugetPackages", Version = "1.0")]

namespace Intent.Modules.AzureFunctions.AzureServiceBus
{
    public class NugetPackages : INugetPackages
    {
        public const string MicrosoftAzureFunctionsWorkerExtensionsServiceBusPackageName = "Microsoft.Azure.Functions.Worker.Extensions.ServiceBus";

        public void RegisterPackages()
        {
            NugetRegistry.Register(MicrosoftAzureFunctionsWorkerExtensionsServiceBusPackageName,
                (framework) => (framework.Major, framework.Minor) switch
                    {
                        ( >= 2, >= 0) => new PackageVersion("5.24.0")
                            .WithNugetDependency("Azure.Identity", "1.16.0")
                            .WithNugetDependency("Azure.Messaging.ServiceBus", "7.20.1")
                            .WithNugetDependency("Google.Protobuf", "3.32.1")
                            .WithNugetDependency("Microsoft.Azure.Functions.Worker.Extensions.Abstractions", "1.3.0")
                            .WithNugetDependency("Microsoft.Azure.Functions.Worker.Extensions.Rpc", "1.0.1")
                            .WithNugetDependency("Microsoft.Extensions.Azure", "1.13.0"),
                        _ => throw new Exception($"Unsupported Framework `{framework.Major}` for NuGet package '{MicrosoftAzureFunctionsWorkerExtensionsServiceBusPackageName}'"),
                    }
                );
        }

        public static NugetPackageInfo MicrosoftAzureFunctionsWorkerExtensionsServiceBus(IOutputTarget outputTarget) => NugetRegistry.GetVersion(MicrosoftAzureFunctionsWorkerExtensionsServiceBusPackageName, outputTarget.GetMaxNetAppVersion());
    }
}