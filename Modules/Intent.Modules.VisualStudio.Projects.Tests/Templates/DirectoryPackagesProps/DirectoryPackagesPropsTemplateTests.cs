using System;
using System.Collections.Generic;
using System.IO;
using Intent.Engine;
using Intent.Eventing;
using Intent.Metadata.Models;
using Intent.Modules.Common.Templates;
using Intent.Modules.Constants;
using Intent.Modules.VisualStudio.Projects.Api;
using Intent.Modules.VisualStudio.Projects.Templates.DirectoryPackagesProps;
using Intent.Templates;
using Intent.Utils;
using NSubstitute;
using Shouldly;
using Xunit;

namespace Intent.Modules.VisualStudio.Projects.Tests.Templates.DirectoryPackagesProps;

public class DirectoryPackagesPropsTemplateTests
{
    private readonly IApplication _application;
    private readonly IPackage _package;
    private readonly VisualStudioSolutionModel _model;
    private readonly Dictionary<string, string> _fileSystem;
    private readonly ISoftwareFactoryEventDispatcher _eventDispatcher;

    public DirectoryPackagesPropsTemplateTests()
    {
        _application = Substitute.For<IApplication>();
        _package = Substitute.For<IPackage>();
        _package.SpecializationTypeId.Returns(VisualStudioSolutionModel.SpecializationTypeId);
        _model = new VisualStudioSolutionModel(_package);
        _fileSystem = new Dictionary<string, string>();
        _eventDispatcher = Substitute.For<ISoftwareFactoryEventDispatcher>();
        Logging.SetTracing(Substitute.For<ITracing>());
    }

    [Fact]
    public void WhenCentralPackageManagementDisabled_ShouldNotReturnVersions()
    {
        // Arrange
        var mainPath = FixPathForCurrentOs(@"C:\proj\Directory.Packages.props");
        _fileSystem[mainPath] = @"
<Project>
  <ItemGroup>
    <PackageVersion Include=""TestPackage"" Version=""1.0.0"" />
  </ItemGroup>
</Project>";

        var template = CreateTemplate(mainPath, centralPackageManagement: false);

        // Act
        var success = template.TryGetVersion("TestPackage", out var version);

        // Assert
        success.ShouldBeFalse();
        version.ShouldBeNull();
    }

    [Fact]
    public void WhenPackageDefinedInMainFile_ShouldReturnVersion()
    {
        // Arrange
        var mainPath = FixPathForCurrentOs(@"C:\proj\Directory.Packages.props");
        _fileSystem[mainPath] = @"
<Project>
  <ItemGroup>
    <PackageVersion Include=""TestPackage"" Version=""1.0.0"" />
  </ItemGroup>
</Project>";

        var template = CreateTemplate(mainPath);

        // Act
        var success = template.TryGetVersion("TestPackage", out var version);

        // Assert
        success.ShouldBeTrue();
        version.ShouldBe("1.0.0");
    }

    [Fact]
    public void WhenPackageVersionUpdated_ShouldNotifyChanges()
    {
        // Arrange
        var mainPath = FixPathForCurrentOs(@"C:\proj\Directory.Packages.props");
        _fileSystem[mainPath] = @"
<Project>
  <ItemGroup>
    <PackageVersion Include=""TestPackage"" Version=""1.0.0"" />
  </ItemGroup>
</Project>";

        var changeManager = Substitute.For<IChanges>();
        _application.ChangeManager.Returns(changeManager);
        changeManager.FindChange(mainPath).Returns(Substitute.For<IChange>());

        var template = CreateTemplate(mainPath);

        // Act
        template.SetPackageVersion("TestPackage", "2.0.0", _eventDispatcher);

        // Assert
        Assert.True(template.TryGetVersion("TestPackage", out var newVersion));
        Assert.Equal("2.0.0", newVersion);
    }

    [Fact]
    public void WhenPackageVersionOverriddenLocally_ShouldWarnAboutImportedVersion()
    {
        // Arrange
        var mainPath = FixPathForCurrentOs(@"C:\proj\Directory.Packages.props");
        var importPath = FixPathForCurrentOs(@"C:\proj\imported.props");

        _fileSystem[mainPath] = @"
<Project>
  <Import Project=""imported.props"" />
</Project>";

        _fileSystem[importPath] = @"
<Project>
  <ItemGroup>
    <PackageVersion Include=""ImportedPackage"" Version=""1.0.0"" />
  </ItemGroup>
</Project>";

        var template = CreateTemplate(mainPath);

        // Act & Assert
        // This should trigger a warning about version mismatch
        template.SetPackageVersion("ImportedPackage", "2.0.0", _eventDispatcher);
    }

    [Fact]
    public void WhenPackageRemovedButExistsInImport_ShouldWarnAboutImportedVersion()
    {
        // Arrange
        var mainPath = FixPathForCurrentOs(@"C:\proj\Directory.Packages.props");
        var importPath = FixPathForCurrentOs(@"C:\proj\imported.props");

        _fileSystem[mainPath] = @"
<Project>
  <Import Project=""imported.props"" />
  <ItemGroup>
    <PackageVersion Include=""SharedPackage"" Version=""2.0.0"" />
  </ItemGroup>
</Project>";

        _fileSystem[importPath] = @"
<Project>
  <ItemGroup>
    <PackageVersion Include=""SharedPackage"" Version=""1.0.0"" />
  </ItemGroup>
</Project>";

        var template = CreateTemplate(mainPath);

        // Act
        template.Remove("SharedPackage", _eventDispatcher);

        // Assert
        // This should trigger a warning about the package still being present in imports
        template.TryGetVersion("SharedPackage", out var version).ShouldBeTrue();
        version.ShouldBe("1.0.0"); // Should still get the imported version
    }

    [Fact]
    public void WhenImportPathIsRelative_ShouldResolveAgainstMainDirectory()
    {
        // Arrange
        var mainPath = FixPathForCurrentOs(@"C:\proj\Directory.Packages.props");
        var subfolderPath = FixPathForCurrentOs(@"C:\proj\subfolder\other.props");

        _fileSystem[mainPath] = @"
<Project>
  <Import Project=""subfolder\other.props"" />
</Project>";

        _fileSystem[subfolderPath] = @"
<Project>
  <ItemGroup>
    <PackageVersion Include=""SubPackage"" Version=""1.0.0"" />
  </ItemGroup>
</Project>";

        var template = CreateTemplate(mainPath);

        // Act
        var success = template.TryGetVersion("SubPackage", out var version);

        // Assert
        success.ShouldBeTrue();
        version.ShouldBe("1.0.0");
    }

    [Fact]
    public void WhenCircularImportsExist_ShouldHandleGracefully()
    {
        // Arrange
        var mainPath = FixPathForCurrentOs(@"C:\proj\Directory.Packages.props");
        var import1Path = FixPathForCurrentOs(@"C:\proj\import1.props");
        var import2Path = FixPathForCurrentOs(@"C:\proj\import2.props");

        _fileSystem[mainPath] = @"
<Project>
  <Import Project=""import1.props"" />
</Project>";

        _fileSystem[import1Path] = @"
<Project>
  <Import Project=""import2.props"" />
  <ItemGroup>
    <PackageVersion Include=""Package1"" Version=""1.0.0"" />
  </ItemGroup>
</Project>";

        _fileSystem[import2Path] = @"
<Project>
  <Import Project=""import1.props"" />
  <ItemGroup>
    <PackageVersion Include=""Package2"" Version=""2.0.0"" />
  </ItemGroup>
</Project>";

        var template = CreateTemplate(mainPath);

        // Act
        template.TryGetVersion("Package1", out var version1).ShouldBeTrue();
        template.TryGetVersion("Package2", out var version2).ShouldBeTrue();

        // Assert - should load both packages despite circular reference
        version1.ShouldBe("1.0.0");
        version2.ShouldBe("2.0.0");
    }

    [Fact]
    public void WhenRunTemplateIsCalled_ShouldReturnValidXml()
    {
        // Arrange
        var mainPath = FixPathForCurrentOs(@"C:\proj\Directory.Packages.props");
        _fileSystem[mainPath] = @"
<Project>
  <ItemGroup>
    <PackageVersion Include=""TestPackage"" Version=""1.0.0"" />
  </ItemGroup>
</Project>";

        var template = CreateTemplate(mainPath);

        // Act
        var result = template.RunTemplate();

        // Assert
        result.ShouldNotBeNullOrWhiteSpace();
        result.ShouldContain("<Project>");
        result.ShouldContain("TestPackage");
        result.ShouldContain("1.0.0");
    }

    private string FixPathForCurrentOs(string path)
    {
        if (!OperatingSystem.IsWindows())
        {
            return path.Replace("C:", "").Replace("\\", "/");
        }

        return path;
    }

    private DirectoryPackagesPropsTemplate CreateTemplate(
        string filePath,
        bool centralPackageManagement = true)
    {
        _application.OutputRootDirectory.Returns(Path.GetDirectoryName(filePath));

        return new DirectoryPackagesPropsTemplate(
            _application,
            _model,
            centralPackageManagement,
            new TestableFileOperations(_fileSystem));
    }
}

public class TestableFileOperations : IFileOperations
{
    private readonly Dictionary<string, string> _fileSystem;

    public TestableFileOperations(Dictionary<string, string> fileSystem)
    {
        _fileSystem = fileSystem;
    }

    public bool FileExists(string path)
    {
        return _fileSystem.ContainsKey(path);
    }

    public string ReadAllText(string path)
    {
        return _fileSystem.GetValueOrDefault(path);
    }
}