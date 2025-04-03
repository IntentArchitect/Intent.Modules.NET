using System;
using Intent.Engine;
using Intent.Modules.Common.CSharp.Nuget;
using Intent.Modules.Common.VisualStudio;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.NugetPackages", Version = "1.0")]

namespace Intent.Modules.Eventing.AzureServiceBus
{
    public class NugetPackages : INugetPackages
    {
        public const string AzureMessagingServiceBusPackageName = "Azure.Messaging.ServiceBus";

        public void RegisterPackages()
        {
            NugetRegistry.Register(AzureMessagingServiceBusPackageName,
                (framework) => framework switch
                    {
                        ( >= 8, 0) => new PackageVersion("7.18.4"),
                        ( >= 2, 0) => new PackageVersion("7.18.4"),
                        _ => throw new Exception($"Unsupported Framework `{framework.Major}` for NuGet package '{AzureMessagingServiceBusPackageName}'"),
                    }
                );
        }

        public static NugetPackageInfo AzureMessagingServiceBus(IOutputTarget outputTarget) => NugetRegistry.GetVersion(AzureMessagingServiceBusPackageName, outputTarget.GetMaxNetAppVersion());
    }
}