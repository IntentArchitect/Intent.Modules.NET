using System;
using Intent.Engine;
using Intent.Modules.Common.CSharp.Nuget;
using Intent.Modules.Common.VisualStudio;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.NugetPackages", Version = "1.0")]

namespace Intent.Modules.Eventing.AzureEventGrid
{
    public class NugetPackages : INugetPackages
    {
        public const string AzureMessagingEventGridPackageName = "Azure.Messaging.EventGrid";

        public void RegisterPackages()
        {
            NugetRegistry.Register(AzureMessagingEventGridPackageName,
                (framework) => (framework.Major, framework.Minor) switch
                    {
                        ( >= 8, >= 0) => new PackageVersion("5.0.0")
                            .WithNugetDependency("Azure.Core", "1.46.2")
                            .WithNugetDependency("Azure.Messaging.EventGrid.SystemEvents", "1.0.0"),
                        ( >= 2, >= 0) => new PackageVersion("5.0.0")
                            .WithNugetDependency("Azure.Core", "1.46.2")
                            .WithNugetDependency("Azure.Messaging.EventGrid.SystemEvents", "1.0.0"),
                        _ => throw new Exception($"Unsupported Framework `{framework.Major}` for NuGet package '{AzureMessagingEventGridPackageName}'"),
                    }
                );
        }

        public static NugetPackageInfo AzureMessagingEventGrid(IOutputTarget outputTarget) => NugetRegistry.GetVersion(AzureMessagingEventGridPackageName, outputTarget.GetMaxNetAppVersion());
    }
}