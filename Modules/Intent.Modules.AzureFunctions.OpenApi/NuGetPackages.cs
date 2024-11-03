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
        public const string MicrosoftCodeAnalysisCSharpWorkspacesPackageName = "Microsoft.CodeAnalysis.CSharp.Workspaces";
        public const string MicrosoftCodeAnalysisWorkspacesCommonPackageName = "Microsoft.CodeAnalysis.Workspaces.Common";

        public void RegisterPackages()
        {
            NugetRegistry.Register(MicrosoftAzureFunctionsWorkerExtensionsOpenApiPackageName,
                (framework) => framework switch
                    {
                        ( >= 2, 0) => new PackageVersion("1.5.1")
                            .WithNugetDependency("Microsoft.Azure.Core.NewtonsoftJson", "1.0.0")
                            .WithNugetDependency("Microsoft.Azure.Functions.Worker.Core", "1.8.0")
                            .WithNugetDependency("Microsoft.Azure.Functions.Worker.Extensions.Http", "3.0.13")
                            .WithNugetDependency("Microsoft.Azure.WebJobs.Extensions.OpenApi.Core", "1.5.1")
                            .WithNugetDependency("YamlDotNet", "12.0.1"),
                        _ => throw new Exception($"Unsupported Framework `{framework.Major}` for NuGet package '{MicrosoftAzureFunctionsWorkerExtensionsOpenApiPackageName}'"),
                    }
                );
            NugetRegistry.Register(MicrosoftAzureWebJobsExtensionsOpenApiPackageName,
                (framework) => framework switch
                    {
                        ( >= 2, 0) => new PackageVersion("1.5.1"),
                        _ => throw new Exception($"Unsupported Framework `{framework.Major}` for NuGet package '{MicrosoftAzureWebJobsExtensionsOpenApiPackageName}'"),
                    }
                );
            NugetRegistry.Register(MicrosoftCodeAnalysisCSharpWorkspacesPackageName,
                (framework) => framework switch
                    {
                        ( >= 2, 0) => new PackageVersion("4.11.0"),
                        _ => throw new Exception($"Unsupported Framework `{framework.Major}` for NuGet package '{MicrosoftCodeAnalysisCSharpWorkspacesPackageName}'"),
                    }
                );
            NugetRegistry.Register(MicrosoftCodeAnalysisWorkspacesCommonPackageName,
                (framework) => framework switch
                    {
                        ( >= 2, 0) => new PackageVersion("4.11.0"),
                        _ => throw new Exception($"Unsupported Framework `{framework.Major}` for NuGet package '{MicrosoftCodeAnalysisWorkspacesCommonPackageName}'"),
                    }
                );
        }

        public static NugetPackageInfo MicrosoftAzureFunctionsWorkerExtensionsOpenApi(IOutputTarget outputTarget) => NugetRegistry.GetVersion(MicrosoftAzureFunctionsWorkerExtensionsOpenApiPackageName, outputTarget.GetMaxNetAppVersion());

        public static NugetPackageInfo MicrosoftAzureWebJobsExtensionsOpenApi(IOutputTarget outputTarget) => NugetRegistry.GetVersion(MicrosoftAzureWebJobsExtensionsOpenApiPackageName, outputTarget.GetMaxNetAppVersion());

        public static NugetPackageInfo MicrosoftCodeAnalysisCSharpWorkspaces(IOutputTarget outputTarget) => NugetRegistry.GetVersion(MicrosoftCodeAnalysisCSharpWorkspacesPackageName, outputTarget.GetMaxNetAppVersion());

        public static NugetPackageInfo MicrosoftCodeAnalysisWorkspacesCommon(IOutputTarget outputTarget) => NugetRegistry.GetVersion(MicrosoftCodeAnalysisWorkspacesCommonPackageName, outputTarget.GetMaxNetAppVersion());
    }
}
