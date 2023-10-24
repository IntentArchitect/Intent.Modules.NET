using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Intent.Metadata.Models;
using Intent.Modules.VisualStudio.Projects.FactoryExtensions.NuGet.HelperTypes;
using Intent.Modules.VisualStudio.Projects.FactoryExtensions.NuGet.SchemeProcessors;
using Intent.Modules.VisualStudio.Projects.Settings;
using Intent.Modules.VisualStudio.Projects.Tests.NuGet.Helpers;
using NuGet.Versioning;
using Xunit;

namespace Intent.Modules.VisualStudio.Projects.Tests.NuGet.SchemeTests
{
    public class NetFrameworkPackageReferenceSchemeTests
    {
        [Fact]
        public void GetsInstalledPackages()
        {
            // Arrange
            var sut = new NetFrameworkPackageReferencesSchemeProcessor();
            var project = TestFixtureHelper.CreateProject(VisualStudioProjectScheme.FrameworkWithPackageReference, TestVersion.Low, TestPackage.One, new Dictionary<string, string>());
            var doc = XDocument.Load(project.FilePath);

            // Act
            var installedPackages = sut.GetInstalledPackages(
                solutionModelId: null,
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
        public void InstallsPackage()
        {
            // Arrange
            var sut = new NetFrameworkPackageReferencesSchemeProcessor();
            var tracing = new TestTracing();
            var project = TestFixtureHelper.CreateNuGetProject(
                scheme: VisualStudioProjectScheme.FrameworkWithPackageReference,
                processor: sut,
                testVersion: TestVersion.Low,
                testPackage: TestPackage.One,
                nugetPackagesToInstall: new Dictionary<string, string>
                {
                    { "PackageToInstall.Id", "1.0.0" }
                });

            // Act
            var result = sut.InstallPackages(
                solutionModelId: null,
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
@"<?xml version=""1.0"" encoding=""utf-8""?>
<Project xmlns=""http://schemas.microsoft.com/developer/msbuild/2003"">
  <ItemGroup>
    <PackageReference Include=""PackageToInstall.Id"">
      <Version>1.0.0</Version>
    </PackageReference>
    <PackageReference Include=""TestPackage.One"">
      <Version>1.0.0</Version>
    </PackageReference>
  </ItemGroup>
</Project>",
                actual: result);
        }

        [Fact]
        public void UpgradesPackage()
        {
            // Arrange
            var sut = new NetFrameworkPackageReferencesSchemeProcessor();
            var tracing = new TestTracing();
            var project = TestFixtureHelper.CreateNuGetProject(
                scheme: VisualStudioProjectScheme.FrameworkWithPackageReference,
                processor: sut,
                testVersion: TestVersion.Low,
                testPackage: TestPackage.One,
                nugetPackagesToInstall: new Dictionary<string, string>
                {
                    { "TestPackage.One", "3.0.0" }
                });

            // Act
            var result = sut.InstallPackages(
                solutionModelId: null,
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
@"<?xml version=""1.0"" encoding=""utf-8""?>
<Project xmlns=""http://schemas.microsoft.com/developer/msbuild/2003"">
  <ItemGroup>
    <PackageReference Include=""TestPackage.One"">
      <Version>3.0.0</Version>
    </PackageReference>
  </ItemGroup>
</Project>",
                actual: result);
        }

        [Theory]
        [InlineData(TestPackage.One, TestPackage.Two)]
        [InlineData(TestPackage.Two, TestPackage.One)]
        public void SortsPackageReferencesInAlphabeticalOrder(TestPackage existingPackage, TestPackage testPackageToInstall)
        {
            // Arrange
            var sut = new NetFrameworkPackageReferencesSchemeProcessor();
            var tracing = new TestTracing();
            var project = TestFixtureHelper.CreateNuGetProject(
                scheme: VisualStudioProjectScheme.FrameworkWithPackageReference,
                processor: sut,
                testVersion: TestVersion.Low,
                testPackage: existingPackage,
                nugetPackagesToInstall: new Dictionary<string, string>
                {
                    { $"{nameof(TestPackage)}.{testPackageToInstall}", "1.0.0" }
                });

            // Act
            var result = sut.InstallPackages(
                solutionModelId: null,
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
@"<?xml version=""1.0"" encoding=""utf-8""?>
<Project xmlns=""http://schemas.microsoft.com/developer/msbuild/2003"">
  <ItemGroup>
    <PackageReference Include=""TestPackage.One"">
      <Version>1.0.0</Version>
    </PackageReference>
    <PackageReference Include=""TestPackage.Two"">
      <Version>1.0.0</Version>
    </PackageReference>
  </ItemGroup>
</Project>",
                actual: result);
        }
    }
}
