using System;
using Intent.Engine;
using Intent.Modules.Common.CSharp.Nuget;
using Intent.Modules.Common.VisualStudio;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.NugetPackages", Version = "1.0")]

namespace Intent.Modules.Aws.SecretsManager
{
    public class NugetPackages : INugetPackages
    {
        public const string AWSSDKSecretsManagerPackageName = "AWSSDK.SecretsManager";

        public void RegisterPackages()
        {
            NugetRegistry.Register(AWSSDKSecretsManagerPackageName,
                (framework) => (framework.Major, framework.Minor) switch
                    {
                        ( >= 0, >= 0) => new PackageVersion("4.0.3")
                            .WithNugetDependency("AWSSDK.Core", "4.0.3.2"),
                        _ => throw new Exception($"Unsupported Framework `{framework.Major}` for NuGet package '{AWSSDKSecretsManagerPackageName}'"),
                    }
                );
        }

        public static NugetPackageInfo AWSSDKSecretsManager(IOutputTarget outputTarget) => NugetRegistry.GetVersion(AWSSDKSecretsManagerPackageName, outputTarget.GetMaxNetAppVersion());
    }
}