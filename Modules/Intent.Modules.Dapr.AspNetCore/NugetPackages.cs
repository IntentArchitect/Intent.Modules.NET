using System;
using Intent.Engine;
using Intent.Modules.Common.CSharp.Nuget;
using Intent.Modules.Common.VisualStudio;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.NugetPackages", Version = "1.0")]

namespace Intent.Modules.Dapr.AspNetCore
{
    public class NugetPackages : INugetPackages
    {
        public const string DaprAspNetCorePackageName = "Dapr.AspNetCore";
        public const string DaprClientPackageName = "Dapr.Client";
        public const string DaprExtensionsConfigurationPackageName = "Dapr.Extensions.Configuration";
        public const string ManDaprSidekickAspNetCorePackageName = "Man.Dapr.Sidekick.AspNetCore";
        public const string MediatRPackageName = "MediatR";

        public void RegisterPackages()
        {
            NugetRegistry.Register(DaprAspNetCorePackageName,
                (framework) => framework switch
                    {
                        ( >= 9, 0) => new PackageVersion("1.15.4")
                            .WithNugetDependency("Dapr.Client", "1.15.4")
                            .WithNugetDependency("Dapr.Common", "1.15.4")
                            .WithNugetDependency("Google.Api.CommonProtos", "2.2.0")
                            .WithNugetDependency("Google.Protobuf", "3.30.2")
                            .WithNugetDependency("Grpc.Net.Client", "2.71.0")
                            .WithNugetDependency("Microsoft.Extensions.Configuration", "6.0.1")
                            .WithNugetDependency("Microsoft.Extensions.Configuration.Abstractions", "6.0.0")
                            .WithNugetDependency("Microsoft.Extensions.DependencyInjection", "8.0.1")
                            .WithNugetDependency("Microsoft.Extensions.DependencyInjection.Abstractions", "8.0.2")
                            .WithNugetDependency("Microsoft.Extensions.Http", "6.0.0")
                            .WithNugetDependency("Microsoft.Extensions.Logging", "6.0.0")
                            .WithNugetDependency("Microsoft.Extensions.Logging.Abstractions", "6.0.4"),
                        ( >= 8, 0) => new PackageVersion("1.15.4")
                            .WithNugetDependency("Dapr.Client", "1.15.4")
                            .WithNugetDependency("Dapr.Common", "1.15.4")
                            .WithNugetDependency("Google.Api.CommonProtos", "2.2.0")
                            .WithNugetDependency("Google.Protobuf", "3.30.2")
                            .WithNugetDependency("Grpc.Net.Client", "2.71.0")
                            .WithNugetDependency("Microsoft.Extensions.Configuration", "6.0.1")
                            .WithNugetDependency("Microsoft.Extensions.Configuration.Abstractions", "6.0.0")
                            .WithNugetDependency("Microsoft.Extensions.DependencyInjection", "8.0.1")
                            .WithNugetDependency("Microsoft.Extensions.DependencyInjection.Abstractions", "8.0.2")
                            .WithNugetDependency("Microsoft.Extensions.Http", "6.0.0")
                            .WithNugetDependency("Microsoft.Extensions.Logging", "6.0.0")
                            .WithNugetDependency("Microsoft.Extensions.Logging.Abstractions", "6.0.4"),
                        ( >= 6, 0) => new PackageVersion("1.15.4")
                            .WithNugetDependency("Dapr.Client", "1.15.4")
                            .WithNugetDependency("Dapr.Common", "1.15.4")
                            .WithNugetDependency("Google.Api.CommonProtos", "2.2.0")
                            .WithNugetDependency("Google.Protobuf", "3.30.2")
                            .WithNugetDependency("Grpc.Net.Client", "2.71.0")
                            .WithNugetDependency("Microsoft.Extensions.Configuration", "6.0.1")
                            .WithNugetDependency("Microsoft.Extensions.Configuration.Abstractions", "6.0.0")
                            .WithNugetDependency("Microsoft.Extensions.DependencyInjection", "8.0.1")
                            .WithNugetDependency("Microsoft.Extensions.DependencyInjection.Abstractions", "8.0.2")
                            .WithNugetDependency("Microsoft.Extensions.Http", "6.0.0")
                            .WithNugetDependency("Microsoft.Extensions.Logging", "6.0.0")
                            .WithNugetDependency("Microsoft.Extensions.Logging.Abstractions", "6.0.4"),
                        _ => throw new Exception($"Unsupported Framework `{framework.Major}` for NuGet package '{DaprAspNetCorePackageName}'"),
                    }
                );
            NugetRegistry.Register(DaprClientPackageName,
                (framework) => framework switch
                    {
                        ( >= 9, 0) => new PackageVersion("1.15.4")
                            .WithNugetDependency("Dapr.Common", "1.15.4")
                            .WithNugetDependency("Dapr.Protos", "1.15.4")
                            .WithNugetDependency("Google.Api.CommonProtos", "2.2.0")
                            .WithNugetDependency("Google.Protobuf", "3.30.2")
                            .WithNugetDependency("Grpc.Net.Client", "2.71.0")
                            .WithNugetDependency("Microsoft.Extensions.Configuration", "6.0.1")
                            .WithNugetDependency("Microsoft.Extensions.Configuration.Abstractions", "6.0.0")
                            .WithNugetDependency("Microsoft.Extensions.DependencyInjection", "8.0.1")
                            .WithNugetDependency("Microsoft.Extensions.DependencyInjection.Abstractions", "8.0.2")
                            .WithNugetDependency("Microsoft.Extensions.Http", "6.0.0")
                            .WithNugetDependency("Microsoft.Extensions.Logging", "6.0.0")
                            .WithNugetDependency("Microsoft.Extensions.Logging.Abstractions", "6.0.4"),
                        ( >= 8, 0) => new PackageVersion("1.15.4")
                            .WithNugetDependency("Dapr.Common", "1.15.4")
                            .WithNugetDependency("Dapr.Protos", "1.15.4")
                            .WithNugetDependency("Google.Api.CommonProtos", "2.2.0")
                            .WithNugetDependency("Google.Protobuf", "3.30.2")
                            .WithNugetDependency("Grpc.Net.Client", "2.71.0")
                            .WithNugetDependency("Microsoft.Extensions.Configuration", "6.0.1")
                            .WithNugetDependency("Microsoft.Extensions.Configuration.Abstractions", "6.0.0")
                            .WithNugetDependency("Microsoft.Extensions.DependencyInjection", "8.0.1")
                            .WithNugetDependency("Microsoft.Extensions.DependencyInjection.Abstractions", "8.0.2")
                            .WithNugetDependency("Microsoft.Extensions.Http", "6.0.0")
                            .WithNugetDependency("Microsoft.Extensions.Logging", "6.0.0")
                            .WithNugetDependency("Microsoft.Extensions.Logging.Abstractions", "6.0.4"),
                        ( >= 6, 0) => new PackageVersion("1.15.4")
                            .WithNugetDependency("Dapr.Common", "1.15.4")
                            .WithNugetDependency("Dapr.Protos", "1.15.4")
                            .WithNugetDependency("Google.Api.CommonProtos", "2.2.0")
                            .WithNugetDependency("Google.Protobuf", "3.30.2")
                            .WithNugetDependency("Grpc.Net.Client", "2.71.0")
                            .WithNugetDependency("Microsoft.Extensions.Configuration", "6.0.1")
                            .WithNugetDependency("Microsoft.Extensions.Configuration.Abstractions", "6.0.0")
                            .WithNugetDependency("Microsoft.Extensions.DependencyInjection", "8.0.1")
                            .WithNugetDependency("Microsoft.Extensions.DependencyInjection.Abstractions", "8.0.2")
                            .WithNugetDependency("Microsoft.Extensions.Http", "6.0.0")
                            .WithNugetDependency("Microsoft.Extensions.Logging", "6.0.0")
                            .WithNugetDependency("Microsoft.Extensions.Logging.Abstractions", "6.0.4"),
                        _ => throw new Exception($"Unsupported Framework `{framework.Major}` for NuGet package '{DaprClientPackageName}'"),
                    }
                );
            NugetRegistry.Register(DaprExtensionsConfigurationPackageName,
                (framework) => framework switch
                    {
                        ( >= 9, 0) => new PackageVersion("1.15.4")
                            .WithNugetDependency("Dapr.Client", "1.15.4")
                            .WithNugetDependency("Dapr.Common", "1.15.4")
                            .WithNugetDependency("Microsoft.Extensions.Configuration", "6.0.1")
                            .WithNugetDependency("Google.Api.CommonProtos", "2.2.0")
                            .WithNugetDependency("Google.Protobuf", "3.30.2")
                            .WithNugetDependency("Grpc.Net.Client", "2.71.0")
                            .WithNugetDependency("Microsoft.Extensions.Configuration.Abstractions", "6.0.0")
                            .WithNugetDependency("Microsoft.Extensions.DependencyInjection", "8.0.1")
                            .WithNugetDependency("Microsoft.Extensions.DependencyInjection.Abstractions", "8.0.2")
                            .WithNugetDependency("Microsoft.Extensions.Http", "6.0.0")
                            .WithNugetDependency("Microsoft.Extensions.Logging", "6.0.0")
                            .WithNugetDependency("Microsoft.Extensions.Logging.Abstractions", "6.0.4"),
                        ( >= 8, 0) => new PackageVersion("1.15.4")
                            .WithNugetDependency("Dapr.Client", "1.15.4")
                            .WithNugetDependency("Dapr.Common", "1.15.4")
                            .WithNugetDependency("Microsoft.Extensions.Configuration", "6.0.1")
                            .WithNugetDependency("Google.Api.CommonProtos", "2.2.0")
                            .WithNugetDependency("Google.Protobuf", "3.30.2")
                            .WithNugetDependency("Grpc.Net.Client", "2.71.0")
                            .WithNugetDependency("Microsoft.Extensions.Configuration.Abstractions", "6.0.0")
                            .WithNugetDependency("Microsoft.Extensions.DependencyInjection", "8.0.1")
                            .WithNugetDependency("Microsoft.Extensions.DependencyInjection.Abstractions", "8.0.2")
                            .WithNugetDependency("Microsoft.Extensions.Http", "6.0.0")
                            .WithNugetDependency("Microsoft.Extensions.Logging", "6.0.0")
                            .WithNugetDependency("Microsoft.Extensions.Logging.Abstractions", "6.0.4"),
                        ( >= 6, 0) => new PackageVersion("1.15.4")
                            .WithNugetDependency("Dapr.Client", "1.15.4")
                            .WithNugetDependency("Dapr.Common", "1.15.4")
                            .WithNugetDependency("Microsoft.Extensions.Configuration", "6.0.1")
                            .WithNugetDependency("Google.Api.CommonProtos", "2.2.0")
                            .WithNugetDependency("Google.Protobuf", "3.30.2")
                            .WithNugetDependency("Grpc.Net.Client", "2.71.0")
                            .WithNugetDependency("Microsoft.Extensions.Configuration.Abstractions", "6.0.0")
                            .WithNugetDependency("Microsoft.Extensions.DependencyInjection", "8.0.1")
                            .WithNugetDependency("Microsoft.Extensions.DependencyInjection.Abstractions", "8.0.2")
                            .WithNugetDependency("Microsoft.Extensions.Http", "6.0.0")
                            .WithNugetDependency("Microsoft.Extensions.Logging", "6.0.0")
                            .WithNugetDependency("Microsoft.Extensions.Logging.Abstractions", "6.0.4"),
                        _ => throw new Exception($"Unsupported Framework `{framework.Major}` for NuGet package '{DaprExtensionsConfigurationPackageName}'"),
                    }
                );
            NugetRegistry.Register(ManDaprSidekickAspNetCorePackageName,
                (framework) => framework switch
                    {
                        ( >= 2, 0) => new PackageVersion("1.2.2"),
                        _ => throw new Exception($"Unsupported Framework `{framework.Major}` for NuGet package '{ManDaprSidekickAspNetCorePackageName}'"),
                    }
                );
            NugetRegistry.Register(MediatRPackageName,
                (framework) => framework switch
                    {
                        ( >= 6, 0) => new PackageVersion("12.1.1", locked: true),
                        ( >= 2, 0) => new PackageVersion("12.5.0")
                            .WithNugetDependency("MediatR.Contracts", "2.0.1")
                            .WithNugetDependency("Microsoft.Bcl.AsyncInterfaces", "8.0.0")
                            .WithNugetDependency("Microsoft.Extensions.DependencyInjection.Abstractions", "8.0.0"),
                        _ => throw new Exception($"Unsupported Framework `{framework.Major}` for NuGet package '{MediatRPackageName}'"),
                    }
                );
        }

        public static NugetPackageInfo DaprAspNetCore(IOutputTarget outputTarget) => NugetRegistry.GetVersion(DaprAspNetCorePackageName, outputTarget.GetMaxNetAppVersion());

        public static NugetPackageInfo DaprClient(IOutputTarget outputTarget) => NugetRegistry.GetVersion(DaprClientPackageName, outputTarget.GetMaxNetAppVersion());

        public static NugetPackageInfo MediatR(IOutputTarget outputTarget) => NugetRegistry.GetVersion(MediatRPackageName, outputTarget.GetMaxNetAppVersion());

        public static NugetPackageInfo ManDaprSidekickAspNetCore(IOutputTarget outputTarget) => NugetRegistry.GetVersion(ManDaprSidekickAspNetCorePackageName, outputTarget.GetMaxNetAppVersion());

        public static NugetPackageInfo DaprExtensionsConfiguration(IOutputTarget outputTarget) => NugetRegistry.GetVersion(DaprExtensionsConfigurationPackageName, outputTarget.GetMaxNetAppVersion());
    }
}
