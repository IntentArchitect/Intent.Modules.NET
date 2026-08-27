using System;
using Intent.Engine;
using Intent.Modules.Common.CSharp.Nuget;
using Intent.Modules.Common.VisualStudio;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.NugetPackages", Version = "1.0")]

namespace Intent.Modules.Eventing.Wolverine
{
    public class NugetPackages : INugetPackages
    {
        public const string WolverineFxPackageName = "WolverineFx";
        public const string WolverineFxAmazonSnsPackageName = "WolverineFx.AmazonSns";
        public const string WolverineFxAmazonSqsPackageName = "WolverineFx.AmazonSqs";
        public const string WolverineFxAzureServiceBusPackageName = "WolverineFx.AzureServiceBus";
        public const string WolverineFxEntityFrameworkCorePackageName = "WolverineFx.EntityFrameworkCore";
        public const string WolverineFxPostgresqlPackageName = "WolverineFx.Postgresql";
        public const string WolverineFxRabbitMQPackageName = "WolverineFx.RabbitMQ";
        public const string WolverineFxSqlServerPackageName = "WolverineFx.SqlServer";

        public void RegisterPackages()
        {
            NugetRegistry.Register(WolverineFxPackageName,
                (framework) => (framework.Major, framework.Minor) switch
                    {
                        ( >= 8, >= 0) => new PackageVersion("5.39.5"),
                        _ => throw new Exception($"Unsupported Framework `{framework.Major}` for NuGet package '{WolverineFxPackageName}'"),
                    }
                );
            NugetRegistry.Register(WolverineFxAmazonSnsPackageName,
                (framework) => (framework.Major, framework.Minor) switch
                    {
                        ( >= 8, >= 0) => new PackageVersion("5.39.5"),
                        _ => throw new Exception($"Unsupported Framework `{framework.Major}` for NuGet package '{WolverineFxAmazonSnsPackageName}'"),
                    }
                );
            NugetRegistry.Register(WolverineFxAmazonSqsPackageName,
                (framework) => (framework.Major, framework.Minor) switch
                    {
                        ( >= 8, >= 0) => new PackageVersion("5.39.5"),
                        _ => throw new Exception($"Unsupported Framework `{framework.Major}` for NuGet package '{WolverineFxAmazonSqsPackageName}'"),
                    }
                );
            NugetRegistry.Register(WolverineFxAzureServiceBusPackageName,
                (framework) => (framework.Major, framework.Minor) switch
                    {
                        ( >= 8, >= 0) => new PackageVersion("5.39.5"),
                        _ => throw new Exception($"Unsupported Framework `{framework.Major}` for NuGet package '{WolverineFxAzureServiceBusPackageName}'"),
                    }
                );
            NugetRegistry.Register(WolverineFxEntityFrameworkCorePackageName,
                (framework) => (framework.Major, framework.Minor) switch
                    {
                        ( >= 8, >= 0) => new PackageVersion("5.39.5"),
                        _ => throw new Exception($"Unsupported Framework `{framework.Major}` for NuGet package '{WolverineFxEntityFrameworkCorePackageName}'"),
                    }
                );
            NugetRegistry.Register(WolverineFxPostgresqlPackageName,
                (framework) => (framework.Major, framework.Minor) switch
                    {
                        ( >= 8, >= 0) => new PackageVersion("5.39.5"),
                        _ => throw new Exception($"Unsupported Framework `{framework.Major}` for NuGet package '{WolverineFxPostgresqlPackageName}'"),
                    }
                );
            NugetRegistry.Register(WolverineFxRabbitMQPackageName,
                (framework) => (framework.Major, framework.Minor) switch
                    {
                        ( >= 8, >= 0) => new PackageVersion("5.39.5"),
                        _ => throw new Exception($"Unsupported Framework `{framework.Major}` for NuGet package '{WolverineFxRabbitMQPackageName}'"),
                    }
                );
            NugetRegistry.Register(WolverineFxSqlServerPackageName,
                (framework) => (framework.Major, framework.Minor) switch
                    {
                        ( >= 8, >= 0) => new PackageVersion("5.39.5"),
                        _ => throw new Exception($"Unsupported Framework `{framework.Major}` for NuGet package '{WolverineFxSqlServerPackageName}'"),
                    }
                );
        }

        public static NugetPackageInfo WolverineFx(IOutputTarget outputTarget) => NugetRegistry.GetVersion(WolverineFxPackageName, outputTarget.GetMaxNetAppVersion());

        public static NugetPackageInfo WolverineFxAmazonSns(IOutputTarget outputTarget) => NugetRegistry.GetVersion(WolverineFxAmazonSnsPackageName, outputTarget.GetMaxNetAppVersion());

        public static NugetPackageInfo WolverineFxAmazonSqs(IOutputTarget outputTarget) => NugetRegistry.GetVersion(WolverineFxAmazonSqsPackageName, outputTarget.GetMaxNetAppVersion());

        public static NugetPackageInfo WolverineFxAzureServiceBus(IOutputTarget outputTarget) => NugetRegistry.GetVersion(WolverineFxAzureServiceBusPackageName, outputTarget.GetMaxNetAppVersion());

        public static NugetPackageInfo WolverineFxEntityFrameworkCore(IOutputTarget outputTarget) => NugetRegistry.GetVersion(WolverineFxEntityFrameworkCorePackageName, outputTarget.GetMaxNetAppVersion());

        public static NugetPackageInfo WolverineFxPostgresql(IOutputTarget outputTarget) => NugetRegistry.GetVersion(WolverineFxPostgresqlPackageName, outputTarget.GetMaxNetAppVersion());

        public static NugetPackageInfo WolverineFxRabbitMQ(IOutputTarget outputTarget) => NugetRegistry.GetVersion(WolverineFxRabbitMQPackageName, outputTarget.GetMaxNetAppVersion());

        public static NugetPackageInfo WolverineFxSqlServer(IOutputTarget outputTarget) => NugetRegistry.GetVersion(WolverineFxSqlServerPackageName, outputTarget.GetMaxNetAppVersion());
    }
}