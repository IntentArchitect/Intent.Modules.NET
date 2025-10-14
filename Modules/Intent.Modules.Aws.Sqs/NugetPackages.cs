using System;
using Intent.Engine;
using Intent.Modules.Common.CSharp.Nuget;
using Intent.Modules.Common.VisualStudio;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.NugetPackages", Version = "1.0")]

namespace Intent.Modules.Aws.Sqs
{
    public class NugetPackages : INugetPackages
    {
        public const string AwsSdkSqsPackageName = "AWSSDK.SQS";
        public const string AwsSdkExtensionsNetCoreSetupPackageName = "AWSSDK.Extensions.NETCore.Setup";
        public const string AmazonLambdaSqsEventsPackageName = "Amazon.Lambda.SQSEvents";
        public const string AmazonLambdaCorePackageName = "Amazon.Lambda.Core";

        public void RegisterPackages()
        {
            // AWSSDK.SQS
            NugetRegistry.Register(AwsSdkSqsPackageName,
                (framework) => (framework.Major, framework.Minor) switch
                    {
                        ( >= 8, >= 0) => new PackageVersion("3.7.400.61")
                            .WithNugetDependency("AWSSDK.Core", "3.7.400.61"),
                        ( >= 6, >= 0) => new PackageVersion("3.7.400.61")
                            .WithNugetDependency("AWSSDK.Core", "3.7.400.61"),
                        _ => throw new Exception($"Unsupported Framework `{framework.Major}` for NuGet package '{AwsSdkSqsPackageName}'"),
                    }
                );

            // AWSSDK.Extensions.NETCore.Setup
            NugetRegistry.Register(AwsSdkExtensionsNetCoreSetupPackageName,
                (framework) => (framework.Major, framework.Minor) switch
                    {
                        ( >= 8, >= 0) => new PackageVersion("3.7.301")
                            .WithNugetDependency("AWSSDK.Core", "3.7.400.61")
                            .WithNugetDependency("Microsoft.Extensions.Configuration.Abstractions", "8.0.0")
                            .WithNugetDependency("Microsoft.Extensions.DependencyInjection.Abstractions", "8.0.2")
                            .WithNugetDependency("Microsoft.Extensions.Logging.Abstractions", "8.0.2"),
                        ( >= 6, >= 0) => new PackageVersion("3.7.301")
                            .WithNugetDependency("AWSSDK.Core", "3.7.400.61")
                            .WithNugetDependency("Microsoft.Extensions.Configuration.Abstractions", "6.0.0")
                            .WithNugetDependency("Microsoft.Extensions.DependencyInjection.Abstractions", "6.0.0")
                            .WithNugetDependency("Microsoft.Extensions.Logging.Abstractions", "6.0.0"),
                        _ => throw new Exception($"Unsupported Framework `{framework.Major}` for NuGet package '{AwsSdkExtensionsNetCoreSetupPackageName}'"),
                    }
                );

            // Amazon.Lambda.SQSEvents
            NugetRegistry.Register(AmazonLambdaSqsEventsPackageName,
                (framework) => (framework.Major, framework.Minor) switch
                    {
                        ( >= 6, >= 0) => new PackageVersion("2.2.0")
                            .WithNugetDependency("Amazon.Lambda.Core", "2.2.0"),
                        _ => throw new Exception($"Unsupported Framework `{framework.Major}` for NuGet package '{AmazonLambdaSqsEventsPackageName}'"),
                    }
                );

            // Amazon.Lambda.Core
            NugetRegistry.Register(AmazonLambdaCorePackageName,
                (framework) => (framework.Major, framework.Minor) switch
                    {
                        ( >= 6, >= 0) => new PackageVersion("2.2.0"),
                        _ => throw new Exception($"Unsupported Framework `{framework.Major}` for NuGet package '{AmazonLambdaCorePackageName}'"),
                    }
                );
        }

        public static NugetPackageInfo AwsSdkSqs(IOutputTarget outputTarget) 
            => NugetRegistry.GetVersion(AwsSdkSqsPackageName, outputTarget.GetMaxNetAppVersion());

        public static NugetPackageInfo AwsSdkExtensionsNetCoreSetup(IOutputTarget outputTarget) 
            => NugetRegistry.GetVersion(AwsSdkExtensionsNetCoreSetupPackageName, outputTarget.GetMaxNetAppVersion());

        public static NugetPackageInfo AmazonLambdaSqsEvents(IOutputTarget outputTarget) 
            => NugetRegistry.GetVersion(AmazonLambdaSqsEventsPackageName, outputTarget.GetMaxNetAppVersion());

        public static NugetPackageInfo AmazonLambdaCore(IOutputTarget outputTarget) 
            => NugetRegistry.GetVersion(AmazonLambdaCorePackageName, outputTarget.GetMaxNetAppVersion());
    }
}
