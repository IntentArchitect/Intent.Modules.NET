using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;
using Intent.Metadata.Models;
using Intent.Modules.VisualStudio.Projects.FactoryExtensions.NuGet.HelperTypes;
using Intent.Modules.VisualStudio.Projects.Settings;
using Intent.Modules.VisualStudio.Projects.Tests.NuGet.Helpers;
using NuGet.Versioning;
using VerifyTests;
using VerifyXunit;
using Xunit;

namespace Intent.Modules.VisualStudio.Projects.Tests.NuGet.SchemeTests
{
    [UsesVerify]
    public class SdkSchemeTests
    {
        [Fact]
        public void GetsInstalledPackages_Default()
        {
            // Arrange
            var sut = TestFixtureHelper.CreateSdkProcessor();
            var project = TestFixtureHelper.CreateProject(VisualStudioProjectScheme.Sdk, TestVersion.Low, TestPackage.One, new Dictionary<string, string>());
            var doc = XDocument.Load(project.FilePath);

            // Act
            var installedPackages = sut.GetInstalledPackages(
                solutionModelId: string.Empty,
                projectPath: project.FilePath,
                xNode: doc);

            // Assert
            Assert.Collection(installedPackages, x =>
            {
                Assert.Equal("TestPackage.One", x.Key);
                Assert.Equal(x.Value.Version, VersionRange.Parse("1.0.0"));
            });
        }

        [Fact]
        public void GetsInstalledPackages_NestedVersion()
        {
            // Arrange
            var sut = TestFixtureHelper.CreateSdkProcessor();
            var project = TestFixtureHelper.CreateProject(VisualStudioProjectScheme.Sdk, TestVersion.Low, TestPackage.Three, new Dictionary<string, string>());
            var doc = XDocument.Load(project.FilePath);

            // Act
            var installedPackages = sut.GetInstalledPackages(
                solutionModelId: string.Empty,
                projectPath: project.FilePath,
                xNode: doc);

            // Assert
            Assert.Collection(installedPackages, x =>
            {
                Assert.Equal("TestPackage.Three", x.Key);
                Assert.Equal(x.Value.Version, VersionRange.Parse("1.0.0"));
            });
        }

        public static IEnumerable<object[]> InstallsPackages_TestData()
        {
            foreach (var depVerMan in Enum.GetValues<DependencyVersionOverwriteBehaviorOption>())
            {
                yield return new object[]
                {
                    "Default",
                    TestVersion.Low, TestPackage.One,
                    depVerMan,
                    new Dictionary<string, string>
                    {
                        { "PackageToInstall.Id", "1.0.0" }
                    }
                };

                yield return new object[]
                {
                    "Nested Install",
                    TestVersion.Low, TestPackage.Three,
                    depVerMan,
                    new Dictionary<string, string>
                    {
                        { "PackageToInstall.Id", "1.0.0" }
                    }
                };

                yield return new object[]
                {
                    "Upgrade",
                    TestVersion.Low, TestPackage.One,
                    depVerMan,
                    new Dictionary<string, string>
                    {
                        { "TestPackage.One", "3.0.0" }
                    }
                };

                yield return new object[]
                {
                    "Nested Upgrade",
                    TestVersion.Low, TestPackage.Three,
                    depVerMan,
                    new Dictionary<string, string>
                    {
                        { "TestPackage.Three", "3.0.0" }
                    }
                };
            }
        }

        [Theory]
        [MemberData(nameof(InstallsPackages_TestData))]
        public async Task InstallsPackages(
            string description,
            TestVersion testVersion,
            TestPackage testPackage,
            DependencyVersionOverwriteBehaviorOption depVerMan,
            Dictionary<string, string> nugetPackagesToInstall)
        {
            // Arrange
            var sut = TestFixtureHelper.CreateSdkProcessor();
            var tracing = new TestTracing();
            var project = TestFixtureHelper.CreateNuGetProject(
                scheme: VisualStudioProjectScheme.Sdk,
                processor: sut,
                testVersion: testVersion,
                testPackage: testPackage,
                nugetPackagesToInstall: nugetPackagesToInstall);

            // Act
            var result = sut.InstallPackages(
                solutionModelId: string.Empty,
                projectStereotypes: Enumerable.Empty<IStereotype>(),
                projectPath: null,
                projectContent: project.Content,
                requestedPackages: project.RequestedPackages,
                installedPackages: project.InstalledPackages,
                toRemovePackages: new List<string>(),
                projectName: project.Name,
                tracing: tracing,
                dependencyVersionOverwriteBehavior: depVerMan);

            // Assert
            var settings = new VerifySettings();
            settings.UseParameters(description, testVersion, testPackage, depVerMan);
            await Verifier.Verify(result, settings);
        }

        [Theory]
        [InlineData(TestPackage.One, TestPackage.Two)]
        [InlineData(TestPackage.Two, TestPackage.One)]
        public void SortsPackageReferencesInAlphabeticalOrder(TestPackage existingPackage, TestPackage testPackageToInstall)
        {
            // Arrange
            var sut = TestFixtureHelper.CreateSdkProcessor();
            var tracing = new TestTracing();
            var project = TestFixtureHelper.CreateNuGetProject(
                scheme: VisualStudioProjectScheme.Sdk,
                processor: sut,
                testVersion: TestVersion.Low,
                testPackage: existingPackage,
                nugetPackagesToInstall: new Dictionary<string, string>
                {
                    { $"{nameof(TestPackage)}.{testPackageToInstall}", "1.0.0" }
                });

            // Act
            var result = sut.InstallPackages(
                solutionModelId: string.Empty,
                projectStereotypes: Enumerable.Empty<IStereotype>(),
                projectPath: null,
                projectContent: project.Content,
                requestedPackages: project.RequestedPackages,
                installedPackages: project.InstalledPackages,
                toRemovePackages: new List<string>(),
                projectName: project.Name,
                tracing: tracing,
                dependencyVersionOverwriteBehavior: DependencyVersionOverwriteBehaviorOption.IfNewer);

            // Assert
            Assert.Equal(
                expected:
@"<Project Sdk=""Microsoft.NET.Sdk"">

  <ItemGroup>
    <PackageReference Include=""TestPackage.One"" Version=""1.0.0"" />
    <PackageReference Include=""TestPackage.Two"" Version=""1.0.0"" />
  </ItemGroup>

</Project>".ReplaceLineEndings(),
                actual: result.ReplaceLineEndings());
        }
    }
}
