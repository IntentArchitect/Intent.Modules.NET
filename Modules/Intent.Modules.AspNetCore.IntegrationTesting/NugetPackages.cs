using System;
using Intent.Engine;
using Intent.Modules.Common.CSharp.Nuget;
using Intent.Modules.Common.VisualStudio;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.NugetPackages", Version = "1.0")]

namespace Intent.Modules.AspNetCore.IntegrationTesting
{
    public class NugetPackages : INugetPackages
    {
        public const string AutoFixturePackageName = "AutoFixture";
        public const string IdentityModelAspNetCorePackageName = "IdentityModel.AspNetCore";
        public const string IEvangelistAzureCosmosRepositoryPackageName = "IEvangelist.Azure.CosmosRepository";
        public const string MicrosoftAspNetCoreMvcTestingPackageName = "Microsoft.AspNetCore.Mvc.Testing";
        public const string MicrosoftAspNetCoreWebUtilitiesPackageName = "Microsoft.AspNetCore.WebUtilities";
        public const string MicrosoftExtensionsHttpPackageName = "Microsoft.Extensions.Http";
        public const string MicrosoftNETTestSdkPackageName = "Microsoft.NET.Test.Sdk";
        public const string SystemTextJsonPackageName = "System.Text.Json";
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
            NugetRegistry.Register(IdentityModelAspNetCorePackageName,
                (framework) => framework switch
                    {
                        ( >= 6, 0) => new PackageVersion("4.3.0")
                            .WithNugetDependency("IdentityModel", "6.0.0")
                            .WithNugetDependency("Microsoft.AspNetCore.Authentication.OpenIdConnect", "6.0.0"),
                        ( >= 2, 0) => new PackageVersion("2.0.0")
                            .WithNugetDependency("IdentityModel", "4.3.0")
                            .WithNugetDependency("Microsoft.AspNetCore.Authentication", "2.1.0")
                            .WithNugetDependency("Microsoft.AspNetCore.Authentication.OpenIdConnect", "2.1.0")
                            .WithNugetDependency("Microsoft.Extensions.Caching.Abstractions", "2.1.0")
                            .WithNugetDependency("Microsoft.Extensions.Http", "2.1.0"),
                        _ => throw new Exception($"Unsupported Framework `{framework.Major}` for NuGet package '{IdentityModelAspNetCorePackageName}'"),
                    }
                );
            NugetRegistry.Register(IEvangelistAzureCosmosRepositoryPackageName,
                (framework) => framework switch
                    {
                        ( >= 8, 0) => new PackageVersion("8.1.7"),
                        ( >= 7, 0) => new PackageVersion("8.1.7"),
                        ( >= 2, 0) => new PackageVersion("8.1.7")
                            .WithNugetDependency("Microsoft.Azure.Cosmos", "3.38.0")
                            .WithNugetDependency("Microsoft.Extensions.Http", "8.0.0")
                            .WithNugetDependency("Microsoft.Extensions.Logging", "8.0.0")
                            .WithNugetDependency("Microsoft.Extensions.Options.ConfigurationExtensions", "8.0.0"),
                        _ => throw new Exception($"Unsupported Framework `{framework.Major}` for NuGet package '{IEvangelistAzureCosmosRepositoryPackageName}'"),
                    }
                );
            NugetRegistry.Register(MicrosoftAspNetCoreMvcTestingPackageName,
                (framework) => framework switch
                    {
                        ( >= 9, 0) => new PackageVersion("9.0.0")
                            .WithNugetDependency("Microsoft.AspNetCore.TestHost", "9.0.0")
                            .WithNugetDependency("Microsoft.Extensions.DependencyModel", "9.0.0")
                            .WithNugetDependency("Microsoft.Extensions.Hosting", "9.0.0"),
                        ( >= 8, 0) => new PackageVersion("8.0.11")
                            .WithNugetDependency("Microsoft.AspNetCore.TestHost", "8.0.11")
                            .WithNugetDependency("Microsoft.Extensions.DependencyModel", "8.0.2")
                            .WithNugetDependency("Microsoft.Extensions.Hosting", "8.0.1"),
                        ( >= 7, 0) => new PackageVersion("7.0.20"),
                        ( >= 6, 0) => new PackageVersion("6.0.36")
                            .WithNugetDependency("Microsoft.AspNetCore.TestHost", "6.0.36")
                            .WithNugetDependency("Microsoft.Extensions.DependencyModel", "6.0.2")
                            .WithNugetDependency("Microsoft.Extensions.Hosting", "6.0.1"),
                        ( >= 2, 0) => new PackageVersion("2.2.0")
                            .WithNugetDependency("Microsoft.AspNetCore.Mvc.Core", "2.2.0")
                            .WithNugetDependency("Microsoft.AspNetCore.TestHost", "2.2.0"),
                        _ => throw new Exception($"Unsupported Framework `{framework.Major}` for NuGet package '{MicrosoftAspNetCoreMvcTestingPackageName}'"),
                    }
                );
            NugetRegistry.Register(MicrosoftAspNetCoreWebUtilitiesPackageName,
                (framework) => framework switch
                    {
                        ( >= 9, 0) => new PackageVersion("9.0.0")
                            .WithNugetDependency("Microsoft.Net.Http.Headers", "9.0.0"),
                        ( >= 8, 0) => new PackageVersion("8.0.11")
                            .WithNugetDependency("Microsoft.Net.Http.Headers", "8.0.11")
                            .WithNugetDependency("System.IO.Pipelines", "8.0.0"),
                        ( >= 2, 0) => new PackageVersion("2.2.0")
                            .WithNugetDependency("Microsoft.Net.Http.Headers", "2.2.0")
                            .WithNugetDependency("System.Text.Encodings.Web", "4.5.0"),
                        _ => throw new Exception($"Unsupported Framework `{framework.Major}` for NuGet package '{MicrosoftAspNetCoreWebUtilitiesPackageName}'"),
                    }
                );
            NugetRegistry.Register(MicrosoftExtensionsHttpPackageName,
                (framework) => framework switch
                    {
                        ( >= 9, 0) => new PackageVersion("9.0.0")
                            .WithNugetDependency("Microsoft.Extensions.Configuration.Abstractions", "9.0.0")
                            .WithNugetDependency("Microsoft.Extensions.DependencyInjection.Abstractions", "9.0.0")
                            .WithNugetDependency("Microsoft.Extensions.Diagnostics", "9.0.0")
                            .WithNugetDependency("Microsoft.Extensions.Logging", "9.0.0")
                            .WithNugetDependency("Microsoft.Extensions.Logging.Abstractions", "9.0.0")
                            .WithNugetDependency("Microsoft.Extensions.Options", "9.0.0"),
                        ( >= 8, 0) => new PackageVersion("9.0.0")
                            .WithNugetDependency("Microsoft.Extensions.Configuration.Abstractions", "9.0.0")
                            .WithNugetDependency("Microsoft.Extensions.DependencyInjection.Abstractions", "9.0.0")
                            .WithNugetDependency("Microsoft.Extensions.Diagnostics", "9.0.0")
                            .WithNugetDependency("Microsoft.Extensions.Logging", "9.0.0")
                            .WithNugetDependency("Microsoft.Extensions.Logging.Abstractions", "9.0.0")
                            .WithNugetDependency("Microsoft.Extensions.Options", "9.0.0"),
                        ( >= 2, 0) => new PackageVersion("9.0.0")
                            .WithNugetDependency("Microsoft.Extensions.Configuration.Abstractions", "9.0.0")
                            .WithNugetDependency("Microsoft.Extensions.DependencyInjection.Abstractions", "9.0.0")
                            .WithNugetDependency("Microsoft.Extensions.Logging", "9.0.0")
                            .WithNugetDependency("Microsoft.Extensions.Logging.Abstractions", "9.0.0")
                            .WithNugetDependency("Microsoft.Extensions.Options", "9.0.0"),
                        _ => throw new Exception($"Unsupported Framework `{framework.Major}` for NuGet package '{MicrosoftExtensionsHttpPackageName}'"),
                    }
                );
            NugetRegistry.Register(MicrosoftNETTestSdkPackageName,
                (framework) => framework switch
                    {
                        ( >= 0, 0) => new PackageVersion("15.5.0")
                            .WithNugetDependency("Microsoft.CodeCoverage", "1.0.3")
                            .WithNugetDependency("Microsoft.TestPlatform.TestHost", "15.5.0"),
                        _ => throw new Exception($"Unsupported Framework `{framework.Major}` for NuGet package '{MicrosoftNETTestSdkPackageName}'"),
                    }
                );
            NugetRegistry.Register(SystemTextJsonPackageName,
                (framework) => framework switch
                    {
                        ( >= 9, 0) => new PackageVersion("9.0.0"),
                        ( >= 8, 0) => new PackageVersion("9.0.0")
                            .WithNugetDependency("System.IO.Pipelines", "9.0.0")
                            .WithNugetDependency("System.Text.Encodings.Web", "9.0.0"),
                        ( >= 2, 0) => new PackageVersion("9.0.0")
                            .WithNugetDependency("Microsoft.Bcl.AsyncInterfaces", "9.0.0")
                            .WithNugetDependency("System.Buffers", "4.5.1")
                            .WithNugetDependency("System.IO.Pipelines", "9.0.0")
                            .WithNugetDependency("System.Memory", "4.5.5")
                            .WithNugetDependency("System.Runtime.CompilerServices.Unsafe", "6.0.0")
                            .WithNugetDependency("System.Text.Encodings.Web", "9.0.0")
                            .WithNugetDependency("System.Threading.Tasks.Extensions", "4.5.4"),
                        _ => throw new Exception($"Unsupported Framework `{framework.Major}` for NuGet package '{SystemTextJsonPackageName}'"),
                    }
                );
            NugetRegistry.Register(TestcontainersCosmosDbPackageName,
                (framework) => framework switch
                    {
                        ( >= 8, 0) => new PackageVersion("4.0.0")
                            .WithNugetDependency("Testcontainers", "4.0.0"),
                        ( >= 6, 0) => new PackageVersion("4.0.0")
                            .WithNugetDependency("Testcontainers", "4.0.0"),
                        ( >= 2, 1) => new PackageVersion("4.0.0")
                            .WithNugetDependency("Testcontainers", "4.0.0"),
                        ( >= 2, 0) => new PackageVersion("4.0.0")
                            .WithNugetDependency("Testcontainers", "4.0.0"),
                        _ => throw new Exception($"Unsupported Framework `{framework.Major}` for NuGet package '{TestcontainersCosmosDbPackageName}'"),
                    }
                );
            NugetRegistry.Register(TestcontainersMongoDbPackageName,
                (framework) => framework switch
                    {
                        ( >= 8, 0) => new PackageVersion("4.0.0")
                            .WithNugetDependency("Testcontainers", "4.0.0"),
                        ( >= 6, 0) => new PackageVersion("4.0.0")
                            .WithNugetDependency("Testcontainers", "4.0.0"),
                        ( >= 2, 1) => new PackageVersion("4.0.0")
                            .WithNugetDependency("Testcontainers", "4.0.0"),
                        ( >= 2, 0) => new PackageVersion("4.0.0")
                            .WithNugetDependency("Testcontainers", "4.0.0"),
                        _ => throw new Exception($"Unsupported Framework `{framework.Major}` for NuGet package '{TestcontainersMongoDbPackageName}'"),
                    }
                );
            NugetRegistry.Register(TestcontainersMsSqlPackageName,
                (framework) => framework switch
                    {
                        ( >= 8, 0) => new PackageVersion("4.0.0")
                            .WithNugetDependency("Testcontainers", "4.0.0"),
                        ( >= 6, 0) => new PackageVersion("4.0.0")
                            .WithNugetDependency("Testcontainers", "4.0.0"),
                        ( >= 2, 1) => new PackageVersion("4.0.0")
                            .WithNugetDependency("Testcontainers", "4.0.0"),
                        ( >= 2, 0) => new PackageVersion("4.0.0")
                            .WithNugetDependency("Testcontainers", "4.0.0"),
                        _ => throw new Exception($"Unsupported Framework `{framework.Major}` for NuGet package '{TestcontainersMsSqlPackageName}'"),
                    }
                );
            NugetRegistry.Register(TestcontainersPostgreSqlPackageName,
                (framework) => framework switch
                    {
                        ( >= 8, 0) => new PackageVersion("4.0.0")
                            .WithNugetDependency("Testcontainers", "4.0.0"),
                        ( >= 6, 0) => new PackageVersion("4.0.0")
                            .WithNugetDependency("Testcontainers", "4.0.0"),
                        ( >= 2, 1) => new PackageVersion("4.0.0")
                            .WithNugetDependency("Testcontainers", "4.0.0"),
                        ( >= 2, 0) => new PackageVersion("4.0.0")
                            .WithNugetDependency("Testcontainers", "4.0.0"),
                        _ => throw new Exception($"Unsupported Framework `{framework.Major}` for NuGet package '{TestcontainersPostgreSqlPackageName}'"),
                    }
                );
            NugetRegistry.Register(XunitPackageName,
                (framework) => framework switch
                    {
                        ( >= 0, 0) => new PackageVersion("2.9.2")
                            .WithNugetDependency("xunit.analyzers", "1.16.0")
                            .WithNugetDependency("xunit.assert", "2.9.2")
                            .WithNugetDependency("xunit.core", "2.9.2"),
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

        public static NugetPackageInfo IdentityModelAspNetCore(IOutputTarget outputTarget) => NugetRegistry.GetVersion(IdentityModelAspNetCorePackageName, outputTarget.GetMaxNetAppVersion());

        public static NugetPackageInfo MicrosoftNETTestSdk(IOutputTarget outputTarget) => NugetRegistry.GetVersion(MicrosoftNETTestSdkPackageName, outputTarget.GetMaxNetAppVersion());

        public static NugetPackageInfo SystemTextJson(IOutputTarget outputTarget) => NugetRegistry.GetVersion(SystemTextJsonPackageName, outputTarget.GetMaxNetAppVersion());

        public static NugetPackageInfo Xunit(IOutputTarget outputTarget) => NugetRegistry.GetVersion(XunitPackageName, outputTarget.GetMaxNetAppVersion());

        public static NugetPackageInfo XunitRunnerVisualstudio(IOutputTarget outputTarget) => NugetRegistry.GetVersion(XunitRunnerVisualstudioPackageName, outputTarget.GetMaxNetAppVersion());

        public static NugetPackageInfo TestcontainersCosmosDb(IOutputTarget outputTarget) => NugetRegistry.GetVersion(TestcontainersCosmosDbPackageName, outputTarget.GetMaxNetAppVersion());

        public static NugetPackageInfo TestcontainersMsSql(IOutputTarget outputTarget) => NugetRegistry.GetVersion(TestcontainersMsSqlPackageName, outputTarget.GetMaxNetAppVersion());

        public static NugetPackageInfo TestcontainersPostgreSql(IOutputTarget outputTarget) => NugetRegistry.GetVersion(TestcontainersPostgreSqlPackageName, outputTarget.GetMaxNetAppVersion());

        public static NugetPackageInfo TestcontainersMongoDb(IOutputTarget outputTarget) => NugetRegistry.GetVersion(TestcontainersMongoDbPackageName, outputTarget.GetMaxNetAppVersion());

        public static NugetPackageInfo IEvangelistAzureCosmosRepository(IOutputTarget outputTarget) => NugetRegistry.GetVersion(IEvangelistAzureCosmosRepositoryPackageName, outputTarget.GetMaxNetAppVersion());

        public static NugetPackageInfo MicrosoftAspNetCoreMvcTesting(IOutputTarget outputTarget) => NugetRegistry.GetVersion(MicrosoftAspNetCoreMvcTestingPackageName, outputTarget.GetMaxNetAppVersion());

        public static NugetPackageInfo MicrosoftAspNetCoreWebUtilities(IOutputTarget outputTarget) => NugetRegistry.GetVersion(MicrosoftAspNetCoreWebUtilitiesPackageName, outputTarget.GetMaxNetAppVersion());

        public static NugetPackageInfo MicrosoftExtensionsHttp(IOutputTarget outputTarget) => NugetRegistry.GetVersion(MicrosoftExtensionsHttpPackageName, outputTarget.GetMaxNetAppVersion());
    }
}
