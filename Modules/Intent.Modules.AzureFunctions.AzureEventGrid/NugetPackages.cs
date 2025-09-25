using System;
using Intent.Engine;
using Intent.Modules.Common.CSharp.Nuget;
using Intent.Modules.Common.VisualStudio;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.NugetPackages", Version = "1.0")]

namespace Intent.Modules.AzureFunctions.AzureEventGrid
{
    public class NugetPackages : INugetPackages
    {
        public const string MicrosoftAzureFunctionsWorkerExtensionsEventGridPackageName = "Microsoft.Azure.Functions.Worker.Extensions.EventGrid";

        public void RegisterPackages()
        {
            NugetRegistry.Register(MicrosoftAzureFunctionsWorkerExtensionsEventGridPackageName,
                (framework) => (framework.Major, framework.Minor) switch
                    {
                        ( >= 2, >= 0) => new PackageVersion("3.6.0")
                            .WithNugetDependency("Azure.Messaging.EventGrid", "4.29.0")
                            .WithNugetDependency("Microsoft.Azure.Functions.Worker.Core", "1.20.0")
                            .WithNugetDependency("Microsoft.Azure.Functions.Worker.Extensions.Abstractions", "1.3.0"),
                        _ => throw new Exception($"Unsupported Framework `{framework.Major}` for NuGet package '{MicrosoftAzureFunctionsWorkerExtensionsEventGridPackageName}'"),
                    }
                );
        }

        public static NugetPackageInfo MicrosoftAzureFunctionsWorkerExtensionsEventGrid(IOutputTarget outputTarget) => NugetRegistry.GetVersion(MicrosoftAzureFunctionsWorkerExtensionsEventGridPackageName, outputTarget.GetMaxNetAppVersion());
    }
}