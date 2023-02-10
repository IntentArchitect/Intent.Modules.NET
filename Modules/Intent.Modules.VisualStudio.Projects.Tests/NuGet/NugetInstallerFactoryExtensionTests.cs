using System.Collections.Generic;
using System.Xml.Linq;
using Intent.Modules.VisualStudio.Projects.FactoryExtensions;
using Intent.Modules.VisualStudio.Projects.FactoryExtensions.NuGet.HelperTypes;
using Intent.Modules.VisualStudio.Projects.Tests.NuGet.Helpers;
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
        public void ConsolidatationNotTriggeredWhenVersionsAreTheSame()
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
            sut.Execute(projects, tracing, (filePath, content) => { });

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
            sut.Execute(projects, tracing, (path, content) => saved.Add((path, content)));

            // Assert
            Assert.Collection(saved, nuGetProject =>
            {
                Assert.Equal(project2.FilePath, nuGetProject.path);
                Assert.Equal(
                    XDocument.Parse(
@"<Project Sdk=""Microsoft.NET.Sdk"">

  <ItemGroup>
    <PackageReference Include=""TestPackage.One"" Version=""2.0.0"" />
  </ItemGroup>

</Project>", LoadOptions.PreserveWhitespace).ToString(),
                    nuGetProject.content);
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
            sut.Execute(projects, tracing, (path, content) => saved.Add((path, content)));

            // Assert
            Assert.Collection(saved, nuGetProject =>
            {
                Assert.Equal(project2.FilePath, nuGetProject.path);
                Assert.Equal(
                    XDocument.Parse(
@"<Project Sdk=""Microsoft.NET.Sdk"">

  <ItemGroup>
    <PackageReference Include=""TestPackage.One"" Version=""2.0.0"" />
    <PackageReference Include=""TestPackage.Two"" Version=""2.0.0"" />
  </ItemGroup>

</Project>", LoadOptions.PreserveWhitespace).ToString(),
                    nuGetProject.content);
            });
        }
    }
}
