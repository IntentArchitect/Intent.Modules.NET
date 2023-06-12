using System.Collections.Generic;
using Intent.Modules.Common.VisualStudio;
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
            var installedPackages = sut.GetInstalledPackages(project.FilePath, null);

            // Assert
            Assert.Empty(installedPackages);
        }

        [Fact]
        public void InstallPackageCreatesWarning()
        {
            // Arrange
            var sut = new UnsupportedSchemeProcessor();
            var tracing = new TestTracing();
            var project = TestFixtureHelper.CreateNuGetProject(VisualStudioProjectScheme.Unsupported, TestVersion.Low, TestPackage.One, nugetPackagesToInstall: new Dictionary<string, string>
            {
                { "PackageToInstall.Id", "1.0.0" }
            });

            // Act
            sut.InstallPackages(
                project.Content,
                project.RequestedPackages,
                project.InstalledPackages,
                project.Name,
                tracing,
                DependencyVersionOverwriteBehaviorOption.IfNewer);

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
            var project = TestFixtureHelper.CreateNuGetProject(VisualStudioProjectScheme.Unsupported, TestVersion.Low, TestPackage.One, nugetPackagesToInstall: new Dictionary<string, string>
            {
                { "TestPackage.One", "3.0.0" }
            });

            // Act
            sut.InstallPackages(
                project.Content,
                project.RequestedPackages,
                project.InstalledPackages,
                project.Name,
                tracing,
                DependencyVersionOverwriteBehaviorOption.IfNewer);

            // Assert
            Assert.Collection(tracing.DebugEntries,
                element => Assert.Contains(
                    "Skipped",
                    element));
        }
    }
}