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
        public const string MicrosoftCodeAnalysisCommonPackageName = "Microsoft.CodeAnalysis.Common";
        public const string MicrosoftCodeAnalysisCSharpWorkspacesPackageName = "Microsoft.CodeAnalysis.CSharp.Workspaces";
        public const string MicrosoftCodeAnalysisWorkspacesCommonPackageName = "Microsoft.CodeAnalysis.Workspaces.Common";

        public void RegisterPackages()
        {
            NugetRegistry.Register(MicrosoftAzureFunctionsWorkerExtensionsOpenApiPackageName,
                (framework) => framework switch
                    {
                        ( >= 7, 0) => new PackageVersion("1.5.1")
                            .WithNugetDependency("Microsoft.Azure.Core.NewtonsoftJson", "1.0.0")
                            .WithNugetDependency("Microsoft.Azure.Functions.Worker.Core", "1.8.0")
                            .WithNugetDependency("Microsoft.Azure.Functions.Worker.Extensions.Http", "3.0.13")
                            .WithNugetDependency("Microsoft.Azure.WebJobs.Extensions.OpenApi.Core", "1.5.1")
                            .WithNugetDependency("YamlDotNet", "12.0.1"),
                        ( >= 6, 0) => new PackageVersion("1.5.1")
                            .WithNugetDependency("Microsoft.Azure.Core.NewtonsoftJson", "1.0.0")
                            .WithNugetDependency("Microsoft.Azure.Functions.Worker.Core", "1.8.0")
                            .WithNugetDependency("Microsoft.Azure.Functions.Worker.Extensions.Http", "3.0.13")
                            .WithNugetDependency("Microsoft.Azure.WebJobs.Extensions.OpenApi.Core", "1.5.1")
                            .WithNugetDependency("YamlDotNet", "12.0.1"),
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
            NugetRegistry.Register(MicrosoftCodeAnalysisCommonPackageName,
                (framework) => framework switch
                    {
                        ( >= 8, 0) => new PackageVersion("4.11.0")
                            .WithNugetDependency("Microsoft.CodeAnalysis.Analyzers", "3.3.4")
                            .WithNugetDependency("System.Collections.Immutable", "8.0.0")
                            .WithNugetDependency("System.Reflection.Metadata", "8.0.0"),
                        ( >= 7, 0) => new PackageVersion("4.11.0")
                            .WithNugetDependency("Microsoft.CodeAnalysis.Analyzers", "3.3.4")
                            .WithNugetDependency("System.Collections.Immutable", "8.0.0")
                            .WithNugetDependency("System.Reflection.Metadata", "8.0.0"),
                        ( >= 2, 0) => new PackageVersion("4.11.0")
                            .WithNugetDependency("Microsoft.CodeAnalysis.Analyzers", "3.3.4")
                            .WithNugetDependency("System.Buffers", "4.5.1")
                            .WithNugetDependency("System.Collections.Immutable", "8.0.0")
                            .WithNugetDependency("System.Memory", "4.5.5")
                            .WithNugetDependency("System.Numerics.Vectors", "4.5.0")
                            .WithNugetDependency("System.Reflection.Metadata", "8.0.0")
                            .WithNugetDependency("System.Runtime.CompilerServices.Unsafe", "6.0.0")
                            .WithNugetDependency("System.Text.Encoding.CodePages", "7.0.0")
                            .WithNugetDependency("System.Threading.Tasks.Extensions", "4.5.4"),
                        _ => throw new Exception($"Unsupported Framework `{framework.Major}` for NuGet package '{MicrosoftCodeAnalysisCommonPackageName}'"),
                    }
                );
            NugetRegistry.Register(MicrosoftCodeAnalysisCSharpWorkspacesPackageName,
                (framework) => framework switch
                    {
                        ( >= 8, 0) => new PackageVersion("4.11.0")
                            .WithNugetDependency("Humanizer.Core", "2.14.1")
                            .WithNugetDependency("Microsoft.CodeAnalysis.Analyzers", "3.3.4")
                            .WithNugetDependency("Microsoft.CodeAnalysis.Common", "4.11.0")
                            .WithNugetDependency("Microsoft.CodeAnalysis.CSharp", "4.11.0")
                            .WithNugetDependency("Microsoft.CodeAnalysis.Workspaces.Common", "4.11.0")
                            .WithNugetDependency("System.Collections.Immutable", "8.0.0")
                            .WithNugetDependency("System.Composition", "8.0.0")
                            .WithNugetDependency("System.IO.Pipelines", "8.0.0")
                            .WithNugetDependency("System.Reflection.Metadata", "8.0.0")
                            .WithNugetDependency("System.Threading.Channels", "7.0.0"),
                        ( >= 7, 0) => new PackageVersion("4.11.0")
                            .WithNugetDependency("Humanizer.Core", "2.14.1")
                            .WithNugetDependency("Microsoft.CodeAnalysis.Analyzers", "3.3.4")
                            .WithNugetDependency("Microsoft.CodeAnalysis.Common", "4.11.0")
                            .WithNugetDependency("Microsoft.CodeAnalysis.CSharp", "4.11.0")
                            .WithNugetDependency("Microsoft.CodeAnalysis.Workspaces.Common", "4.11.0")
                            .WithNugetDependency("System.Collections.Immutable", "8.0.0")
                            .WithNugetDependency("System.Composition", "8.0.0")
                            .WithNugetDependency("System.IO.Pipelines", "8.0.0")
                            .WithNugetDependency("System.Reflection.Metadata", "8.0.0")
                            .WithNugetDependency("System.Threading.Channels", "7.0.0"),
                        ( >= 2, 0) => new PackageVersion("4.11.0"),
                        _ => throw new Exception($"Unsupported Framework `{framework.Major}` for NuGet package '{MicrosoftCodeAnalysisCSharpWorkspacesPackageName}'"),
                    }
                );
            NugetRegistry.Register(MicrosoftCodeAnalysisWorkspacesCommonPackageName,
                (framework) => framework switch
                    {
                        ( >= 8, 0) => new PackageVersion("4.11.0")
                            .WithNugetDependency("Humanizer.Core", "2.14.1")
                            .WithNugetDependency("Microsoft.CodeAnalysis.Analyzers", "3.3.4")
                            .WithNugetDependency("Microsoft.CodeAnalysis.Common", "4.11.0")
                            .WithNugetDependency("System.Collections.Immutable", "8.0.0")
                            .WithNugetDependency("System.Composition", "8.0.0")
                            .WithNugetDependency("System.IO.Pipelines", "8.0.0")
                            .WithNugetDependency("System.Reflection.Metadata", "8.0.0")
                            .WithNugetDependency("System.Threading.Channels", "7.0.0"),
                        ( >= 7, 0) => new PackageVersion("4.11.0")
                            .WithNugetDependency("Humanizer.Core", "2.14.1")
                            .WithNugetDependency("Microsoft.CodeAnalysis.Analyzers", "3.3.4")
                            .WithNugetDependency("Microsoft.CodeAnalysis.Common", "4.11.0")
                            .WithNugetDependency("System.Collections.Immutable", "8.0.0")
                            .WithNugetDependency("System.Composition", "8.0.0")
                            .WithNugetDependency("System.IO.Pipelines", "8.0.0")
                            .WithNugetDependency("System.Reflection.Metadata", "8.0.0")
                            .WithNugetDependency("System.Threading.Channels", "7.0.0"),
                        ( >= 2, 0) => new PackageVersion("4.11.0"),
                        _ => throw new Exception($"Unsupported Framework `{framework.Major}` for NuGet package '{MicrosoftCodeAnalysisWorkspacesCommonPackageName}'"),
                    }
                );
        }

        public static NugetPackageInfo MicrosoftAzureFunctionsWorkerExtensionsOpenApi(IOutputTarget outputTarget) => NugetRegistry.GetVersion(MicrosoftAzureFunctionsWorkerExtensionsOpenApiPackageName, outputTarget.GetMaxNetAppVersion());

        public static NugetPackageInfo MicrosoftAzureWebJobsExtensionsOpenApi(IOutputTarget outputTarget) => NugetRegistry.GetVersion(MicrosoftAzureWebJobsExtensionsOpenApiPackageName, outputTarget.GetMaxNetAppVersion());

        public static NugetPackageInfo MicrosoftCodeAnalysisCommon(IOutputTarget outputTarget) => NugetRegistry.GetVersion(MicrosoftCodeAnalysisCommonPackageName, outputTarget.GetMaxNetAppVersion());

        public static NugetPackageInfo MicrosoftCodeAnalysisCSharpWorkspaces(IOutputTarget outputTarget) => NugetRegistry.GetVersion(MicrosoftCodeAnalysisCSharpWorkspacesPackageName, outputTarget.GetMaxNetAppVersion());

        public static NugetPackageInfo MicrosoftCodeAnalysisWorkspacesCommon(IOutputTarget outputTarget) => NugetRegistry.GetVersion(MicrosoftCodeAnalysisWorkspacesCommonPackageName, outputTarget.GetMaxNetAppVersion());
    }
}
