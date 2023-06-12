using System.Collections.Generic;
using System.Xml.Linq;
using Intent.Modules.Common.VisualStudio;
using Intent.Modules.VisualStudio.Projects.FactoryExtensions.NuGet.HelperTypes;
using Intent.Modules.VisualStudio.Projects.FactoryExtensions.NuGet.SchemeProcessors;
using Intent.Modules.VisualStudio.Projects.Settings;
using Intent.Modules.VisualStudio.Projects.Tests.NuGet.Helpers;
using NuGet.Versioning;
using Xunit;

namespace Intent.Modules.VisualStudio.Projects.Tests.NuGet.SchemeTests
{
    public class NetFrameworkPackagesDotConfigSchemeTests
    {
        [Fact]
        public void GetsInstalledPackages()
        {
            // Arrange
            var sut = new NetFrameworkPackagesDotConfigSchemeProcessor();
            var project = TestFixtureHelper.CreateProject(VisualStudioProjectScheme.FrameworkWithPackagesDotConfig, TestVersion.Low, TestPackage.One, new Dictionary<string, string>());
            var doc = XDocument.Load(project.FilePath);

            // Act
            var installedPackages = sut.GetInstalledPackages(project.FilePath, doc);

            // Assert
            Assert.Collection(installedPackages, x =>
            {
                Assert.Equal("TestPackage.One", x.Key);
                Assert.Equal(x.Value.Version, VersionRange.Parse("1.0.0"));
            });
        }

        [Fact]
        public void InstallPackageCreatesWarning()
        {
            // Arrange
            var sut = new NetFrameworkPackagesDotConfigSchemeProcessor();
            var tracing = new TestTracing();
            var project = TestFixtureHelper.CreateNuGetProject(VisualStudioProjectScheme.FrameworkWithPackagesDotConfig, TestVersion.Low, TestPackage.One, nugetPackagesToInstall: new Dictionary<string, string>
            {
                { "PackageToInstall.Id", "1.0.0" }
            });

            // Act
            var result = sut.InstallPackages(
                project.Content,
                project.RequestedPackages,
                project.InstalledPackages,
                project.Name,
                tracing,
                DependencyVersionOverwriteBehaviorOption.IfNewer);

            // Assert
            Assert.Collection(tracing.WarningEntries,
                element => Assert.Contains(
                    "https://blog.nuget.org/20180409/migrate-packages-config-to-package-reference.html",
                    element));
        }

        [Fact]
        public void UpgradePackageCreatesWarning()
        {
            // Arrange
            var sut = new NetFrameworkPackagesDotConfigSchemeProcessor();
            var tracing = new TestTracing();
            var project = TestFixtureHelper.CreateNuGetProject(VisualStudioProjectScheme.FrameworkWithPackagesDotConfig, TestVersion.Low, TestPackage.One, nugetPackagesToInstall: new Dictionary<string, string>
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
            Assert.Collection(tracing.WarningEntries,
                element => Assert.Contains(
                    "https://blog.nuget.org/20180409/migrate-packages-config-to-package-reference.html",
                    element));
        }
    }
}