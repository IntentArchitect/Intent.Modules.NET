using System;
using System.IO;
using System.Linq;
using System.Threading;
using Intent.Engine;
using Intent.Modules.VisualStudio.Projects.Templates.VisualStudioSolution.Merging;
using Microsoft.VisualStudio.SolutionPersistence.Model;
using Microsoft.VisualStudio.SolutionPersistence.Serializer;
using Shouldly;
using Xunit;

namespace Intent.Modules.VisualStudio.Projects.Tests.Templates.SlnxFile
{
  public class SlnxMergerTests
  {
    // ----- Bootstrap / no history -----

    [Fact]
    public void WhenNoExistingFile_ShouldGenerateFreshContentWithoutIds()
    {
      var domainId = Guid.NewGuid();
      var generated = Build(m =>
      {
        var folder = m.AddFolder("/3 - Domain/");
        var project = m.AddProject("MyApp.Domain/MyApp.Domain.csproj", null, folder);
        project.Id = domainId;
      });

      var result = SlnxMerger.Merge(generated, existing: null, previousOutput: null);

      ContainsIdAttribute(result).ShouldBeFalse();
      var model = Parse(result);
      model.SolutionProjects.Single().FilePath.Replace('\\', '/').ShouldBe("MyApp.Domain/MyApp.Domain.csproj");
    }

    [Fact]
    public void WhenExistingFileButNoPreviousOutput_ShouldFallBackToPathMatching()
    {
      // No previous-output cache (fresh clone) - must fall back to plain path matching.
      var domainId = Guid.NewGuid();
      var generated = Build(m =>
      {
        var folder = m.AddFolder("/3 - Domain/");
        var project = m.AddProject("MyApp.Domain/MyApp.Domain.csproj", null, folder);
        project.Id = domainId;
        var api = m.AddFolder("/1 - Api/");
        var newProject = m.AddProject("MyApp.NewThing/MyApp.NewThing.csproj", null, api);
        newProject.Id = Guid.NewGuid();
      });

      var existing = Build(m =>
      {
        var folder = m.AddFolder("/3 - Domain/");
        m.AddProject("MyApp.Domain/MyApp.Domain.csproj", null, folder);
      });

      var result = SlnxMerger.Merge(generated, existing, previousOutput: null);

      ContainsIdAttribute(result).ShouldBeFalse();
      var model = Parse(result);
      model.SolutionProjects.Select(p => p.FilePath.Replace('\\', '/')).ShouldBe(
        ["MyApp.Domain/MyApp.Domain.csproj", "MyApp.NewThing/MyApp.NewThing.csproj"],
        ignoreOrder: true);
    }

    [Fact]
    public void WhenPreviousOutputHasNoRealIds_MigrationBoundary_ShouldOrphanOldEntryRatherThanThrow()
    {
      // Base from before Ids were stamped - a rename in this run can't correlate, by design.
      var domainId = Guid.NewGuid();
      var previousOutput = Build(m =>
      {
        var folder = m.AddFolder("/3 - Domain/");
        m.AddProject("MyApp.Domain/MyApp.Domain.csproj", null, folder); // no Id - legacy output
      });

      var existing = Build(m =>
      {
        var folder = m.AddFolder("/3 - Domain/");
        m.AddProject("MyApp.Domain/MyApp.Domain.csproj", null, folder);
      });

      var generated = Build(m =>
      {
        var folder = m.AddFolder("/3 - Domain/");
        var project = m.AddProject("MyApp.Core/MyApp.Core.csproj", null, folder);
        project.Id = domainId;
      });

      var result = SlnxMerger.Merge(generated, existing, previousOutput);

      var model = Parse(result);
      model.SolutionProjects.Select(p => p.FilePath.Replace('\\', '/')).ShouldBe(
        ["MyApp.Domain/MyApp.Domain.csproj", "MyApp.Core/MyApp.Core.csproj"],
        ignoreOrder: true);
    }

    // ----- Idempotency -----

    [Fact]
    public void WhenNothingChanged_ShouldBeIdempotent()
    {
      var projectId = Guid.NewGuid();
      string Generate() => Build(m =>
      {
        var folder = m.AddFolder("/3 - Domain/");
        var project = m.AddProject("MyApp.Domain/MyApp.Domain.csproj", null, folder);
        project.Id = projectId;
      });

      var firstRun = SlnxMerger.Merge(Generate(), existing: null, previousOutput: null);
      var secondRun = SlnxMerger.Merge(Generate(), existing: firstRun, previousOutput: Generate());

      var model = Parse(secondRun);
      model.SolutionProjects.Count().ShouldBe(1);
      ContainsIdAttribute(secondRun).ShouldBeFalse();
    }

    // ----- Project-level changes -----

    [Fact]
    public void WhenProjectAdded_ShouldInsertIt()
    {
      var existingId = Guid.NewGuid();
      var newId = Guid.NewGuid();

      var previousOutput = Build(m =>
      {
        var folder = m.AddFolder("/1 - Api/");
        var p = m.AddProject("MyApp.Api/MyApp.Api.csproj", null, folder);
        p.Id = existingId;
      });

      var existing = Build(m =>
      {
        var folder = m.AddFolder("/1 - Api/");
        m.AddProject("MyApp.Api/MyApp.Api.csproj", null, folder);
      });

      var generated = Build(m =>
      {
        var folder = m.AddFolder("/1 - Api/");
        var p = m.AddProject("MyApp.Api/MyApp.Api.csproj", null, folder);
        p.Id = existingId;
        var domain = m.AddFolder("/3 - Domain/");
        var newProject = m.AddProject("MyApp.Domain/MyApp.Domain.csproj", null, domain);
        newProject.Id = newId;
      });

      var result = SlnxMerger.Merge(generated, existing, previousOutput);

      var model = Parse(result);
      model.SolutionProjects.Select(p => p.FilePath.Replace('\\', '/')).ShouldBe(
        ["MyApp.Api/MyApp.Api.csproj", "MyApp.Domain/MyApp.Domain.csproj"],
        ignoreOrder: true);
    }

    [Fact]
    public void WhenProjectRemovedFromModel_ShouldPreserveExistingEntry()
    {
      var apiId = Guid.NewGuid();
      var domainId = Guid.NewGuid();

      var previousOutput = Build(m =>
      {
        var api = m.AddFolder("/1 - Api/");
        var p1 = m.AddProject("MyApp.Api/MyApp.Api.csproj", null, api);
        p1.Id = apiId;
        var domain = m.AddFolder("/3 - Domain/");
        var p2 = m.AddProject("MyApp.Domain/MyApp.Domain.csproj", null, domain);
        p2.Id = domainId;
      });

      var existing = Build(m =>
      {
        var api = m.AddFolder("/1 - Api/");
        m.AddProject("MyApp.Api/MyApp.Api.csproj", null, api);
        var domain = m.AddFolder("/3 - Domain/");
        m.AddProject("MyApp.Domain/MyApp.Domain.csproj", null, domain);
      });

      // Domain project removed from the Intent model this run
      var generated = Build(m =>
      {
        var api = m.AddFolder("/1 - Api/");
        var p1 = m.AddProject("MyApp.Api/MyApp.Api.csproj", null, api);
        p1.Id = apiId;
      });

      var result = SlnxMerger.Merge(generated, existing, previousOutput);

      var model = Parse(result);
      model.SolutionProjects.Select(p => p.FilePath.Replace('\\', '/')).ShouldBe(
        ["MyApp.Api/MyApp.Api.csproj", "MyApp.Domain/MyApp.Domain.csproj"],
        ignoreOrder: true);
    }

    [Fact]
    public void WhenProjectRenamedSameFolder_UntouchedByUser_ShouldRenameInPlace()
    {
      var projectId = Guid.NewGuid();

      var previousOutput = Build(m =>
      {
        var folder = m.AddFolder("/3 - Domain/");
        var p = m.AddProject("MyApp.Domain/MyApp.Domain.csproj", null, folder);
        p.Id = projectId;
      });

      var existing = Build(m =>
      {
        var folder = m.AddFolder("/3 - Domain/");
        m.AddProject("MyApp.Domain/MyApp.Domain.csproj", null, folder);
      });

      var generated = Build(m =>
      {
        var folder = m.AddFolder("/3 - Domain/");
        var p = m.AddProject("MyApp.Core/MyApp.Core.csproj", null, folder);
        p.Id = projectId;
      });

      var result = SlnxMerger.Merge(generated, existing, previousOutput);

      var model = Parse(result);
      model.SolutionProjects.Count().ShouldBe(1, "the renamed project must update the existing entry, not orphan it");
      var project = model.SolutionProjects.Single();
      project.FilePath.Replace('\\', '/').ShouldBe("MyApp.Core/MyApp.Core.csproj");
      project.Parent!.Path.ShouldBe("/3 - Domain/");
      ContainsIdAttribute(result).ShouldBeFalse();
    }

    [Fact]
    public void WhenProjectMovedToDifferentFolder_UntouchedByUser_ShouldReparentInPlace()
    {
      var projectId = Guid.NewGuid();

      var previousOutput = Build(m =>
      {
        var src = m.AddFolder("/src/");
        m.AddFolder("/tests/");
        var p = m.AddProject("src/MyApp.Api/MyApp.Api.csproj", null, src);
        p.Id = projectId;
      });

      var existing = Build(m =>
      {
        var src = m.AddFolder("/src/");
        m.AddFolder("/tests/");
        m.AddProject("src/MyApp.Api/MyApp.Api.csproj", null, src);
      });

      var generated = Build(m =>
      {
        m.AddFolder("/src/");
        var tests = m.AddFolder("/tests/");
        var p = m.AddProject("src/MyApp.Api/MyApp.Api.csproj", null, tests);
        p.Id = projectId;
      });

      var result = SlnxMerger.Merge(generated, existing, previousOutput);

      var model = Parse(result);
      model.SolutionProjects.Count().ShouldBe(1);
      var project = model.SolutionProjects.Single();
      project.Parent!.Path.ShouldBe("/tests/");
    }

    [Fact]
    public void WhenProjectRenamedAndMovedSimultaneously_ShouldApplyBoth()
    {
      var projectId = Guid.NewGuid();

      var previousOutput = Build(m =>
      {
        var src = m.AddFolder("/src/");
        m.AddFolder("/tests/");
        var p = m.AddProject("src/MyApp.Api/MyApp.Api.csproj", null, src);
        p.Id = projectId;
      });

      var existing = Build(m =>
      {
        var src = m.AddFolder("/src/");
        m.AddFolder("/tests/");
        m.AddProject("src/MyApp.Api/MyApp.Api.csproj", null, src);
      });

      var generated = Build(m =>
      {
        m.AddFolder("/src/");
        var tests = m.AddFolder("/tests/");
        var p = m.AddProject("tests/MyApp.Api.Tests/MyApp.Api.Tests.csproj", null, tests);
        p.Id = projectId;
      });

      var result = SlnxMerger.Merge(generated, existing, previousOutput);

      var model = Parse(result);
      model.SolutionProjects.Count().ShouldBe(1);
      var project = model.SolutionProjects.Single();
      project.FilePath.Replace('\\', '/').ShouldBe("tests/MyApp.Api.Tests/MyApp.Api.Tests.csproj");
      project.Parent!.Path.ShouldBe("/tests/");
    }

    [Fact]
    public void WhenProjectRenamed_ButUserAlreadyEditedThatEntry_ShouldBackOffAndInsertFresh()
    {
      var projectId = Guid.NewGuid();

      var previousOutput = Build(m =>
      {
        var folder = m.AddFolder("/3 - Domain/");
        var p = m.AddProject("MyApp.Domain/MyApp.Domain.csproj", null, folder);
        p.Id = projectId;
      });

      // User already hand-renamed it in VS - the old path is simply gone from Existing.
      var existing = Build(m =>
      {
        var folder = m.AddFolder("/3 - Domain/");
        m.AddProject("MyApp.HandRenamed/MyApp.HandRenamed.csproj", null, folder);
      });

      var generated = Build(m =>
      {
        var folder = m.AddFolder("/3 - Domain/");
        var p = m.AddProject("MyApp.Core/MyApp.Core.csproj", null, folder);
        p.Id = projectId;
      });

      var result = SlnxMerger.Merge(generated, existing, previousOutput);

      var model = Parse(result);
      model.SolutionProjects.Select(p => p.FilePath.Replace('\\', '/')).ShouldBe(
        ["MyApp.HandRenamed/MyApp.HandRenamed.csproj", "MyApp.Core/MyApp.Core.csproj"],
        ignoreOrder: true);
    }

    [Fact]
    public void WhenTwoProjectsSwapPaths_ShouldNotCrossWireIdentities()
    {
      var idA = Guid.NewGuid();
      var idB = Guid.NewGuid();

      var previousOutput = Build(m =>
      {
        var folder = m.AddFolder("/1 - Api/");
        var a = m.AddProject("Foo/Foo.csproj", null, folder);
        a.Id = idA;
        var b = m.AddProject("Bar/Bar.csproj", null, folder);
        b.Id = idB;
      });

      var existing = Build(m =>
      {
        var folder = m.AddFolder("/1 - Api/");
        m.AddProject("Foo/Foo.csproj", null, folder);
        m.AddProject("Bar/Bar.csproj", null, folder);
      });

      // A renamed from Foo -> Bar, B renamed from Bar -> Foo, in the same run
      var generated = Build(m =>
      {
        var folder = m.AddFolder("/1 - Api/");
        var a = m.AddProject("Bar/Bar.csproj", null, folder);
        a.Id = idA;
        var b = m.AddProject("Foo/Foo.csproj", null, folder);
        b.Id = idB;
      });

      var result = SlnxMerger.Merge(generated, existing, previousOutput);

      var model = Parse(result);
      model.SolutionProjects.Count().ShouldBe(2, "the swap must not collapse or duplicate either entry");
      model.SolutionProjects.Select(p => p.FilePath.Replace('\\', '/')).ShouldBe(
        ["Foo/Foo.csproj", "Bar/Bar.csproj"],
        ignoreOrder: true);
    }

    // ----- Folder-level changes -----

    [Fact]
    public void WhenFolderRenamed_UntouchedByUser_ShouldRenameInPlaceNoOrphan()
    {
      var folderId = Guid.NewGuid();
      var projectId = Guid.NewGuid();

      var previousOutput = Build(m =>
      {
        var folder = m.AddFolder("/3 - Domain/");
        folder.Id = folderId;
        var p = m.AddProject("MyApp.Domain/MyApp.Domain.csproj", null, folder);
        p.Id = projectId;
      });

      var existing = Build(m =>
      {
        var folder = m.AddFolder("/3 - Domain/");
        m.AddProject("MyApp.Domain/MyApp.Domain.csproj", null, folder);
      });

      var generated = Build(m =>
      {
        var folder = m.AddFolder("/3 - Core/");
        folder.Id = folderId;
        var p = m.AddProject("MyApp.Domain/MyApp.Domain.csproj", null, folder);
        p.Id = projectId;
      });

      var result = SlnxMerger.Merge(generated, existing, previousOutput);

      var model = Parse(result);
      model.SolutionFolders.Select(f => f.Path).ShouldBe(["/3 - Core/"], "the folder must be renamed in place, not left as an orphan alongside a new one");
      model.SolutionProjects.Single().Parent!.Path.ShouldBe("/3 - Core/");
    }

    [Fact]
    public void WhenFolderReparented_ShouldMoveInPlace()
    {
      var folderId = Guid.NewGuid();

      var previousOutput = Build(m =>
      {
        m.AddFolder("/A/");
        m.AddFolder("/B/");
        var folder = m.AddFolder("/A/Shared/");
        folder.Id = folderId;
      });

      var existing = Build(m =>
      {
        m.AddFolder("/A/");
        m.AddFolder("/B/");
        m.AddFolder("/A/Shared/");
      });

      var generated = Build(m =>
      {
        m.AddFolder("/A/");
        m.AddFolder("/B/");
        var b = m.AddFolder("/B/Shared/");
        b.Id = folderId;
      });

      var result = SlnxMerger.Merge(generated, existing, previousOutput);

      var model = Parse(result);
      model.SolutionFolders.Select(f => f.Path).ShouldContain("/B/Shared/");
      model.SolutionFolders.Select(f => f.Path).ShouldNotContain("/A/Shared/");
    }

    [Fact]
    public void WhenNestedFolderRenamed_ParentUnaffected_ShouldRenameOnlyThatFolder()
    {
      var parentId = Guid.NewGuid();
      var childId = Guid.NewGuid();

      var previousOutput = Build(m =>
      {
        var parent = m.AddFolder("/Database/");
        parent.Id = parentId;
        var child = m.AddFolder("/Database/Infrastructure/");
        child.Id = childId;
      });

      var existing = Build(m =>
      {
        m.AddFolder("/Database/");
        m.AddFolder("/Database/Infrastructure/");
      });

      var generated = Build(m =>
      {
        var parent = m.AddFolder("/Database/");
        parent.Id = parentId;
        var child = m.AddFolder("/Database/Persistence/");
        child.Id = childId;
      });

      var result = SlnxMerger.Merge(generated, existing, previousOutput);

      var model = Parse(result);
      model.SolutionFolders.Select(f => f.Path).ShouldBe(["/Database/", "/Database/Persistence/"], ignoreOrder: true);
    }

    [Fact]
    public void WhenParentFolderRenamed_ChildFolderShouldCascadeCorrectly()
    {
      var parentId = Guid.NewGuid();
      var childId = Guid.NewGuid();
      var projectId = Guid.NewGuid();

      var previousOutput = Build(m =>
      {
        var parent = m.AddFolder("/Database/");
        parent.Id = parentId;
        var child = m.AddFolder("/Database/Infrastructure/");
        child.Id = childId;
        var p = m.AddProject("Infrastructure/MyApp.Migrations/MyApp.Migrations.csproj", null, child);
        p.Id = projectId;
      });

      var existing = Build(m =>
      {
        m.AddFolder("/Database/");
        var child = m.AddFolder("/Database/Infrastructure/");
        m.AddProject("Infrastructure/MyApp.Migrations/MyApp.Migrations.csproj", null, child);
      });

      // Parent renamed, child's own name unchanged
      var generated = Build(m =>
      {
        var parent = m.AddFolder("/Persistence/");
        parent.Id = parentId;
        var child = m.AddFolder("/Persistence/Infrastructure/");
        child.Id = childId;
        var p = m.AddProject("Infrastructure/MyApp.Migrations/MyApp.Migrations.csproj", null, child);
        p.Id = projectId;
      });

      var result = SlnxMerger.Merge(generated, existing, previousOutput);

      var model = Parse(result);
      model.SolutionFolders.Select(f => f.Path).ShouldBe(["/Persistence/", "/Persistence/Infrastructure/"], ignoreOrder: true);
      model.SolutionProjects.Single().Parent!.Path.ShouldBe("/Persistence/Infrastructure/");
    }

    [Fact]
    public void WhenFolderRenamed_ButUserAlreadyHandRenamedIt_ShouldBackOffAndInsertFreshFolder()
    {
      var folderId = Guid.NewGuid();

      var previousOutput = Build(m =>
      {
        var folder = m.AddFolder("/3 - Domain/");
        folder.Id = folderId;
      });

      // User already hand-renamed the folder directly in the file
      var existing = Build(m => m.AddFolder("/3 - Domain Hand-Renamed/"));

      var generated = Build(m =>
      {
        var folder = m.AddFolder("/3 - Core/");
        folder.Id = folderId;
      });

      var result = SlnxMerger.Merge(generated, existing, previousOutput);

      var model = Parse(result);
      model.SolutionFolders.Select(f => f.Path).ShouldBe(["/3 - Domain Hand-Renamed/", "/3 - Core/"], ignoreOrder: true);
    }

    [Fact]
    public void WhenTwoFoldersSwapNames_ShouldNotCrossWireIdentities()
    {
      var idA = Guid.NewGuid();
      var idB = Guid.NewGuid();

      var previousOutput = Build(m =>
      {
        var a = m.AddFolder("/Foo/");
        a.Id = idA;
        var b = m.AddFolder("/Bar/");
        b.Id = idB;
      });

      var existing = Build(m =>
      {
        m.AddFolder("/Foo/");
        m.AddFolder("/Bar/");
      });

      var generated = Build(m =>
      {
        var a = m.AddFolder("/Bar/");
        a.Id = idA;
        var b = m.AddFolder("/Foo/");
        b.Id = idB;
      });

      var result = SlnxMerger.Merge(generated, existing, previousOutput);

      var model = Parse(result);
      model.SolutionFolders.Count().ShouldBe(2, "the swap must not collapse or duplicate either folder");
    }

    // ----- Preservation of manual content -----

    [Fact]
    public void WhenUserManuallyAddedProject_ShouldBePreserved()
    {
      var apiId = Guid.NewGuid();

      var previousOutput = Build(m =>
      {
        var folder = m.AddFolder("/1 - Api/");
        var p = m.AddProject("MyApp.Api/MyApp.Api.csproj", null, folder);
        p.Id = apiId;
      });

      var existing = Build(m =>
      {
        var folder = m.AddFolder("/1 - Api/");
        m.AddProject("MyApp.Api/MyApp.Api.csproj", null, folder);
        m.AddProject("MyApp.External/MyApp.External.csproj"); // added by hand, never Intent-modelled
      });

      var generated = Build(m =>
      {
        var folder = m.AddFolder("/1 - Api/");
        var p = m.AddProject("MyApp.Api/MyApp.Api.csproj", null, folder);
        p.Id = apiId;
      });

      var result = SlnxMerger.Merge(generated, existing, previousOutput);

      var model = Parse(result);
      model.SolutionProjects.Select(p => p.FilePath.Replace('\\', '/')).ShouldContain("MyApp.External/MyApp.External.csproj");
    }

    [Fact]
    public void WhenUserManuallyAddedFolder_ShouldBePreserved()
    {
      var previousOutput = Build(m => { });
      var existing = Build(m => m.AddFolder("/NewFolder1/"));
      var generated = Build(m => { });

      var result = SlnxMerger.Merge(generated, existing, previousOutput);

      var model = Parse(result);
      model.SolutionFolders.Select(f => f.Path).ShouldContain("/NewFolder1/");
    }

    [Fact]
    public void WhenUserAddedUnrelatedSolutionItems_ShouldSurviveRegardlessOfIntentChanges()
    {
      var apiId = Guid.NewGuid();

      var previousOutput = Build(m =>
      {
        var folder = m.AddFolder("/0 - Solution Items/");
        folder.AddFile("README.md");
      });

      var existing = Build(m =>
      {
        var folder = m.AddFolder("/0 - Solution Items/");
        folder.AddFile("README.md");
      });

      var generated = Build(m =>
      {
        var folder = m.AddFolder("/1 - Api/");
        var p = m.AddProject("MyApp.Api/MyApp.Api.csproj", null, folder);
        p.Id = apiId;
      });

      var result = SlnxMerger.Merge(generated, existing, previousOutput);

      var model = Parse(result);
      model.SolutionFolders.Single(f => f.Path == "/0 - Solution Items/").Files.ShouldContain("README.md");
      model.SolutionProjects.Select(p => p.FilePath.Replace('\\', '/')).ShouldContain("MyApp.Api/MyApp.Api.csproj");
    }

    [Fact]
    public void WhenExistingFileHasNonDefaultBuildTypeName_ShouldPreserveIt()
    {
      // "Staging" isn't a library default, so this proves genuine preservation, not coincidence.
      var previousOutput = Build(m => { });

      var existing = Build(m =>
      {
        m.AddBuildType("Staging");
      });

      var generated = Build(m => { });

      var result = SlnxMerger.Merge(generated, existing, previousOutput);
      var model = Parse(result);

      model.BuildTypes.ShouldContain("Staging", customMessage: "raw regenerated .slnx:\n" + result);
    }

    [Fact]
    public void WhenExistingFileHasNonIdentityProjectConfigurationRule_ShouldPreserveIt()
    {
      // A per-project rule needs at least one Platform declared for its "*" wildcard to expand against.
      var previousOutput = Build(m =>
      {
        var api = m.AddFolder("/1 - Api/");
        m.AddProject("MyApp.Api/MyApp.Api.csproj", null, api);
      });

      var existing = Build(m =>
      {
        m.AddBuildType("Debug");
        m.AddPlatform("x64");
        var api = m.AddFolder("/1 - Api/");
        var apiProject = m.AddProject("MyApp.Api/MyApp.Api.csproj", null, api);
        apiProject.AddProjectConfigurationRule(new ConfigurationRule(BuildDimension.BuildType, "Debug", string.Empty, "Staging"));
      });

      var generated = Build(m =>
      {
        var api = m.AddFolder("/1 - Api/");
        m.AddProject("MyApp.Api/MyApp.Api.csproj", null, api);
      });

      var result = SlnxMerger.Merge(generated, existing, previousOutput);
      var model = Parse(result);
      var apiResult = model.SolutionProjects.Single(p => p.FilePath.Replace('\\', '/') == "MyApp.Api/MyApp.Api.csproj");

      (apiResult.ProjectConfigurationRules?.Count ?? 0).ShouldBe(1, "raw regenerated .slnx:\n" + result);
    }

    [Fact]
    public void WhenExistingFileHasManualBuildConfiguration_ShouldPreserveEverything()
    {
      // Non-identity values (MyDebug/MyRelease) so this doesn't rely on the Debug/Release defaults.
      var apiId = Guid.NewGuid();
      var domainId = Guid.NewGuid();

      var previousOutput = Build(m =>
      {
        var api = m.AddFolder("/1 - Api/");
        var p1 = m.AddProject("MyApp.Api/MyApp.Api.csproj", null, api);
        p1.Id = apiId;
        var domain = m.AddFolder("/3 - Domain/");
        var p2 = m.AddProject("MyApp.Domain/MyApp.Domain.csproj", null, domain);
        p2.Id = domainId;
      });

      var existing = Build(m =>
      {
        m.AddBuildType("Debug");
        m.AddBuildType("Release");
        m.AddPlatform("Any CPU");
        m.AddPlatform("x64");

        var api = m.AddFolder("/1 - Api/");
        var apiProject = m.AddProject("MyApp.Api/MyApp.Api.csproj", null, api);
        var domain = m.AddFolder("/3 - Domain/");
        var domainProject = m.AddProject("MyApp.Domain/MyApp.Domain.csproj", null, domain);

        apiProject.AddProjectConfigurationRule(new ConfigurationRule(BuildDimension.BuildType, "Debug", string.Empty, "MyDebug"));
        apiProject.AddProjectConfigurationRule(new ConfigurationRule(BuildDimension.BuildType, "Release", string.Empty, "MyRelease"));
        apiProject.AddDependency(domainProject);
        apiProject.AddProperties("ManualTest", PropertiesScope.PreLoad).Add("Foo", "Bar");
      });

      var generated = Build(m =>
      {
        var api = m.AddFolder("/1 - Api/");
        var p1 = m.AddProject("MyApp.Api/MyApp.Api.csproj", null, api);
        p1.Id = apiId;
        var domain = m.AddFolder("/3 - Domain/");
        var p2 = m.AddProject("MyApp.Domain/MyApp.Domain.csproj", null, domain);
        p2.Id = domainId;
      });

      var result = SlnxMerger.Merge(generated, existing, previousOutput);
      var model = Parse(result);
      var resultApiProject = model.SolutionProjects.Single(p => p.FilePath.Replace('\\', '/') == "MyApp.Api/MyApp.Api.csproj");

      var actual = string.Join("\n", new[]
      {
        $"BuildTypes: {string.Join(",", model.BuildTypes)}",
        $"Platforms: {string.Join(",", model.Platforms)}",
        $"Api.ProjectConfigurationRules: {resultApiProject.ProjectConfigurationRules?.Count ?? 0}",
        $"Api.Dependencies: {resultApiProject.Dependencies?.Count ?? 0}",
        $"Api.Properties: {resultApiProject.Properties?.Count ?? 0}",
      });

      actual.ShouldBe(string.Join("\n", new[]
      {
        "BuildTypes: Debug,Release",
        "Platforms: Any CPU,x64",
        "Api.ProjectConfigurationRules: 2",
        "Api.Dependencies: 1",
        "Api.Properties: 1",
      }), "raw regenerated .slnx:\n" + result);
    }

    [Fact]
    public void WhenExistingFileIsTheReportedRealWorldRepro_ShouldPreserveEverything()
    {
      // Literal repro from a real solution: identity BuildType rules (Debug->Debug, Release->Release).
      const string previousOutput = """
          <Solution>
            <Folder Name="/1 - Api/" Id="e8d44da4-54a6-405d-b825-6efca31297b1">
              <Project Path="NewApplication.Api/NewApplication.Api.csproj" Id="6df42bcd-087d-48e9-aaf6-9615a0a7ec7c" />
            </Folder>
            <Folder Name="/2 - Application/" Id="c8eff722-74d1-43cb-bc5f-98563ca3c0a5">
              <Project Path="NewApplication.Application.UnitTests/NewApplication.Application.UnitTests.csproj" Id="bf49bc09-ab93-4583-bb48-8174fcc8d073" />
              <Project Path="NewApplication.Application/NewApplication.Application.csproj" Id="d1e5c1fb-3db9-435a-a672-487fcb281c2a" />
            </Folder>
            <Folder Name="/3 - Domain/" Id="ca017856-32b9-4ae6-8325-8aee63438909">
              <Project Path="NewApplication.Domain/NewApplication.Domain.csproj" Id="bf438d87-2917-499e-9956-f908eb9eb7f1" />
            </Folder>
            <Folder Name="/4 - Infrastructure/" Id="c2df5fb0-f8c6-4218-b229-2729b255f6d4">
              <Project Path="NewApplication.Infrastructure/NewApplication.Infrastructure.csproj" Id="160970db-191e-41e4-84e0-d90f02e5c19e" />
            </Folder>
          </Solution>
          """;

      const string existing = """
          <Solution>
            <!-- Manually added to verify the Software Factory preserves custom .slnx content -->
            <Configurations>
              <BuildType Name="Debug" />
              <BuildType Name="Release" />
              <Platform Name="Any CPU" />
              <Platform Name="x64" />
            </Configurations>
            <Folder Name="/1 - Api/">
              <Project Path="NewApplication.Api/NewApplication.Api.csproj">
                <BuildType Solution="Debug|*" Project="Debug" />
                <BuildType Solution="Release|*" Project="Release" />
                <BuildDependency Project="NewApplication.Domain/NewApplication.Domain.csproj" />
                <Properties Name="ManualTest" Value="true" />
              </Project>
            </Folder>
            <Folder Name="/2 - Application/">
              <Project Path="NewApplication.Application.UnitTests/NewApplication.Application.UnitTests.csproj" />
              <Project Path="NewApplication.Application/NewApplication.Application.csproj" />
            </Folder>
            <Folder Name="/3 - Domain/">
              <Project Path="NewApplication.Domain/NewApplication.Domain.csproj" />
            </Folder>
            <Folder Name="/4 - Infrastructure/">
              <Project Path="NewApplication.Infrastructure/NewApplication.Infrastructure.csproj" />
            </Folder>
          </Solution>
          """;

      var result = SlnxMerger.Merge(generated: previousOutput, existing, previousOutput);
      var model = Parse(result);
      var apiProject = model.SolutionProjects.Single(p => p.FilePath.Replace('\\', '/') == "NewApplication.Api/NewApplication.Api.csproj");

      var actual = string.Join("\n", new[]
      {
        $"BuildTypes: {string.Join(",", model.BuildTypes)}",
        $"Platforms: {string.Join(",", model.Platforms)}",
        $"Comment present: {result.Contains("Manually added to verify")}",
        $"Api.ProjectConfigurationRules: {apiProject.ProjectConfigurationRules?.Count ?? 0}",
        $"Api.Dependencies: {apiProject.Dependencies?.Count ?? 0}",
        $"Api.Properties: {apiProject.Properties?.Count ?? 0}",
        $"Project count: {model.SolutionProjects.Count()}",
      });

      actual.ShouldBe(string.Join("\n", new[]
      {
        "BuildTypes: Debug,Release",
        "Platforms: Any CPU,x64",
        "Comment present: True",
        "Api.ProjectConfigurationRules: 2",
        "Api.Dependencies: 1",
        "Api.Properties: 1",
        "Project count: 5",
      }), "raw regenerated .slnx:\n" + result);

      result.ShouldContain("<BuildType Name=\"Debug\" />");
      result.ShouldContain("<BuildType Name=\"Release\" />");
    }

    // ----- Crash / corruption -----

    [Fact]
    public void WhenExistingFileHasDuplicateProjectEntry_ShouldThrowExplainingDuplicate()
    {
      var generated = Build(m => { });
      const string existing = """
        <Solution>
        <Folder Name="/3 - Domain/">
        <Project Path="MyApp.Domain/MyApp.Domain.csproj" />
        </Folder>
        <Folder Name="/4 - Infrastructure/">
        <Project Path="MyApp.Domain/MyApp.Domain.csproj" />
        </Folder>
        </Solution>
        """;

      var ex = Should.Throw<Exception>(() => SlnxMerger.Merge(generated, existing, previousOutput: null));
      ex.Message.ShouldContain("duplicate");
      ex.Message.ShouldContain(Normalized(existing));
    }

    [Fact]
    public void WhenExistingFileIsNotValidXml_ShouldThrowExplainingInvalidXml()
    {
      var generated = Build(m => { });
      const string existing = """
        <Solution>
        <Folder Name="/3 - Domain/">
        <Project Path="MyApp.Domain/MyApp.Domain.csproj" />
        </Folder>
        """; // missing closing </Solution> - not well-formed XML

      var ex = Should.Throw<Exception>(() => SlnxMerger.Merge(generated, existing, previousOutput: null));
      ex.Message.ShouldContain("not valid XML");
      ex.Message.ShouldContain(Normalized(existing));
    }

    [Fact]
    public void WhenPreviousOutputIsClassicSlnFormat_SwitchedFromSlnToSlnx_ShouldIgnoreHistoryRatherThanThrow()
    {
      // A .sln-to-.slnx format switch hands back the old .sln text as previousOutput - not crash.
      const string previousOutput = """
        Microsoft Visual Studio Solution File, Format Version 12.00
        # Visual Studio Version 17
        Project("{FAE04EC0-301F-11D3-BF4B-00C04F79EFBC}") = "MyApp.Domain", "MyApp.Domain\MyApp.Domain.csproj", "{11111111-1111-1111-1111-111111111111}"
        EndProject
        """;

      var existing = Build(m =>
      {
        var folder = m.AddFolder("/3 - Domain/");
        m.AddProject("MyApp.Domain/MyApp.Domain.csproj", null, folder);
      });

      var generated = Build(m =>
      {
        var folder = m.AddFolder("/3 - Domain/");
        m.AddProject("MyApp.Domain/MyApp.Domain.csproj", null, folder);
      });

      var result = SlnxMerger.Merge(generated, existing, previousOutput);

      var model = Parse(result);
      model.SolutionProjects.Select(p => p.FilePath.Replace('\\', '/')).ShouldBe(["MyApp.Domain/MyApp.Domain.csproj"]);
    }

    [Fact]
    public void WhenExistingFileIsWellFormedXmlButNotASolution_ShouldThrowWithLibraryMessage()
    {
      var generated = Build(m => { });
      const string existing = """
        <NotASolution>
        </NotASolution>
        """;

      var ex = Should.Throw<Exception>(() => SlnxMerger.Merge(generated, existing, previousOutput: null));
      ex.Message.ShouldContain("Not a solution file");
      ex.Message.ShouldNotContain("duplicate");
      ex.Message.ShouldContain(Normalized(existing));
    }

    // ----- Output guarantee -----

    [Fact]
    public void WhenNoExistingFile_RebuildStripsGeneratedIdRegardlessOfSource()
    {
      // Only the first-generation path guarantees no Id - see WhenExistingFileAlreadyHasAnIdAttribute_PreservationContractKeepsItUntouched for the reconcile path.
      var untouchedId = Guid.NewGuid();

      var generated = Build(m =>
      {
        var folder = m.AddFolder("/1 - Api/");
        var p = m.AddProject("MyApp.Api/MyApp.Api.csproj", null, folder);
        p.Id = untouchedId;
      });

      var result = SlnxMerger.Merge(generated, existing: null, previousOutput: null);

      ContainsIdAttribute(result).ShouldBeFalse();
    }

    [Fact]
    public void WhenNoExistingFile_RebuildStripsGeneratedIdsAcrossComplexFirstGeneration()
    {
      // Same first-generation-only guarantee as above, exercised with a more complex model.
      var idA = Guid.NewGuid();
      var idB = Guid.NewGuid();

      var generated = Build(m =>
      {
        var folder = m.AddFolder("/3 - Core/");
        var a = m.AddProject("MyApp.Core/MyApp.Core.csproj", null, folder);
        a.Id = idA;
        var b = m.AddProject("MyApp.New/MyApp.New.csproj", null, folder);
        b.Id = idB;
      });

      var result = SlnxMerger.Merge(generated, existing: null, previousOutput: null);

      ContainsIdAttribute(result).ShouldBeFalse();
    }

    // ----- Preservation of existing constructs (build types, properties, comments) -----

    private const string PreservationFixture = """
      <Solution Description="Acme Portal solution" Version="1.0">
      <!-- a hand-written comment -->
      <Configurations>
      <BuildType Name="Debug" />
      <BuildType Name="Release-Linux" />
      <BuildType Name="Release-Windows" />
      <Platform Name="Any CPU" />
      <Platform Name="x64" />
      </Configurations>
      <Folder Name="/0 - Solution Items/">
      <File Path=".gitignore" />
      <File Path="Directory.Build.props" />
      </Folder>
      <Folder Name="/1 - Server/" />
      <Folder Name="/1 - Server/1.a - Api/">
      <Project Path="Acme.Portal.Api/Acme.Portal.Api.csproj" Type="C#" DisplayName="Portal API">
      <BuildType Solution="Release-Linux|*" Project="Release" />
      <BuildType Solution="Release-Windows|*" Project="Release" />
      <BuildDependency Project="Acme.Portal.Domain/Acme.Portal.Domain.csproj" />
      <Properties Name="Custom">
      <Property Name="DeployTarget" Value="linux-x64" />
      </Properties>
      </Project>
      </Folder>
      <Folder Name="/1 - Server/1.b - Domain/">
      <Project Path="Acme.Portal.Domain/Acme.Portal.Domain.csproj" />
      </Folder>
      </Solution>
      """;

    [Fact]
    public void WhenNothingIntentDrivenChanges_EveryUserConstructSurvivesUnchanged()
    {
      var generated = Build(m =>
      {
        m.AddFolder("/1 - Server/");
        var api = m.AddFolder("/1 - Server/1.a - Api/");
        m.AddProject("Acme.Portal.Api/Acme.Portal.Api.csproj", null, api);
        var domain = m.AddFolder("/1 - Server/1.b - Domain/");
        m.AddProject("Acme.Portal.Domain/Acme.Portal.Domain.csproj", null, domain);
      });

      var result = SlnxMerger.Merge(generated, PreservationFixture, previousOutput: null);

      result.ShouldContain("Release-Linux");
      result.ShouldContain("Release-Windows");
      result.ShouldContain("BuildDependency");
      result.ShouldContain("DeployTarget");
      result.ShouldContain("a hand-written comment");
      result.ShouldContain("Description=\"Acme Portal solution\"");
      result.ShouldContain("Type=\"C#\"");
      result.ShouldContain("Portal API");
    }

    [Fact]
    public void WhenUnrelatedProjectAdded_ConfigurationsAndExistingRulesSurvive()
    {
      var newId = Guid.NewGuid();
      var generated = Build(m =>
      {
        m.AddFolder("/1 - Server/");
        var api = m.AddFolder("/1 - Server/1.a - Api/");
        m.AddProject("Acme.Portal.Api/Acme.Portal.Api.csproj", null, api);
        var domain = m.AddFolder("/1 - Server/1.b - Domain/");
        m.AddProject("Acme.Portal.Domain/Acme.Portal.Domain.csproj", null, domain);
        var worker = m.AddFolder("/1 - Server/1.c - Worker/");
        var p = m.AddProject("Acme.Portal.Worker/Acme.Portal.Worker.csproj", null, worker);
        p.Id = newId;
      });

      var result = SlnxMerger.Merge(generated, PreservationFixture, previousOutput: null);

      result.ShouldContain("Release-Linux");
      result.ShouldContain("BuildDependency");
      var model = Parse(result);
      model.SolutionProjects.Select(p => p.FilePath.Replace('\\', '/')).ShouldContain("Acme.Portal.Worker/Acme.Portal.Worker.csproj");
    }

    [Fact]
    public void WhenProjectWithBuildTypeRulesIsRenamed_RulesFollowTheRename()
    {
      var apiId = Guid.NewGuid();

      var previousOutput = Build(m =>
      {
        var api = m.AddFolder("/1 - Server/1.a - Api/");
        var p = m.AddProject("Acme.Portal.Api/Acme.Portal.Api.csproj", null, api);
        p.Id = apiId;
      });

      var generated = Build(m =>
      {
        var api = m.AddFolder("/1 - Server/1.a - Api/");
        var p = m.AddProject("Acme.Portal.WebApi/Acme.Portal.WebApi.csproj", null, api);
        p.Id = apiId;
        var domain = m.AddFolder("/1 - Server/1.b - Domain/");
        m.AddProject("Acme.Portal.Domain/Acme.Portal.Domain.csproj", null, domain);
      });

      var result = SlnxMerger.Merge(generated, PreservationFixture, previousOutput);

      result.ShouldContain("Acme.Portal.WebApi");
      result.ShouldNotContain("Acme.Portal.Api/Acme.Portal.Api.csproj");
      result.ShouldContain("Release-Linux|*");
      result.ShouldContain("DeployTarget");
      result.ShouldContain("Type=\"C#\"");
      result.ShouldContain("Portal API");
    }

    [Fact]
    public void WhenProjectWithBuildTypeRulesIsReparented_RulesFollowTheMove()
    {
      var apiId = Guid.NewGuid();

      var previousOutput = Build(m =>
      {
        var api = m.AddFolder("/1 - Server/1.a - Api/");
        var p = m.AddProject("Acme.Portal.Api/Acme.Portal.Api.csproj", null, api);
        p.Id = apiId;
      });

      var generated = Build(m =>
      {
        var hosts = m.AddFolder("/1 - Server/1.z - Hosts/");
        var p = m.AddProject("Acme.Portal.Api/Acme.Portal.Api.csproj", null, hosts);
        p.Id = apiId;
        var domain = m.AddFolder("/1 - Server/1.b - Domain/");
        m.AddProject("Acme.Portal.Domain/Acme.Portal.Domain.csproj", null, domain);
      });

      var result = SlnxMerger.Merge(generated, PreservationFixture, previousOutput);

      var model = Parse(result);
      var project = model.SolutionProjects.Single(p => p.FilePath.Replace('\\', '/') == "Acme.Portal.Api/Acme.Portal.Api.csproj");
      project.Parent!.Path.ShouldBe("/1 - Server/1.z - Hosts/");
      result.ShouldContain("Release-Linux|*");
      result.ShouldContain("BuildDependency");
      result.ShouldContain("Type=\"C#\"");
      result.ShouldContain("Portal API");
    }

    [Fact]
    public void WhenProjectEntryRemovedByHand_ItsRulesDoNotReappearOnReinsert()
    {
      const string existingWithoutApi = """
        <Solution>
        <Folder Name="/1 - Server/" />
        <Folder Name="/1 - Server/1.b - Domain/">
        <Project Path="Acme.Portal.Domain/Acme.Portal.Domain.csproj" />
        </Folder>
        </Solution>
        """;

      var generated = Build(m =>
      {
        var api = m.AddFolder("/1 - Server/1.a - Api/");
        m.AddProject("Acme.Portal.Api/Acme.Portal.Api.csproj", null, api);
        var domain = m.AddFolder("/1 - Server/1.b - Domain/");
        m.AddProject("Acme.Portal.Domain/Acme.Portal.Domain.csproj", null, domain);
      });

      var result = SlnxMerger.Merge(generated, existingWithoutApi, previousOutput: null);

      var model = Parse(result);
      model.SolutionProjects.Select(p => p.FilePath.Replace('\\', '/')).ShouldContain("Acme.Portal.Api/Acme.Portal.Api.csproj");
      result.ShouldNotContain("Release-Linux");
      result.ShouldNotContain("BuildDependency");
    }

    [Fact]
    public void WhenExistingFileAlreadyHasAnIdAttribute_PreservationContractKeepsItUntouched()
    {
      // Intentional behaviour change: Reconcile never assigns an Id, so a real one survives.
      var untouchedId = Guid.NewGuid();

      var previousOutput = Build(m =>
      {
        var folder = m.AddFolder("/1 - Api/");
        var p = m.AddProject("MyApp.Api/MyApp.Api.csproj", null, folder);
        p.Id = untouchedId;
      });

      var existing = Build(m =>
      {
        var folder = m.AddFolder("/1 - Api/");
        var p = m.AddProject("MyApp.Api/MyApp.Api.csproj", null, folder);
        p.Id = untouchedId; // a real Id already present on disk
      });

      var generated = Build(m =>
      {
        var folder = m.AddFolder("/1 - Api/");
        var p = m.AddProject("MyApp.Api/MyApp.Api.csproj", null, folder);
        p.Id = untouchedId;
      });

      var result = SlnxMerger.Merge(generated, existing, previousOutput);

      ContainsIdAttribute(result).ShouldBeTrue();
    }

    // ----- Helpers -----

    private static bool ContainsIdAttribute(string content) =>
      System.Text.RegularExpressions.Regex.IsMatch(content, @"\sId=""");

    private static string Normalized(string content) =>
      content.Replace("\r\n", "\n");

    private static string Build(Action<SolutionModel> configure)
    {
      var model = new SolutionModel();
      configure(model);
      return Serialize(model);
    }

    private static string Serialize(SolutionModel model)
    {
      using var stream = new MemoryStream();
      SolutionSerializers.SlnXml.SaveAsync(stream, model, CancellationToken.None).GetAwaiter().GetResult();
      stream.Position = 0;
      return new StreamReader(stream).ReadToEnd();
    }

    private static SolutionModel Parse(string content)
    {
      using var stream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(content));
      return SolutionSerializers.SlnXml.OpenAsync(stream, CancellationToken.None).GetAwaiter().GetResult();
    }
  }
}
