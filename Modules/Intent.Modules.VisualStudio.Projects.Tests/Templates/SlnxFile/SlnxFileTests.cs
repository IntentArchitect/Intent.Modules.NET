using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Configuration;
using Intent.Metadata.Models;
using Intent.Modules.VisualStudio.Projects.Api;
using Intent.Modules.VisualStudio.Projects.Templates.VisualStudioSolution;
using Microsoft.VisualStudio.SolutionPersistence.Model;
using IntentSolutionFolderModel = Intent.Modules.VisualStudio.Projects.Api.SolutionFolderModel;
using NSubstitute;
using Shouldly;
using Xunit;

namespace Intent.Modules.VisualStudio.Projects.Tests.Templates.SlnxFile
{
    public class SlnxFileTests
    {
        [Fact]
        public void WhenNew_FlatProjects_ShouldAddAllAtRoot()
        {
            var model = new SolutionModel();
            var projects = new[]
            {
                CreateProject("MyApp.Api", relativeLocation: "MyApp.Api"),
                CreateProject("MyApp.Domain", relativeLocation: "MyApp.Domain"),
            };

            VisualStudioSolutionSlnxTemplate.SyncFoldersAndProjects(model, [], projects);

            model.SolutionProjects.Select(p => p.FilePath).ShouldBe(
                ["MyApp.Api/MyApp.Api.csproj", "MyApp.Domain/MyApp.Domain.csproj"],
                ignoreOrder: true);
            model.SolutionFolders.ShouldBeEmpty();
        }

        [Fact]
        public void WhenNew_WithNestedFolders_ShouldPlaceProjectsInCorrectFolders()
        {
            var model = new SolutionModel();
            var srcFolder = CreateFolder("src");
            var testFolder = CreateFolder("tests");
            var projects = new[]
            {
                CreateProject("MyApp.Api", relativeLocation: "src/MyApp.Api", parentFolder: srcFolder),
                CreateProject("MyApp.Domain", relativeLocation: "src/MyApp.Domain", parentFolder: srcFolder),
                CreateProject("MyApp.Tests", relativeLocation: "tests/MyApp.Tests", parentFolder: testFolder),
            };

            VisualStudioSolutionSlnxTemplate.SyncFoldersAndProjects(model, [srcFolder, testFolder], projects);

            model.SolutionFolders.Select(f => f.Path).ShouldBe(["/src/", "/tests/"], ignoreOrder: true);

            var srcSlnxFolder = model.SolutionFolders.Single(f => f.Path == "/src/");
            model.SolutionProjects
                .Where(p => p.Parent == srcSlnxFolder)
                .Select(p => p.FilePath)
                .ShouldBe(["src/MyApp.Api/MyApp.Api.csproj", "src/MyApp.Domain/MyApp.Domain.csproj"], ignoreOrder: true);

            var testSlnxFolder = model.SolutionFolders.Single(f => f.Path == "/tests/");
            model.SolutionProjects
                .Where(p => p.Parent == testSlnxFolder)
                .Select(p => p.FilePath)
                .ShouldBe(["tests/MyApp.Tests/MyApp.Tests.csproj"], ignoreOrder: true);
        }

        [Fact]
        public void WhenExisting_WithNewProject_ShouldAddProjectAndKeepExisting()
        {
            var model = new SolutionModel();
            model.AddProject("MyApp.Api/MyApp.Api.csproj");
            model.AddProject("MyApp.Domain/MyApp.Domain.csproj");

            var projects = new[]
            {
                CreateProject("MyApp.Api", relativeLocation: "MyApp.Api"),
                CreateProject("MyApp.Domain", relativeLocation: "MyApp.Domain"),
                CreateProject("MyApp.Application", relativeLocation: "MyApp.Application"),
            };

            VisualStudioSolutionSlnxTemplate.SyncFoldersAndProjects(model, [], projects);

            model.SolutionProjects.Select(p => p.FilePath).ShouldBe(
                [
                    "MyApp.Api/MyApp.Api.csproj",
                    "MyApp.Domain/MyApp.Domain.csproj",
                    "MyApp.Application/MyApp.Application.csproj"
                ],
                ignoreOrder: true);
        }

        [Fact]
        public void WhenExisting_WithRemovedProject_ShouldRemoveStaleProject()
        {
            var model = new SolutionModel();
            model.AddProject("MyApp.Api/MyApp.Api.csproj");
            model.AddProject("MyApp.Domain/MyApp.Domain.csproj");
            model.AddProject("MyApp.OldService/MyApp.OldService.csproj");

            var projects = new[]
            {
                CreateProject("MyApp.Api", relativeLocation: "MyApp.Api"),
                CreateProject("MyApp.Domain", relativeLocation: "MyApp.Domain"),
            };

            VisualStudioSolutionSlnxTemplate.SyncFoldersAndProjects(model, [], projects);

            model.SolutionProjects.Select(p => p.FilePath).ShouldBe(
                ["MyApp.Api/MyApp.Api.csproj", "MyApp.Domain/MyApp.Domain.csproj"],
                ignoreOrder: true);
        }

        [Fact]
        public void WhenExisting_WithRemovedFolder_ShouldRemoveStaleFolderAndItsProjects()
        {
            var model = new SolutionModel();
            var existingFolder = model.AddFolder("/legacy/");
            model.AddProject("legacy/MyApp.Legacy/MyApp.Legacy.csproj", null, existingFolder);
            model.AddProject("MyApp.Api/MyApp.Api.csproj");

            var projects = new[]
            {
                CreateProject("MyApp.Api", relativeLocation: "MyApp.Api"),
            };

            VisualStudioSolutionSlnxTemplate.SyncFoldersAndProjects(model, [], projects);

            model.SolutionFolders.ShouldBeEmpty();
            model.SolutionProjects.Select(p => p.FilePath)
                .ShouldBe(["MyApp.Api/MyApp.Api.csproj"], ignoreOrder: true);
        }

        [Fact]
        public void WhenAlreadySynced_ShouldBeIdempotent()
        {
            var model = new SolutionModel();
            var srcFolder = CreateFolder("src");
            var projects = new[]
            {
                CreateProject("MyApp.Api", relativeLocation: "src/MyApp.Api", parentFolder: srcFolder),
                CreateProject("MyApp.Domain", relativeLocation: "src/MyApp.Domain", parentFolder: srcFolder),
            };

            VisualStudioSolutionSlnxTemplate.SyncFoldersAndProjects(model, [srcFolder], projects);
            var afterFirstSync = new
            {
                Projects = model.SolutionProjects.Select(p => p.FilePath).OrderBy(x => x).ToArray(),
                Folders = model.SolutionFolders.Select(f => f.Path).OrderBy(x => x).ToArray(),
            };

            VisualStudioSolutionSlnxTemplate.SyncFoldersAndProjects(model, [srcFolder], projects);
            var afterSecondSync = new
            {
                Projects = model.SolutionProjects.Select(p => p.FilePath).OrderBy(x => x).ToArray(),
                Folders = model.SolutionFolders.Select(f => f.Path).OrderBy(x => x).ToArray(),
            };

            afterSecondSync.Projects.ShouldBe(afterFirstSync.Projects);
            afterSecondSync.Folders.ShouldBe(afterFirstSync.Folders);
        }

        [Fact]
        public void WhenNew_DeepNestedFolders_ShouldNestCorrectly()
        {
            var model = new SolutionModel();
            var infraFolder = CreateFolder("Infrastructure");
            var dbFolder = CreateFolder("Database", subFolders: [infraFolder]);
            var projects = new[]
            {
                CreateProject("MyApp.Migrations", relativeLocation: "Infrastructure/MyApp.Migrations", parentFolder: infraFolder),
            };

            VisualStudioSolutionSlnxTemplate.SyncFoldersAndProjects(model, [dbFolder], projects);

            model.SolutionFolders.Select(f => f.Path).ShouldBe(
                ["/Database/", "/Database/Infrastructure/"],
                ignoreOrder: true);
            model.SolutionProjects.Single().FilePath.ShouldBe("Infrastructure/MyApp.Migrations/MyApp.Migrations.csproj");
        }

        private static IVisualStudioSolutionProject CreateProject(
            string name,
            string relativeLocation,
            IntentSolutionFolderModel parentFolder = null,
            string fileExtension = "csproj")
        {
            var outputTargetConfig = Substitute.For<IOutputTargetConfig>();
            outputTargetConfig.RelativeLocation.Returns(relativeLocation);

            var project = Substitute.For<IVisualStudioSolutionProject>();
            project.Name.Returns(name);
            project.FileExtension.Returns(fileExtension);
            project.ToOutputTargetConfig().Returns(outputTargetConfig);
            project.ParentFolder.Returns(parentFolder);
            return project;
        }

        private static IntentSolutionFolderModel CreateFolder(string name, IEnumerable<IntentSolutionFolderModel> subFolders = null)
        {
            var element = Substitute.For<IElement>();
            subFolders ??= [];

            var childElements = subFolders.Select(x =>
            {
                x.InternalElement.ParentElement.Returns(element);
                x.InternalElement.ParentId.Returns(element.Id);
                return x.InternalElement;
            }).ToArray();

            element.Id.Returns(Guid.NewGuid().ToString());
            element.Name.Returns(name);
            element.ChildElements.Returns(childElements);
            element.SpecializationType.Returns(IntentSolutionFolderModel.SpecializationType);
            element.SpecializationTypeId.Returns(IntentSolutionFolderModel.SpecializationTypeId);
            element.ParentElement.Returns((IElement)null);
            element.ParentId.Returns((string)null);

            return new IntentSolutionFolderModel(element);
        }
    }
}
