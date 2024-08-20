using System;
using Intent.Engine;
using Intent.Modules.Common.CSharp.Nuget;
using Intent.Modules.Common.CSharp.VisualStudio;
using Intent.Modules.Common.VisualStudio;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.NugetPackages", Version = "1.0")]

namespace Intent.Modules.AspNetCore.IntegrationTesting
{
    public class NugetPackages : INugetPackages
    {
        public const string AutoFixturePackageName = "AutoFixture";
        public const string IEvangelistAzureCosmosRepositoryPackageName = "IEvangelist.Azure.CosmosRepository";
        public const string MicrosoftAspNetCoreMvcTestingPackageName = "Microsoft.AspNetCore.Mvc.Testing";
        public const string MicrosoftNETTestSdkPackageName = "Microsoft.NET.Test.Sdk";
        public const string TestcontainersCosmosDbPackageName = "Testcontainers.CosmosDb";
        public const string TestcontainersMongoDbPackageName = "Testcontainers.MongoDb";
        public const string TestcontainersMsSqlPackageName = "Testcontainers.MsSql";
        public const string TestcontainersPostgreSqlPackageName = "Testcontainers.PostgreSql";
        public const string XunitPackageName = "xunit";
        public const string XunitRunnerVisualstudioPackageName = "xunit.runner.visualstudio";

        public void RegisterPackages()
        {
            NugetRegistry.Register(AutoFixturePackageName,
                (framework) => framework switch
                    {
                        ( >= 2, 0) => new PackageVersion("4.18.1"),
                        _ => throw new Exception($"Unsupported Framework `{framework.Major}` for NuGet package '{AutoFixturePackageName}'"),
                    }
                );
            NugetRegistry.Register(IEvangelistAzureCosmosRepositoryPackageName,
                (framework) => framework switch
                    {
                        ( >= 8, 0) => new PackageVersion("8.1.7"),
                        ( >= 7, 0) => new PackageVersion("8.1.7"),
                        _ => throw new Exception($"Unsupported Framework `{framework.Major}` for NuGet package '{IEvangelistAzureCosmosRepositoryPackageName}'"),
                    }
                );
            NugetRegistry.Register(MicrosoftAspNetCoreMvcTestingPackageName,
                (framework) => framework switch
                    {
                        ( >= 8, 0) => new PackageVersion("8.0.7"),
                        ( >= 7, 0) => new PackageVersion("7.0.20"),
                        ( >= 6, 0) => new PackageVersion("6.0.32"),
                        _ => throw new Exception($"Unsupported Framework `{framework.Major}` for NuGet package '{MicrosoftAspNetCoreMvcTestingPackageName}'"),
                    }
                );
            NugetRegistry.Register(MicrosoftNETTestSdkPackageName,
                (framework) => framework switch
                    {
                        ( >= 0, 0) => new PackageVersion("15.5.0"),
                        _ => throw new Exception($"Unsupported Framework `{framework.Major}` for NuGet package '{MicrosoftNETTestSdkPackageName}'"),
                    }
                );
            NugetRegistry.Register(TestcontainersCosmosDbPackageName,
                (framework) => framework switch
                    {
                        ( >= 8, 0) => new PackageVersion("3.9.0"),
                        ( >= 6, 0) => new PackageVersion("3.9.0"),
                        _ => throw new Exception($"Unsupported Framework `{framework.Major}` for NuGet package '{TestcontainersCosmosDbPackageName}'"),
                    }
                );
            NugetRegistry.Register(TestcontainersMongoDbPackageName,
                (framework) => framework switch
                    {
                        ( >= 8, 0) => new PackageVersion("3.9.0"),
                        ( >= 6, 0) => new PackageVersion("3.9.0"),
                        _ => throw new Exception($"Unsupported Framework `{framework.Major}` for NuGet package '{TestcontainersMongoDbPackageName}'"),
                    }
                );
            NugetRegistry.Register(TestcontainersMsSqlPackageName,
                (framework) => framework switch
                    {
                        ( >= 8, 0) => new PackageVersion("3.9.0"),
                        ( >= 6, 0) => new PackageVersion("3.9.0"),
                        _ => throw new Exception($"Unsupported Framework `{framework.Major}` for NuGet package '{TestcontainersMsSqlPackageName}'"),
                    }
                );
            NugetRegistry.Register(TestcontainersPostgreSqlPackageName,
                (framework) => framework switch
                    {
                        ( >= 8, 0) => new PackageVersion("3.9.0"),
                        ( >= 6, 0) => new PackageVersion("3.9.0"),
                        _ => throw new Exception($"Unsupported Framework `{framework.Major}` for NuGet package '{TestcontainersPostgreSqlPackageName}'"),
                    }
                );
            NugetRegistry.Register(XunitPackageName,
                (framework) => framework switch
                    {
                        ( >= 0, 0) => new PackageVersion("2.9.0"),
                        _ => throw new Exception($"Unsupported Framework `{framework.Major}` for NuGet package '{XunitPackageName}'"),
                    }
                );
            NugetRegistry.Register(XunitRunnerVisualstudioPackageName,
                (framework) => framework switch
                    {
                        ( >= 6, 0) => new PackageVersion("2.8.2"),
                        _ => throw new Exception($"Unsupported Framework `{framework.Major}` for NuGet package '{XunitRunnerVisualstudioPackageName}'"),
                    }
                );
        }

        public static NugetPackageInfo AutoFixture(IOutputTarget outputTarget) => NugetRegistry.GetVersion(AutoFixturePackageName, outputTarget.GetMaxNetAppVersion());

        public static NugetPackageInfo MicrosoftNETTestSdk(IOutputTarget outputTarget) => NugetRegistry.GetVersion(MicrosoftNETTestSdkPackageName, outputTarget.GetMaxNetAppVersion());

        public static NugetPackageInfo Xunit(IOutputTarget outputTarget) => NugetRegistry.GetVersion(XunitPackageName, outputTarget.GetMaxNetAppVersion());

        public static NugetPackageInfo XunitRunnerVisualstudio(IOutputTarget outputTarget) => NugetRegistry.GetVersion(XunitRunnerVisualstudioPackageName, outputTarget.GetMaxNetAppVersion());

        public static NugetPackageInfo TestcontainersCosmosDb(IOutputTarget outputTarget) => NugetRegistry.GetVersion(TestcontainersCosmosDbPackageName, outputTarget.GetMaxNetAppVersion());

        public static NugetPackageInfo TestcontainersMsSql(IOutputTarget outputTarget) => NugetRegistry.GetVersion(TestcontainersMsSqlPackageName, outputTarget.GetMaxNetAppVersion());

        public static NugetPackageInfo TestcontainersPostgreSql(IOutputTarget outputTarget) => NugetRegistry.GetVersion(TestcontainersPostgreSqlPackageName, outputTarget.GetMaxNetAppVersion());

        public static NugetPackageInfo TestcontainersMongoDb(IOutputTarget outputTarget) => NugetRegistry.GetVersion(TestcontainersMongoDbPackageName, outputTarget.GetMaxNetAppVersion());

        public static NugetPackageInfo IEvangelistAzureCosmosRepository(IOutputTarget outputTarget) => NugetRegistry.GetVersion(IEvangelistAzureCosmosRepositoryPackageName, outputTarget.GetMaxNetAppVersion());

        public static NugetPackageInfo MicrosoftAspNetCoreMvcTesting(IOutputTarget outputTarget) => NugetRegistry.GetVersion(MicrosoftAspNetCoreMvcTestingPackageName, outputTarget.GetMaxNetAppVersion());
    }
}
