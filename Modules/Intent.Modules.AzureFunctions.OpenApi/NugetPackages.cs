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
        public const string MicrosoftAzureFunctionsWorkerExtensionsOpenApiPackageName = "Microsoft.Azure.Functions.Worker.Extensions.OpenApi";
        public const string MicrosoftAzureWebJobsExtensionsOpenApiPackageName = "Microsoft.Azure.WebJobs.Extensions.OpenApi";

        public void RegisterPackages()
        {
            NugetRegistry.Register(MicrosoftAzureFunctionsWorkerExtensionsOpenApiPackageName,
                (framework) => (framework.Major, framework.Minor) switch
                    {
                        ( >= 7, >= 0) => new PackageVersion("1.6.0")
                            .WithNugetDependency("Microsoft.Azure.Core.NewtonsoftJson", "1.0.0")
                            .WithNugetDependency("Microsoft.Azure.Functions.Worker.Core", "1.8.0")
                            .WithNugetDependency("Microsoft.Azure.Functions.Worker.Extensions.Http", "3.0.13")
                            .WithNugetDependency("Microsoft.Azure.WebJobs.Extensions.OpenApi.Core", "1.6.0")
                            .WithNugetDependency("YamlDotNet", "12.0.1"),
                        ( >= 6, >= 0) => new PackageVersion("1.6.0")
                            .WithNugetDependency("Microsoft.Azure.Core.NewtonsoftJson", "1.0.0")
                            .WithNugetDependency("Microsoft.Azure.Functions.Worker.Core", "1.8.0")
                            .WithNugetDependency("Microsoft.Azure.Functions.Worker.Extensions.Http", "3.0.13")
                            .WithNugetDependency("Microsoft.Azure.WebJobs.Extensions.OpenApi.Core", "1.6.0")
                            .WithNugetDependency("YamlDotNet", "12.0.1"),
                        ( >= 2, >= 0) => new PackageVersion("1.6.0")
                            .WithNugetDependency("Microsoft.Azure.Core.NewtonsoftJson", "1.0.0")
                            .WithNugetDependency("Microsoft.Azure.Functions.Worker.Core", "1.8.0")
                            .WithNugetDependency("Microsoft.Azure.Functions.Worker.Extensions.Http", "3.0.13")
                            .WithNugetDependency("Microsoft.Azure.WebJobs.Extensions.OpenApi.Core", "1.6.0")
                            .WithNugetDependency("YamlDotNet", "12.0.1"),
                        _ => throw new Exception($"Unsupported Framework `{framework.Major}` for NuGet package '{MicrosoftAzureFunctionsWorkerExtensionsOpenApiPackageName}'"),
                    }
                );
            NugetRegistry.Register(MicrosoftAzureWebJobsExtensionsOpenApiPackageName,
                (framework) => (framework.Major, framework.Minor) switch
                    {
                        ( >= 2, >= 0) => new PackageVersion("1.6.0")
                            .WithNugetDependency("Microsoft.Azure.WebJobs.Extensions.OpenApi.Core", "1.6.0")
                            .WithNugetDependency("Microsoft.Azure.WebJobs.Script.Abstractions", "1.0.0-preview")
                            .WithNugetDependency("YamlDotNet", "12.0.1"),
                        _ => throw new Exception($"Unsupported Framework `{framework.Major}` for NuGet package '{MicrosoftAzureWebJobsExtensionsOpenApiPackageName}'"),
                    }
                );
        }

        public static NugetPackageInfo MicrosoftAzureFunctionsWorkerExtensionsOpenApi(IOutputTarget outputTarget) => NugetRegistry.GetVersion(MicrosoftAzureFunctionsWorkerExtensionsOpenApiPackageName, outputTarget.GetMaxNetAppVersion());

        public static NugetPackageInfo MicrosoftAzureWebJobsExtensionsOpenApi(IOutputTarget outputTarget) => NugetRegistry.GetVersion(MicrosoftAzureWebJobsExtensionsOpenApiPackageName, outputTarget.GetMaxNetAppVersion());
    }
}
