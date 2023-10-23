using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
using Intent.Modules.VisualStudio.Projects.FactoryExtensions.NuGet.HelperTypes;
using Intent.Modules.VisualStudio.Projects.FactoryExtensions.NuGet.SchemeProcessors;
using Intent.Modules.VisualStudio.Projects.Settings;
using Intent.Modules.VisualStudio.Projects.Tests.NuGet.Helpers;
using Xunit;

namespace Intent.Modules.VisualStudio.Projects.Tests.NuGet.SchemeTests
{
    public class UnsupportedSchemeTests
    {
        [Fact]
        public void GetInstalledPackagesReturnsEmptyCollection()
        {
            // Arrange
            var sut = new UnsupportedSchemeProcessor();
            var project = TestFixtureHelper.CreateProject(VisualStudioProjectScheme.Unsupported, TestVersion.Low, TestPackage.One, new Dictionary<string, string>());

            // Act
            var installedPackages = sut.GetInstalledPackages(
                solutionModelId: null,
                projectPath: project.FilePath,
                xNode: null);

            // Assert
            Assert.Empty(installedPackages);
        }

        [Fact]
        public void InstallPackageCreatesWarning()
        {
            // Arrange
            var sut = new UnsupportedSchemeProcessor();
            var tracing = new TestTracing();
            var project = TestFixtureHelper.CreateNuGetProject(
                scheme: VisualStudioProjectScheme.Unsupported,
                processor: sut,
                testVersion: TestVersion.Low,
                testPackage: TestPackage.One,
                nugetPackagesToInstall: new Dictionary<string, string>
                {
                    { "PackageToInstall.Id", "1.0.0" }
                });

            // Act
            sut.InstallPackages(
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
            Assert.Collection(tracing.DebugEntries,
                element => Assert.Contains(
                    "Skipped",
                    element));
        }

        [Fact]
        public void UpgradePackageCreatesWarning()
        {
            // Arrange
            var sut = new UnsupportedSchemeProcessor();
            var tracing = new TestTracing();
            var project = TestFixtureHelper.CreateNuGetProject(
                scheme: VisualStudioProjectScheme.Unsupported,
                processor: sut,
                testVersion: TestVersion.Low,
                testPackage: TestPackage.One,
                nugetPackagesToInstall: new Dictionary<string, string>
                {
                    { "TestPackage.One", "3.0.0" }
                });

            // Act
            sut.InstallPackages(
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
            Assert.Collection(tracing.DebugEntries,
                element => Assert.Contains(
                    "Skipped",
                    element));
        }
    }
}