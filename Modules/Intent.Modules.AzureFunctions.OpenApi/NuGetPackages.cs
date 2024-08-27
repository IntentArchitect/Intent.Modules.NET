using System;
using Intent.Engine;
using Intent.Modules.Common.CSharp.Nuget;
using Intent.Modules.Common.VisualStudio;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.NugetPackages", Version = "1.0")]

namespace Intent.Modules.AzureFunctions.OpenApi
{
    public class NugetPackages : INugetPackages
    {
        public const string MicrosoftAzureWebJobsExtensionsOpenApiPackageName = "Microsoft.Azure.WebJobs.Extensions.OpenApi";

        public void RegisterPackages()
        {
            NugetRegistry.Register(MicrosoftAzureWebJobsExtensionsOpenApiPackageName,
                (framework) => framework switch
                    {
                        ( >= 2, 0) => new PackageVersion("1.5.1"),
                        _ => throw new Exception($"Unsupported Framework `{framework.Major}` for NuGet package '{MicrosoftAzureWebJobsExtensionsOpenApiPackageName}'"),
                    }
                );
        }

        public static NugetPackageInfo MicrosoftAzureWebJobsExtensionsOpenApi(IOutputTarget outputTarget) => NugetRegistry.GetVersion(MicrosoftAzureWebJobsExtensionsOpenApiPackageName, outputTarget.GetMaxNetAppVersion());
    }
}
