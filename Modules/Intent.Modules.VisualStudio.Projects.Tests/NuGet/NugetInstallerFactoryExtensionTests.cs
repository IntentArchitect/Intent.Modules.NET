using System;
using System.Collections.Generic;
using System.Xml.Linq;
using Intent.Engine;
using Intent.Metadata.Models;
using Intent.Modules.Common.VisualStudio;
using Intent.Modules.VisualStudio.Projects.FactoryExtensions;
using Intent.Modules.VisualStudio.Projects.FactoryExtensions.NuGet.HelperTypes;
using Intent.Modules.VisualStudio.Projects.Settings;
using Intent.Modules.VisualStudio.Projects.Tests.NuGet.Helpers;
using NSubstitute;
using NuGet.Versioning;
using Shouldly;
using Xunit;

namespace Intent.Modules.VisualStudio.Projects.Tests.NuGet
{
    public class NugetInstallerFactoryExtensionTests
    {
        [Theory]
        [InlineData(VisualStudioProjectScheme.Sdk)]
        [InlineData(VisualStudioProjectScheme.FrameworkWithPackageReference)]
        [InlineData(VisualStudioProjectScheme.FrameworkWithPackagesDotConfig)]
        public void ResolvesCorrectly(object untypedNuGetScheme)
        {
            var nuGetScheme = (VisualStudioProjectScheme)untypedNuGetScheme;

            // Arrange
            var project = TestFixtureHelper.CreateProject(nuGetScheme, TestVersion.High, TestPackage.One, new Dictionary<string, string>());
            var document = XDocument.Load(project.FilePath);

            // Act
            var result = document.ResolveProjectScheme();

            // Assert
            Assert.Equal(nuGetScheme, result);
        }

        [Fact]
        public void ConsolidationNotTriggeredWhenVersionsAreTheSame()
        {
            // Arrange
            var tracing = new TestTracing();
            var sut = TestFixtureHelper.GetNuGetInstaller(true, false);
            var projects = new[]
            {
                TestFixtureHelper.CreateProject(VisualStudioProjectScheme.Sdk, TestVersion.High, TestPackage.One, new Dictionary<string, string>()),
                TestFixtureHelper.CreateProject(VisualStudioProjectScheme.Sdk, TestVersion.High, TestPackage.One, new Dictionary<string, string>()),
            };

            // Act
            sut.Execute(
                nuGetProjectSchemeProcessors: new Dictionary<VisualStudioProjectScheme, INuGetSchemeProcessor>
                {
                    [VisualStudioProjectScheme.Sdk] = TestFixtureHelper.CreateSdkProcessor()
                },
                applicationProjects: projects,
                tracing: tracing,
                saveProjectDelegate: (filePath, content) => { },
                dependencyVersionOverwriteBehavior: DependencyVersionOverwriteBehaviorOption.IfNewer);

            // Assert
            Assert.Empty(tracing.InfoEntries);
        }

        [Fact]
        public void ConsolidatesInstalledPackageVersions()
        {
            // Arrange
            var tracing = new TestTracing();
            var sut = TestFixtureHelper.GetNuGetInstaller(true, false);
            var saved = new List<(string path, string content)>();
            var project1 = TestFixtureHelper.CreateProject(VisualStudioProjectScheme.Sdk, TestVersion.High, TestPackage.One, new Dictionary<string, string>());
            var project2 = TestFixtureHelper.CreateProject(VisualStudioProjectScheme.Sdk, TestVersion.Low, TestPackage.One, new Dictionary<string, string>());
            var projects = new[] { project1, project2 };

            // Act
            sut.Execute(
                nuGetProjectSchemeProcessors: new Dictionary<VisualStudioProjectScheme, INuGetSchemeProcessor>
                {
                    [VisualStudioProjectScheme.Sdk] = TestFixtureHelper.CreateSdkProcessor()
                },
                applicationProjects: projects,
                tracing: tracing,
                saveProjectDelegate: (path, content) => saved.Add((path, content)),
                dependencyVersionOverwriteBehavior: DependencyVersionOverwriteBehaviorOption.IfNewer);

            // Assert
            Assert.Collection(saved, nuGetProject =>
            {
                Assert.Equal(project2.FilePath, nuGetProject.path);
                    nuGetProject.content.ReplaceLineEndings().ShouldBe(
                    XDocument.Parse(
                        @"<Project Sdk=""Microsoft.NET.Sdk"">

  <ItemGroup>
    <PackageReference Include=""TestPackage.One"" Version=""2.0.0"" />
  </ItemGroup>

</Project>", LoadOptions.PreserveWhitespace).ToString().ReplaceLineEndings());
            });
        }

        [Fact]
        public void ConsolidateInstallsVersionsToHighestExisting()
        {
            // Arrange
            var tracing = new TestTracing();
            var sut = TestFixtureHelper.GetNuGetInstaller(true, false);
            var saved = new List<(string path, string content)>();
            var project1 = TestFixtureHelper.CreateProject(VisualStudioProjectScheme.Sdk, TestVersion.High, TestPackage.One, new Dictionary<string, string>());
            var project2 = TestFixtureHelper.CreateProject(VisualStudioProjectScheme.Sdk, TestVersion.High, TestPackage.Two, new Dictionary<string, string>
            {
                { "TestPackage.One", "1.0.0" }
            });
            var projects = new[] { project1, project2 };

            // Act
            sut.Execute(applicationProjects: projects,
                nuGetProjectSchemeProcessors: new Dictionary<VisualStudioProjectScheme, INuGetSchemeProcessor>
                {
                    [VisualStudioProjectScheme.Sdk] = TestFixtureHelper.CreateSdkProcessor()
                },
                tracing: tracing,
                saveProjectDelegate: (path, content) =>
                {
                    saved.Add((path, content));
                },
                dependencyVersionOverwriteBehavior: DependencyVersionOverwriteBehaviorOption.IfNewer);

            // Assert
            Assert.Collection(saved, nuGetProject =>
            {
                Assert.Equal(project2.FilePath, nuGetProject.path);
                nuGetProject.content.ReplaceLineEndings().ShouldBe(XDocument.Parse(
@"<Project Sdk=""Microsoft.NET.Sdk"">

  <ItemGroup>
    <PackageReference Include=""TestPackage.One"" Version=""2.0.0"" />
    <PackageReference Include=""TestPackage.Two"" Version=""2.0.0"" />
  </ItemGroup>

</Project>", LoadOptions.PreserveWhitespace).ToString().ReplaceLineEndings());
            });

        }

        [Fact]
        public void NuGetPackageConsolidationTest_RemoveTransativeDependencies()
        {
            // Arrange
            var tracing = new TestTracing();
            var sut = TestFixtureHelper.GetNuGetInstaller(true, false);

            var testCase = TestFixtureHelper.GetCleanArchitectureProjectSetup();

            testCase.Api.RequestedPackages.AddNuGet("RoslynWeaver", "2.1.4");

            testCase.Application.RequestedPackages.AddNuGet("RoslynWeaver", "2.1.4");

            testCase.Domain.RequestedPackages.AddNuGet("RoslynWeaver", "2.1.4");

            testCase.Infrastructure.RequestedPackages.AddNuGet("RoslynWeaver", "2.1.4");

            testCase.Test.RequestedPackages.AddNuGet("RoslynWeaver", "2.1.4");


            // Act
            sut.ConsolidateRequestedPackages(testCase.Projects());

            // Assert
            Assert.True(testCase.Domain.RequestedPackages.ContainsKey("RoslynWeaver"));
            Assert.Equal("2.1.4", testCase.Domain.RequestedPackages["RoslynWeaver"].Version.MinVersion.ToString());

            Assert.False(testCase.Api.RequestedPackages.ContainsKey("RoslynWeaver"));
            Assert.False(testCase.Application.RequestedPackages.ContainsKey("RoslynWeaver"));
            Assert.False(testCase.Infrastructure.RequestedPackages.ContainsKey("RoslynWeaver"));
            Assert.False(testCase.Test.RequestedPackages.ContainsKey("RoslynWeaver"));
        }

        [Fact]
        public void NuGetPackageConsolidationTest_ResolveHighestVersion()
        {
            // Arrange
            var tracing = new TestTracing();
            var sut = TestFixtureHelper.GetNuGetInstaller(true, false);

            var testCase = TestFixtureHelper.GetCleanArchitectureProjectSetup();

            testCase.Api.RequestedPackages.AddNuGet("RoslynWeaver", "2.1.5");

            testCase.Application.RequestedPackages.AddNuGet("RoslynWeaver", "2.1.4");

            testCase.Domain.RequestedPackages.AddNuGet("RoslynWeaver", "2.1.4");

            testCase.Infrastructure.RequestedPackages.AddNuGet("RoslynWeaver", "2.1.4");

            testCase.Test.RequestedPackages.AddNuGet("RoslynWeaver", "2.1.4");


            // Act
            sut.ConsolidateRequestedPackages(testCase.Projects());

            // Assert
            Assert.True(testCase.Domain.RequestedPackages.ContainsKey("RoslynWeaver"));
            Assert.Equal("2.1.5", testCase.Domain.RequestedPackages["RoslynWeaver"].Version.MinVersion.ToString());
            Assert.False(testCase.Api.RequestedPackages.ContainsKey("RoslynWeaver"));
            Assert.False(testCase.Application.RequestedPackages.ContainsKey("RoslynWeaver"));
            Assert.False(testCase.Infrastructure.RequestedPackages.ContainsKey("RoslynWeaver"));
            Assert.False(testCase.Test.RequestedPackages.ContainsKey("RoslynWeaver"));
        }

        [Fact]
        public void NuGetPackageConsolidationTest_RequestUpgradeOfAlreadyInstalledTransitivePackages()
        {
            // Arrange
            var tracing = new TestTracing();
            var sut = TestFixtureHelper.GetNuGetInstaller(true, false);

            var testCase = TestFixtureHelper.GetCleanArchitectureProjectSetup();

            //Older version already installed
            testCase.Api.InstalledPackages.AddNuGet("RoslynWeaver", "2.1.4");
            testCase.Api.RequestedPackages.AddNuGet("RoslynWeaver", "2.1.4");

            testCase.Application.RequestedPackages.AddNuGet("RoslynWeaver", "2.1.4");

            testCase.Domain.RequestedPackages.AddNuGet("RoslynWeaver", "2.1.5");

            testCase.Infrastructure.RequestedPackages.AddNuGet("RoslynWeaver", "2.1.4");

            testCase.Test.RequestedPackages.AddNuGet("RoslynWeaver", "2.1.4");

            // Act
            sut.ConsolidateRequestedPackages(testCase.Projects());

            // Assert
            Assert.True(testCase.Domain.RequestedPackages.ContainsKey("RoslynWeaver"));
            Assert.Equal("2.1.5", testCase.Domain.RequestedPackages["RoslynWeaver"].Version.MinVersion.ToString());
            Assert.True(testCase.Api.RequestedPackages.ContainsKey("RoslynWeaver"));
            Assert.Equal("2.1.5", testCase.Api.RequestedPackages["RoslynWeaver"].Version.MinVersion.ToString());

            Assert.False(testCase.Application.RequestedPackages.ContainsKey("RoslynWeaver"));
            Assert.False(testCase.Infrastructure.RequestedPackages.ContainsKey("RoslynWeaver"));
            Assert.False(testCase.Test.RequestedPackages.ContainsKey("RoslynWeaver"));
        }

        [Fact]
        public void NuGetPackageConsolidationTest_RemoveTransitiveImplicitDependencies()
        {
            // Arrange
            var tracing = new TestTracing();
            var sut = TestFixtureHelper.GetNuGetInstaller(true, false);

            var testCase = TestFixtureHelper.GetCleanArchitectureProjectSetup();

            testCase.Api.RequestedPackages.AddNuGet("Common", "1.0.0");

            testCase.Domain.RequestedPackages.AddNuGet("Common.CSharp", "2.0.0", c => 
            {
                c.Dependencies.Add(new NugetPackageDependency("Common", "1.0.0"));
            });

            // Act
            sut.ConsolidateRequestedPackages(testCase.Projects());

            // Assert
            Assert.True(testCase.Domain.RequestedPackages.ContainsKey("Common.CSharp"));
            Assert.Equal("2.0.0", testCase.Domain.RequestedPackages["Common.CSharp"].Version.MinVersion.ToString());
            Assert.False(testCase.Domain.RequestedPackages.ContainsKey("Common"));
            Assert.False(testCase.Api.RequestedPackages.ContainsKey("Common"));
        }

        [Fact]
        public void NuGetPackageConsolidationTest_KeepInstalledTransitiveImplicitDependencyAndUpgrade()
        {
            // Arrange
            var tracing = new TestTracing();
            var sut = TestFixtureHelper.GetNuGetInstaller(true, false);

            var testCase = TestFixtureHelper.GetCleanArchitectureProjectSetup();

            testCase.Api.RequestedPackages.AddNuGet("Common", "1.0.0");
            testCase.Api.InstalledPackages.AddNuGet("Common", "1.0.0");

            testCase.Domain.RequestedPackages.AddNuGet("Common.CSharp", "2.0.0", c =>
            {
                c.Dependencies.Add(new NugetPackageDependency("Common", "1.5.0"));
            });

            // Act
            sut.ConsolidateRequestedPackages(testCase.Projects());

            // Assert
            Assert.True(testCase.Domain.RequestedPackages.ContainsKey("Common.CSharp"));
            Assert.Equal("2.0.0", testCase.Domain.RequestedPackages["Common.CSharp"].Version.MinVersion.ToString());
            Assert.False(testCase.Domain.RequestedPackages.ContainsKey("Common"));
            Assert.True(testCase.Api.RequestedPackages.ContainsKey("Common"));
            Assert.Equal("1.5.0", testCase.Api.RequestedPackages["Common"].Version.MinVersion.ToString());
        }

        [Fact]
        public void NuGetPackageConsolidationTest_RequestNewerVersionOfImplicitDependency()
        {
            // Arrange
            var tracing = new TestTracing();
            var sut = TestFixtureHelper.GetNuGetInstaller(true, false);

            var testCase = TestFixtureHelper.GetCleanArchitectureProjectSetup();

            testCase.Api.RequestedPackages.AddNuGet("Common", "1.5.0");

            testCase.Domain.RequestedPackages.AddNuGet("Common.CSharp", "2.0.0", c =>
            {
                c.Dependencies.Add(new NugetPackageDependency("Common", "1.0.0"));
            });

            // Act
            sut.ConsolidateRequestedPackages(testCase.Projects());

            // Assert
            Assert.True(testCase.Domain.RequestedPackages.ContainsKey("Common.CSharp"));
            Assert.Equal("2.0.0", testCase.Domain.RequestedPackages["Common.CSharp"].Version.MinVersion.ToString());
            Assert.False(testCase.Domain.RequestedPackages.ContainsKey("Common"));
            Assert.True(testCase.Api.RequestedPackages.ContainsKey("Common"));
            Assert.Equal("1.5.0", testCase.Api.RequestedPackages["Common"].Version.MinVersion.ToString());
        }

        [Fact]
        public void NuGetPackageConsolidationTest_RequestNewerVersionOfImplicitDependencyWithTransitive()
        {
            // Arrange
            var tracing = new TestTracing();
            var sut = TestFixtureHelper.GetNuGetInstaller(true, false);

            var testCase = TestFixtureHelper.GetCleanArchitectureProjectSetup();

            testCase.Api.RequestedPackages.AddNuGet("Common", "1.5.0");
            testCase.Application.RequestedPackages.AddNuGet("Common", "1.5.0");

            testCase.Domain.RequestedPackages.AddNuGet("Common.CSharp", "2.0.0", c =>
            {
                c.Dependencies.Add(new NugetPackageDependency("Common", "1.0.0"));
            });

            // Act
            sut.ConsolidateRequestedPackages(testCase.Projects());

            // Assert
            Assert.True(testCase.Domain.RequestedPackages.ContainsKey("Common.CSharp"));
            Assert.Equal("2.0.0", testCase.Domain.RequestedPackages["Common.CSharp"].Version.MinVersion.ToString());
            Assert.False(testCase.Domain.RequestedPackages.ContainsKey("Common"));
            Assert.False(testCase.Api.RequestedPackages.ContainsKey("Common"));
            Assert.True(testCase.Application.RequestedPackages.ContainsKey("Common"));
            Assert.Equal("1.5.0", testCase.Application.RequestedPackages["Common"].Version.MinVersion.ToString());
        }

        [Fact]
        public void NuGetPackageConsolidationTest_RequestNewerVersionOfImplicitDependencyWithTransitiveFallBackOnDependants()
        {
            // Arrange
            var tracing = new TestTracing();
            var sut = TestFixtureHelper.GetNuGetInstaller(true, false);

            var testCase = TestFixtureHelper.GetCleanArchitectureProjectSetup();

            testCase.Api.RequestedPackages.AddNuGet("Common", "1.0.0");
            testCase.Application.RequestedPackages.AddNuGet("Common", "1.0.0");

            testCase.Domain.InstalledPackages.AddNuGet("Common.CSharp", "2.0.1");
            //Will fall back on the below request for "guessing" dependencies
            testCase.Domain.RequestedPackages.AddNuGet("Common.CSharp", "2.0.0", c =>
            {
                c.Dependencies.Add(new NugetPackageDependency("Common", "1.0.0"));
            });

            // Act
            sut.ConsolidateRequestedPackages(testCase.Projects());

            // Assert
            Assert.True(testCase.Domain.RequestedPackages.ContainsKey("Common.CSharp"));
            Assert.Equal("2.0.0", testCase.Domain.RequestedPackages["Common.CSharp"].Version.MinVersion.ToString());
            Assert.False(testCase.Domain.RequestedPackages.ContainsKey("Common"));
            Assert.False(testCase.Api.RequestedPackages.ContainsKey("Common"));
            Assert.False(testCase.Application.RequestedPackages.ContainsKey("Common"));
        }

        [Fact]
        public void NuGetPackageConsolidationTest_RequestNewerVersionOfImplicitDependencyWithTransitiveFallBackOnDependantsWithUpgrade()
        {
            // Arrange
            var tracing = new TestTracing();
            var sut = TestFixtureHelper.GetNuGetInstaller(true, false);

            var testCase = TestFixtureHelper.GetCleanArchitectureProjectSetup();

            testCase.Api.RequestedPackages.AddNuGet("Common", "1.5.0");
            testCase.Application.RequestedPackages.AddNuGet("Common", "1.0.0");

            testCase.Domain.InstalledPackages.AddNuGet("Common.CSharp", "2.0.1");
            //Will fall back on the below request for "guessing" dependencies
            testCase.Domain.RequestedPackages.AddNuGet("Common.CSharp", "2.0.0", c =>
            {
                c.Dependencies.Add(new NugetPackageDependency("Common", "1.0.0"));
            });

            // Act
            sut.ConsolidateRequestedPackages(testCase.Projects());

            // Assert

            Assert.True(testCase.Domain.RequestedPackages.ContainsKey("Common.CSharp"));
            Assert.Equal("2.0.0", testCase.Domain.RequestedPackages["Common.CSharp"].Version.MinVersion.ToString());
            Assert.False(testCase.Domain.RequestedPackages.ContainsKey("Common"));
            Assert.True(testCase.Application.RequestedPackages.ContainsKey("Common"));
            Assert.Equal("1.5.0", testCase.Application.RequestedPackages["Common"].Version.MinVersion.ToString());
            Assert.False(testCase.Api.RequestedPackages.ContainsKey("Common"));
        }

        [Fact]
        public void NuGetPackageConsolidationTest_SameProjectDependant()
        {
            // Arrange
            var tracing = new TestTracing();
            var sut = TestFixtureHelper.GetNuGetInstaller(true, false);

            var testCase = TestFixtureHelper.GetCleanArchitectureProjectSetup();


            testCase.Domain.RequestedPackages.AddNuGet("Common", "1.0.0");
            //Will fall back on the below request for "guessing" dependencies
            testCase.Domain.RequestedPackages.AddNuGet("Common.CSharp", "2.0.0", c =>
            {
                c.Dependencies.Add(new NugetPackageDependency("Common", "1.0.0"));
                c.Dependencies.Add(new NugetPackageDependency("Common2", "1.0.0"));
            });
            testCase.Domain.RequestedPackages.AddNuGet("Common2", "1.5.0");

            // Act
            sut.ConsolidateRequestedPackages(testCase.Projects());

            // Assert

            Assert.True(testCase.Domain.RequestedPackages.ContainsKey("Common.CSharp"));
            Assert.Equal("2.0.0", testCase.Domain.RequestedPackages["Common.CSharp"].Version.MinVersion.ToString());
            Assert.True(testCase.Domain.RequestedPackages.ContainsKey("Common2"));
            Assert.Equal("1.5.0", testCase.Domain.RequestedPackages["Common2"].Version.MinVersion.ToString());
            Assert.False(testCase.Domain.RequestedPackages.ContainsKey("Common"));
        }

    }

    internal static class NugetHelperExtensions
    {
        internal static void AddNuGet(this Dictionary<string, NuGetPackage> dictionary, string packageName, string version, Action<NugetPackageInfo> configurePackge = null)
        {
            var package = new NugetPackageInfo(packageName, version);
            if (configurePackge != null)
            {
                configurePackge(package);
            }
            dictionary.Add(packageName, NuGetPackage.Create("", package));
        }

    }
}
