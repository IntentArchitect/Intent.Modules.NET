using Intent.Engine;
using Intent.Modules.Common.CSharp.Nuget;
using Intent.Modules.Common.VisualStudio;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.NugetPackages", Version = "1.0")]

namespace Intent.Modules.Aws.DynamoDB
{
    public class NugetPackages : INugetPackages
    {
        public const string AWSSDKDynamoDBv2PackageName = "AWSSDK.DynamoDBv2";

        public void RegisterPackages()
        {
            NugetRegistry.Register(AWSSDKDynamoDBv2PackageName,
                (framework) => (framework.Major, framework.Minor) switch
                    {
                        ( >= 0, >= 0) => new PackageVersion("4.0.7")
                            .WithNugetDependency("AWSSDK.Core", "4.0.0.32"),
                        _ => throw new Exception($"Unsupported Framework `{framework.Major}` for NuGet package '{AWSSDKDynamoDBv2PackageName}'"),
                    }
                );
        }

        public static NugetPackageInfo AWSSDKDynamoDBv2(IOutputTarget outputTarget) => NugetRegistry.GetVersion(AWSSDKDynamoDBv2PackageName, outputTarget.GetMaxNetAppVersion());
    }
}