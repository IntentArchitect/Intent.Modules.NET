using System;
using Intent.Engine;
using Intent.Modules.Common.CSharp.Nuget;
using Intent.Modules.Common.VisualStudio;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.NugetPackages", Version = "1.0")]

namespace Intent.Modules.Aws.Lambda.Functions
{
    public class NugetPackages : INugetPackages
    {
        public const string AmazonLambdaAnnotationsPackageName = "Amazon.Lambda.Annotations";
        public const string AmazonLambdaAPIGatewayEventsPackageName = "Amazon.Lambda.APIGatewayEvents";
        public const string AmazonLambdaCorePackageName = "Amazon.Lambda.Core";
        public const string AmazonLambdaLoggingAspNetCorePackageName = "Amazon.Lambda.Logging.AspNetCore";
        public const string AmazonLambdaSerializationSystemTextJsonPackageName = "Amazon.Lambda.Serialization.SystemTextJson";

        public void RegisterPackages()
        {
            NugetRegistry.Register(AmazonLambdaAnnotationsPackageName,
                (framework) => (framework.Major, framework.Minor) switch
                    {
                        ( >= 8, >= 0) => new PackageVersion("1.7.0")
                            .WithNugetDependency("Amazon.Lambda.Core", "2.5.1"),
                        ( >= 6, >= 0) => new PackageVersion("1.7.0")
                            .WithNugetDependency("Amazon.Lambda.Core", "2.5.1"),
                        ( >= 2, >= 0) => new PackageVersion("1.7.0")
                            .WithNugetDependency("Amazon.Lambda.Core", "2.5.1"),
                        _ => throw new Exception($"Unsupported Framework `{framework.Major}` for NuGet package '{AmazonLambdaAnnotationsPackageName}'"),
                    }
                );
            NugetRegistry.Register(AmazonLambdaAPIGatewayEventsPackageName,
                (framework) => (framework.Major, framework.Minor) switch
                    {
                        ( >= 8, >= 0) => new PackageVersion("2.7.1"),
                        ( >= 2, >= 0) => new PackageVersion("2.7.1")
                            .WithNugetDependency("Newtonsoft.Json", "13.0.1"),
                        _ => throw new Exception($"Unsupported Framework `{framework.Major}` for NuGet package '{AmazonLambdaAPIGatewayEventsPackageName}'"),
                    }
                );
            NugetRegistry.Register(AmazonLambdaCorePackageName,
                (framework) => (framework.Major, framework.Minor) switch
                    {
                        ( >= 8, >= 0) => new PackageVersion("2.7.0"),
                        ( >= 6, >= 0) => new PackageVersion("2.7.0"),
                        ( >= 2, >= 0) => new PackageVersion("2.7.0"),
                        _ => throw new Exception($"Unsupported Framework `{framework.Major}` for NuGet package '{AmazonLambdaCorePackageName}'"),
                    }
                );
            NugetRegistry.Register(AmazonLambdaLoggingAspNetCorePackageName,
                (framework) => (framework.Major, framework.Minor) switch
                    {
                        ( >= 8, >= 0) => new PackageVersion("4.1.0")
                            .WithNugetDependency("Amazon.Lambda.Core", "2.7.0")
                            .WithNugetDependency("Microsoft.Extensions.Configuration.Abstractions", "2.1.0")
                            .WithNugetDependency("Microsoft.Extensions.Logging", "2.1.0")
                            .WithNugetDependency("Microsoft.Extensions.Logging.Abstractions", "2.1.0"),
                        ( >= 6, >= 0) => new PackageVersion("4.1.0")
                            .WithNugetDependency("Amazon.Lambda.Core", "2.7.0")
                            .WithNugetDependency("Microsoft.Extensions.Configuration.Abstractions", "2.1.0")
                            .WithNugetDependency("Microsoft.Extensions.Logging", "2.1.0")
                            .WithNugetDependency("Microsoft.Extensions.Logging.Abstractions", "2.1.0"),
                        ( >= 2, >= 0) => new PackageVersion("3.1.1")
                            .WithNugetDependency("Amazon.Lambda.Core", "2.5.0")
                            .WithNugetDependency("Microsoft.Extensions.Configuration.Abstractions", "2.1.0")
                            .WithNugetDependency("Microsoft.Extensions.Logging", "2.1.0")
                            .WithNugetDependency("Microsoft.Extensions.Logging.Abstractions", "2.1.0"),
                        _ => throw new Exception($"Unsupported Framework `{framework.Major}` for NuGet package '{AmazonLambdaLoggingAspNetCorePackageName}'"),
                    }
                );
            NugetRegistry.Register(AmazonLambdaSerializationSystemTextJsonPackageName,
                (framework) => (framework.Major, framework.Minor) switch
                    {
                        ( >= 8, >= 0) => new PackageVersion("2.4.4")
                            .WithNugetDependency("Amazon.Lambda.Core", "2.3.0"),
                        ( >= 6, >= 0) => new PackageVersion("2.4.4")
                            .WithNugetDependency("Amazon.Lambda.Core", "2.3.0"),
                        _ => throw new Exception($"Unsupported Framework `{framework.Major}` for NuGet package '{AmazonLambdaSerializationSystemTextJsonPackageName}'"),
                    }
                );
        }

        public static NugetPackageInfo AmazonLambdaAnnotations(IOutputTarget outputTarget) => NugetRegistry.GetVersion(AmazonLambdaAnnotationsPackageName, outputTarget.GetMaxNetAppVersion());

        public static NugetPackageInfo AmazonLambdaAPIGatewayEvents(IOutputTarget outputTarget) => NugetRegistry.GetVersion(AmazonLambdaAPIGatewayEventsPackageName, outputTarget.GetMaxNetAppVersion());

        public static NugetPackageInfo AmazonLambdaCore(IOutputTarget outputTarget) => NugetRegistry.GetVersion(AmazonLambdaCorePackageName, outputTarget.GetMaxNetAppVersion());

        public static NugetPackageInfo AmazonLambdaLoggingAspNetCore(IOutputTarget outputTarget) => NugetRegistry.GetVersion(AmazonLambdaLoggingAspNetCorePackageName, outputTarget.GetMaxNetAppVersion());

        public static NugetPackageInfo AmazonLambdaSerializationSystemTextJson(IOutputTarget outputTarget) => NugetRegistry.GetVersion(AmazonLambdaSerializationSystemTextJsonPackageName, outputTarget.GetMaxNetAppVersion());
    }
}