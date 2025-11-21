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
        public const string AmazonLambdaSqsEventsPackageName = "Amazon.Lambda.SQSEvents";
        public const string AwsSdkSqsPackageName = "AWSSDK.SQS";

        public void RegisterPackages()
        {
            NugetRegistry.Register(AmazonLambdaSqsEventsPackageName,
                (framework) => (framework.Major, framework.Minor) switch
                    {
                        ( >= 8, >= 0) => new PackageVersion("2.2.0"),
                        ( >= 2, >= 0) => new PackageVersion("2.2.0"),
                        _ => throw new Exception($"Unsupported Framework `{framework.Major}` for NuGet package '{AmazonLambdaSqsEventsPackageName}'"),
                    }
                );

            NugetRegistry.Register(AwsSdkSqsPackageName,
                (framework) => (framework.Major, framework.Minor) switch
                    {
                        ( >= 0, >= 0) => new PackageVersion("4.0.2.4")
                            .WithNugetDependency("AWSSDK.Core", "4.0.3.2"),
                        _ => throw new Exception($"Unsupported Framework `{framework.Major}` for NuGet package '{AwsSdkSqsPackageName}'"),
                    }
                );
        }

        public static NugetPackageInfo AmazonLambdaSqsEvents(IOutputTarget outputTarget) => NugetRegistry.GetVersion(AmazonLambdaSqsEventsPackageName, outputTarget.GetMaxNetAppVersion());

        public static NugetPackageInfo AwsSdkSqs(IOutputTarget outputTarget) => NugetRegistry.GetVersion(AwsSdkSqsPackageName, outputTarget.GetMaxNetAppVersion());
    }
}
