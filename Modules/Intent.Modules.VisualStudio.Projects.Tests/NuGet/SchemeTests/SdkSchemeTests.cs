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
using Shouldly;
using VerifyTests;
using VerifyXunit;
using Xunit;

namespace Intent.Modules.VisualStudio.Projects.Tests.NuGet.SchemeTests
{
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
                Assert.Equal("1.0.0", x.Value.VersionInfo.Version);
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
                Assert.Equal("1.0.0", x.Value.VersionInfo.Version );
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

        [Fact]
        public void ItShouldRespectIgnore()
        {
            // Arrange
            var sut = TestFixtureHelper.CreateSdkProcessor();
            var tracing = new TestTracing();
            var project = TestFixtureHelper.CreateNuGetProject(
                scheme: VisualStudioProjectScheme.Sdk,
                processor: sut,
                testVersion: TestVersion.Low,
                testPackage: TestPackage.One,
                nugetPackagesToInstall: new Dictionary<string, string>
                {
                    { "TestPackage", "2.0.0" }
                });

            // Act
            var result = sut.InstallPackages(
                solutionModelId: string.Empty,
                projectStereotypes: Enumerable.Empty<IStereotype>(),
                projectPath: null,
                projectContent:
                    """
                    <Project Sdk="Microsoft.NET.Sdk">

                      <ItemGroup>
                        <PackageReference Include="TestPackage" Version="1.0.0" IntentIgnore="true" />
                      </ItemGroup>

                    </Project>
                    """,
                requestedPackages: project.RequestedPackages,
                installedPackages: project.InstalledPackages,
                toRemovePackages: new List<string>(),
                projectName: project.Name,
                tracing: tracing,
                dependencyVersionOverwriteBehavior: DependencyVersionOverwriteBehaviorOption.IfNewer);

            // Assert
            result.ReplaceLineEndings().ShouldBe(
                """
                <Project Sdk="Microsoft.NET.Sdk">

                  <ItemGroup>
                    <PackageReference Include="TestPackage" Version="1.0.0" IntentIgnore="true" />
                  </ItemGroup>

                </Project>
                """.ReplaceLineEndings());
        }

        private const string CommentedSortedProject =
            """
            <Project Sdk="Microsoft.NET.Sdk">

              <ItemGroup>
                <!-- Identity -->
                <PackageReference Include="Azure.Identity" Version="1.13.0" /> <!-- managed identity only -->
                <!-- Pinned: 9.x breaks our custom converters, see #412 -->
                <PackageReference Include="Newtonsoft.Json" Version="13.0.3" />
                <PackageReference Include="Serilog" Version="4.0.0" /> <!-- keep in step with Serilog.Sinks.* -->
              </ItemGroup>

            </Project>
            """;

        [Fact]
        public void AlreadySortedProjectWithCommentsIsLeftUntouched()
        {
            var result = InstallInto(CommentedSortedProject, packagesToInstall: new Dictionary<string, string>());

            result.ShouldBe(CommentedSortedProject.ReplaceLineEndings());
        }

        [Theory]
        [InlineData("Aaa.First")]
        [InlineData("Microsoft.Extensions.Logging")]
        [InlineData("Newtonsoft.Json.Bson")]
        [InlineData("Zzz.Last")]
        public void InstallingIntoSortedProjectWithCommentsOnlyAddsTheNewLine(string packageId)
        {
            var result = InstallInto(CommentedSortedProject, packagesToInstall: new Dictionary<string, string>
            {
                { packageId, "1.0.0" }
            });

            var expectedLines = CommentedSortedProject.ReplaceLineEndings().Split(Environment.NewLine).ToList();
            var insertBefore = packageId switch
            {
                "Aaa.First" => "    <!-- Identity -->",
                "Microsoft.Extensions.Logging" => "    <!-- Pinned: 9.x breaks our custom converters, see #412 -->",
                "Newtonsoft.Json.Bson" => """    <PackageReference Include="Serilog" Version="4.0.0" /> <!-- keep in step with Serilog.Sinks.* -->""",
                _ => "  </ItemGroup>"
            };
            expectedLines.Insert(expectedLines.IndexOf(insertBefore), $"""    <PackageReference Include="{packageId}" Version="1.0.0" />""");

            result.Split(Environment.NewLine).ShouldBe(expectedLines);
        }

        [Theory]
        [InlineData("Aaa.One", "Aaa.Two", "    <!-- Identity -->")]
        [InlineData("Bbb.One", "Bbb.Two", "    <!-- Pinned: 9.x breaks our custom converters, see #412 -->")]
        [InlineData("Zzz.One", "Zzz.Two", "  </ItemGroup>")]
        [InlineData("Zzz.Two", "Zzz.One", "  </ItemGroup>")]
        public void InstallingSeveralPackagesIntoSortedProjectWithCommentsOnlyAddsTheNewLines(string firstPackageId, string secondPackageId, string insertBefore)
        {
            var result = InstallInto(CommentedSortedProject, packagesToInstall: new Dictionary<string, string>
            {
                { firstPackageId, "1.0.0" },
                { secondPackageId, "1.0.0" }
            });

            var expectedLines = CommentedSortedProject.ReplaceLineEndings().Split(Environment.NewLine).ToList();
            expectedLines.InsertRange(
                expectedLines.IndexOf(insertBefore),
                new[] { firstPackageId, secondPackageId }
                    .Order()
                    .Select(id => $"""    <PackageReference Include="{id}" Version="1.0.0" />"""));

            result.Split(Environment.NewLine).ShouldBe(expectedLines);
        }

        [Fact]
        public void InstallingIntoAMixedItemGroupDoesNotTakeOverASameLineComment()
        {
            var result = InstallInto(
                """
                <Project Sdk="Microsoft.NET.Sdk">

                  <ItemGroup>
                    <PackageReference Include="Azure.Identity" Version="1.13.0" /> <!-- managed identity only -->
                    <None Include="readme.md" />
                  </ItemGroup>

                </Project>
                """,
                packagesToInstall: new Dictionary<string, string>
                {
                    { "Zzz.Last", "1.0.0" }
                });

            var lines = result.Split(Environment.NewLine).Select(x => x.Trim()).ToArray();
            lines.ShouldContain("""<PackageReference Include="Azure.Identity" Version="1.13.0" /> <!-- managed identity only -->""");
            lines.ShouldContain("""<PackageReference Include="Zzz.Last" Version="1.0.0" />""");
            lines.ShouldContain("""<None Include="readme.md" />""");
        }

        [Fact]
        public void SortingAnUnsortedProjectKeepsCommentsBesideTheirPackages()
        {
            var result = InstallInto(
                """
                <Project Sdk="Microsoft.NET.Sdk">

                  <ItemGroup>
                    <PackageReference Include="Serilog" Version="4.0.0" /> <!-- keep in step with Serilog.Sinks.* -->
                    <!-- Pinned: 9.x breaks our custom converters, see #412 -->
                    <PackageReference Include="Newtonsoft.Json" Version="13.0.3" />
                    <PackageReference Include="Azure.Identity" Version="1.13.0" />
                    <!-- end of packages -->
                  </ItemGroup>

                </Project>
                """,
                packagesToInstall: new Dictionary<string, string>());

            result.ShouldBe(
                """
                <Project Sdk="Microsoft.NET.Sdk">

                  <ItemGroup>
                    <PackageReference Include="Azure.Identity" Version="1.13.0" />
                    <!-- Pinned: 9.x breaks our custom converters, see #412 -->
                    <PackageReference Include="Newtonsoft.Json" Version="13.0.3" />
                    <PackageReference Include="Serilog" Version="4.0.0" /> <!-- keep in step with Serilog.Sinks.* -->
                    <!-- end of packages -->
                  </ItemGroup>

                </Project>
                """.ReplaceLineEndings());
        }

        [Fact]
        public void SortingKeepsTheXmlDeclarationAndLineEndings()
        {
            var result = InstallInto(
                "<?xml version=\"1.0\" encoding=\"utf-8\"?>\r\n" +
                "<Project Sdk=\"Microsoft.NET.Sdk\">\r\n" +
                "  <ItemGroup>\r\n" +
                "    <PackageReference Include=\"B\" Version=\"1.0.0\" />\r\n" +
                "    <PackageReference Include=\"A\" Version=\"1.0.0\" />\r\n" +
                "  </ItemGroup>\r\n" +
                "</Project>",
                packagesToInstall: new Dictionary<string, string>(),
                normalizeLineEndings: false);

            result.ShouldBe(
                "<?xml version=\"1.0\" encoding=\"utf-8\"?>\r\n" +
                "<Project Sdk=\"Microsoft.NET.Sdk\">\r\n" +
                "  <ItemGroup>\r\n" +
                "    <PackageReference Include=\"A\" Version=\"1.0.0\" />\r\n" +
                "    <PackageReference Include=\"B\" Version=\"1.0.0\" />\r\n" +
                "  </ItemGroup>\r\n" +
                "</Project>");
        }

        [Fact]
        public void RemovingTheOnlyPackageInAGroupWithACommentKeepsTheGroupAndComment()
        {
            var result = InstallInto(
                """
                <Project Sdk="Microsoft.NET.Sdk">

                  <ItemGroup>
                    <PackageReference Include="Azure.Identity" Version="1.13.0" />
                  </ItemGroup>

                  <ItemGroup>
                    <!-- Pinned: 9.x breaks our custom converters, see #412 -->
                    <PackageReference Include="Newtonsoft.Json" Version="13.0.3" />
                  </ItemGroup>

                </Project>
                """,
                packagesToInstall: new Dictionary<string, string>(),
                toRemovePackages: ["Newtonsoft.Json"]);

            result.ShouldNotContain("Newtonsoft.Json\"");
            result.ShouldContain("<!-- Pinned: 9.x breaks our custom converters, see #412 -->");
            XDocument.Parse(result).Root!.Elements("ItemGroup").Count().ShouldBe(2);
        }

        [Fact]
        public void RemovingTheOnlyPackageInAGroupWithoutCommentsRemovesTheGroup()
        {
            var result = InstallInto(
                """
                <Project Sdk="Microsoft.NET.Sdk">

                  <ItemGroup>
                    <PackageReference Include="Azure.Identity" Version="1.13.0" />
                  </ItemGroup>

                  <ItemGroup>
                    <PackageReference Include="Newtonsoft.Json" Version="13.0.3" />
                  </ItemGroup>

                </Project>
                """,
                packagesToInstall: new Dictionary<string, string>(),
                toRemovePackages: ["Newtonsoft.Json"]);

            XDocument.Parse(result).Root!.Elements("ItemGroup").Count().ShouldBe(1);
        }

        private static string InstallInto(
            string projectContent,
            Dictionary<string, string> packagesToInstall,
            List<string> toRemovePackages = null,
            bool normalizeLineEndings = true)
        {
            var sut = TestFixtureHelper.CreateSdkProcessor();
            var project = TestFixtureHelper.CreateNuGetProject(
                scheme: VisualStudioProjectScheme.Sdk,
                processor: sut,
                testVersion: TestVersion.Low,
                testPackage: TestPackage.One,
                nugetPackagesToInstall: packagesToInstall);

            var result = sut.InstallPackages(
                solutionModelId: string.Empty,
                projectStereotypes: Enumerable.Empty<IStereotype>(),
                projectPath: null,
                projectContent: projectContent,
                requestedPackages: project.RequestedPackages,
                installedPackages: project.InstalledPackages,
                toRemovePackages: toRemovePackages ?? [],
                projectName: project.Name,
                tracing: new TestTracing(),
                dependencyVersionOverwriteBehavior: DependencyVersionOverwriteBehaviorOption.IfNewer);

            return normalizeLineEndings ? result.ReplaceLineEndings() : result;
        }
    }
}
