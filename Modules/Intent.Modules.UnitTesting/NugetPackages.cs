using System;
using Intent.Engine;
using Intent.Modules.Common.CSharp.Nuget;
using Intent.Modules.Common.VisualStudio;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.NugetPackages", Version = "1.0")]

namespace Intent.Modules.UnitTesting
{
    public class NugetPackages : INugetPackages
    {
        public const string CoverletCollectorPackageName = "coverlet.collector";
        public const string MicrosoftNETTestSdkPackageName = "Microsoft.NET.Test.Sdk";
        public const string MoqPackageName = "Moq";
        public const string NSubstitutePackageName = "NSubstitute";
        public const string XunitRunnerVisualstudioPackageName = "xunit.runner.visualstudio";
        public const string XunitV3PackageName = "xunit.v3";

        public void RegisterPackages()
        {
            NugetRegistry.Register(CoverletCollectorPackageName,
                (framework) => (framework.Major, framework.Minor) switch
                    {
                        ( >= 0, >= 0) => new PackageVersion("6.0.4"),
                        _ => throw new Exception($"Unsupported Framework `{framework.Major}` for NuGet package '{CoverletCollectorPackageName}'"),
                    }
                );
            NugetRegistry.Register(MicrosoftNETTestSdkPackageName,
                (framework) => (framework.Major, framework.Minor) switch
                    {
                        ( >= 0, >= 0) => new PackageVersion("17.13.0", locked: true)
                            .WithNugetDependency("Microsoft.CodeCoverage", "17.13.0")
                            .WithNugetDependency("Microsoft.TestPlatform.TestHost", "17.13.0"),
                        _ => throw new Exception($"Unsupported Framework `{framework.Major}` for NuGet package '{MicrosoftNETTestSdkPackageName}'"),
                    }
                );
            NugetRegistry.Register(MoqPackageName,
                (framework) => (framework.Major, framework.Minor) switch
                    {
                        ( >= 6, >= 0) => new PackageVersion("4.20.72")
                            .WithNugetDependency("Castle.Core", "5.1.1"),
                        ( >= 2, >= 1) => new PackageVersion("4.20.72")
                            .WithNugetDependency("Castle.Core", "5.1.1"),
                        ( >= 2, >= 0) => new PackageVersion("4.20.72")
                            .WithNugetDependency("Castle.Core", "5.1.1")
                            .WithNugetDependency("System.Threading.Tasks.Extensions", "4.5.4"),
                        _ => throw new Exception($"Unsupported Framework `{framework.Major}` for NuGet package '{MoqPackageName}'"),
                    }
                );
            NugetRegistry.Register(NSubstitutePackageName,
                (framework) => (framework.Major, framework.Minor) switch
                    {
                        ( >= 6, >= 0) => new PackageVersion("5.3.0")
                            .WithNugetDependency("Castle.Core", "5.1.1"),
                        ( >= 2, >= 0) => new PackageVersion("5.3.0")
                            .WithNugetDependency("Castle.Core", "5.1.1")
                            .WithNugetDependency("System.Threading.Tasks.Extensions", "4.3.0"),
                        _ => throw new Exception($"Unsupported Framework `{framework.Major}` for NuGet package '{NSubstitutePackageName}'"),
                    }
                );
            NugetRegistry.Register(XunitRunnerVisualstudioPackageName,
                (framework) => (framework.Major, framework.Minor) switch
                    {
                        ( >= 8, >= 0) => new PackageVersion("3.1.0"),
                        ( >= 6, >= 0) => new PackageVersion("3.0.2"),
                        _ => throw new Exception($"Unsupported Framework `{framework.Major}` for NuGet package '{XunitRunnerVisualstudioPackageName}'"),
                    }
                );
            NugetRegistry.Register(XunitV3PackageName,
                (framework) => (framework.Major, framework.Minor) switch
                    {
                        ( >= 8, >= 0) => new PackageVersion("2.0.2")
                            .WithNugetDependency("xunit.analyzers", "1.21.0")
                            .WithNugetDependency("xunit.v3.assert", "2.0.2")
                            .WithNugetDependency("xunit.v3.core", "2.0.2"),
                        ( >= 6, >= 0) => new PackageVersion("1.1.0")
                            .WithNugetDependency("xunit.analyzers", "1.20.0")
                            .WithNugetDependency("xunit.v3.assert", "1.1.0")
                            .WithNugetDependency("xunit.v3.core", "1.1.0"),
                        _ => throw new Exception($"Unsupported Framework `{framework.Major}` for NuGet package '{XunitV3PackageName}'"),
                    }
                );
        }

        public static NugetPackageInfo CoverletCollector(IOutputTarget outputTarget) => NugetRegistry.GetVersion(CoverletCollectorPackageName, outputTarget.GetMaxNetAppVersion());

        public static NugetPackageInfo MicrosoftNETTestSdk(IOutputTarget outputTarget) => NugetRegistry.GetVersion(MicrosoftNETTestSdkPackageName, outputTarget.GetMaxNetAppVersion());

        public static NugetPackageInfo Moq(IOutputTarget outputTarget) => NugetRegistry.GetVersion(MoqPackageName, outputTarget.GetMaxNetAppVersion());

        public static NugetPackageInfo NSubstitute(IOutputTarget outputTarget) => NugetRegistry.GetVersion(NSubstitutePackageName, outputTarget.GetMaxNetAppVersion());

        public static NugetPackageInfo XunitRunnerVisualstudio(IOutputTarget outputTarget) => NugetRegistry.GetVersion(XunitRunnerVisualstudioPackageName, outputTarget.GetMaxNetAppVersion());

        public static NugetPackageInfo XunitV3(IOutputTarget outputTarget) => NugetRegistry.GetVersion(XunitV3PackageName, outputTarget.GetMaxNetAppVersion());
    }
}