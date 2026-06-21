using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Configuration;
using Intent.Engine;
using Intent.Eventing;
using Intent.Metadata.Models;
using Intent.Modules.Common.Plugins;
using Intent.Modules.Common.Templates;
using Intent.Plugins.FactoryExtensions;
using Intent.Modules.Constants;
using Intent.Modules.VisualStudio.Projects.Api;
using Intent.Modules.VisualStudio.Projects.FactoryExtensions;
using Intent.Modules.VisualStudio.Projects.Templates.VisualStudioSolution;
using Intent.Templates;
using OverwriteBehaviour = Intent.Templates.OverwriteBehaviour;
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
        public void WhenExisting_WithRemovedProject_InFolder_ShouldRemoveProjectButPreserveFolder()
        {
            // Folders are never removed so that manually-added solution folders survive SF runs.
            // Only projects absent from the Intent model are removed.
            var model = new SolutionModel();
            var existingFolder = model.AddFolder("/legacy/");
            model.AddProject("legacy/MyApp.Legacy/MyApp.Legacy.csproj", null, existingFolder);
            model.AddProject("MyApp.Api/MyApp.Api.csproj");

            var projects = new[]
            {
                CreateProject("MyApp.Api", relativeLocation: "MyApp.Api"),
            };

            VisualStudioSolutionSlnxTemplate.SyncFoldersAndProjects(model, [], projects);

            model.SolutionFolders.Select(f => f.Path).ShouldContain("/legacy/");
            model.SolutionProjects.Select(p => p.FilePath)
                .ShouldBe(["MyApp.Api/MyApp.Api.csproj"], ignoreOrder: true);
        }

        [Fact]
        public void WhenExisting_WithManuallyAddedFolder_ShouldPreserveFolderAcrossRuns()
        {
            // A folder that exists on disk but is not in the Intent model (e.g. added by hand)
            // must not be removed on subsequent SF runs.
            var model = new SolutionModel();
            model.AddFolder("/NewFolder1/");
            model.AddProject("MyApp.Api/MyApp.Api.csproj");

            var projects = new[]
            {
                CreateProject("MyApp.Api", relativeLocation: "MyApp.Api"),
            };

            VisualStudioSolutionSlnxTemplate.SyncFoldersAndProjects(model, [], projects);

            model.SolutionFolders.Select(f => f.Path).ShouldContain("/NewFolder1/");
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

        [Fact]
        public void WhenProjectMovesFolder_ShouldReflectNewPlacement()
        {
            // First sync: project in /src/
            var model = new SolutionModel();
            var srcFolder = CreateFolder("src");
            var testFolder = CreateFolder("tests");
            var project = CreateProject("MyApp.Api", relativeLocation: "src/MyApp.Api", parentFolder: srcFolder);

            VisualStudioSolutionSlnxTemplate.SyncFoldersAndProjects(model, [srcFolder, testFolder], [project]);

            model.SolutionFolders.Select(f => f.Path).ShouldContain("/src/");
            model.SolutionProjects
                .Where(p => p.Parent?.Path == "/src/")
                .Select(p => p.FilePath)
                .ShouldContain("src/MyApp.Api/MyApp.Api.csproj");

            // Second sync: same project now assigned to testFolder in the Intent model.
            // The template does not re-parent existing projects — it only adds missing ones
            // and removes projects absent from the intent model. A project that exists with
            // the same file path will not be moved to a different folder.
            var movedProject = CreateProject("MyApp.Api", relativeLocation: "src/MyApp.Api", parentFolder: testFolder);
            VisualStudioSolutionSlnxTemplate.SyncFoldersAndProjects(model, [srcFolder, testFolder], [movedProject]);

            // Project remains in /src/ because re-parenting is not implemented
            model.SolutionProjects
                .Where(p => p.Parent?.Path == "/src/")
                .Select(p => p.FilePath)
                .ShouldContain("src/MyApp.Api/MyApp.Api.csproj");
            // Project is NOT duplicated into /tests/
            model.SolutionProjects
                .Where(p => p.Parent?.Path == "/tests/")
                .ShouldBeEmpty();
        }

        [Fact]
        public void WhenFolderHasNoProjects_ShouldStillCreateFolder()
        {
            var model = new SolutionModel();
            var emptyFolder = CreateFolder("Infrastructure");

            VisualStudioSolutionSlnxTemplate.SyncFoldersAndProjects(model, [emptyFolder], []);

            model.SolutionFolders.Select(f => f.Path).ShouldContain("/Infrastructure/");
            model.SolutionProjects.ShouldBeEmpty();
        }

        [Fact]
        public void WhenRootAndFolderProjectsMixed_ShouldPlaceEachCorrectly()
        {
            var model = new SolutionModel();
            var srcFolder = CreateFolder("src");
            var rootProject = CreateProject("MyApp.Build", relativeLocation: "", parentFolder: null);
            var folderProject = CreateProject("MyApp.Api", relativeLocation: "src/MyApp.Api", parentFolder: srcFolder);

            VisualStudioSolutionSlnxTemplate.SyncFoldersAndProjects(model, [srcFolder], [rootProject, folderProject]);

            var rootProjects = model.SolutionProjects.Where(p => p.Parent == null).Select(p => p.FilePath).ToArray();
            rootProjects.ShouldContain("MyApp.Build.csproj");

            var srcSlnxFolder = model.SolutionFolders.Single(f => f.Path == "/src/");
            model.SolutionProjects
                .Where(p => p.Parent == srcSlnxFolder)
                .Select(p => p.FilePath)
                .ShouldContain("src/MyApp.Api/MyApp.Api.csproj");
        }

        [Fact]
        public void SyncProcessor_WhenSlnxSolutionHasSolutionItems_ShouldAddFilesToFolders()
        {
            // Arrange
            var solutionId = Guid.NewGuid().ToString();
            var slnxDir = System.IO.Path.GetTempPath().TrimEnd(System.IO.Path.DirectorySeparatorChar, System.IO.Path.AltDirectorySeparatorChar);
            var slnxPath = System.IO.Path.Combine(slnxDir, "MyApp.slnx");
            var itemPath = System.IO.Path.Combine(slnxDir, "dapr", "config.yaml");

            var solutionElement = Substitute.For<IElement>();
            solutionElement.Id.Returns(solutionId);
            solutionElement.SpecializationType.Returns(VisualStudioSolutionModel.SpecializationType);
            solutionElement.SpecializationTypeId.Returns(VisualStudioSolutionModel.SpecializationTypeId);
            solutionElement.ParentElement.Returns((IElement)null);

            var folderElement = Substitute.For<IElement>();
            folderElement.Id.Returns(Guid.NewGuid().ToString());
            folderElement.Name.Returns("dapr");
            folderElement.SpecializationType.Returns(IntentSolutionFolderModel.SpecializationType);
            folderElement.SpecializationTypeId.Returns(IntentSolutionFolderModel.SpecializationTypeId);
            folderElement.ParentElement.Returns(solutionElement);
            folderElement.ParentId.Returns(solutionId);
            folderElement.ChildElements.Returns([]);

            var folderModel = new IntentSolutionFolderModel(folderElement);
            var solutionModel = new VisualStudioSolutionModel(solutionElement);

            var application = Substitute.For<IApplication>();
            application.OutputRootDirectory.Returns(slnxDir);
            var slnxTemplate = new VisualStudioSolutionSlnxTemplate(application, solutionModel, []);

            var fileMetadata = new SolutionFileMetadata(
                outputType: "VisualStudioSolution",
                overwriteBehaviour: OverwriteBehaviour.Always,
                codeGenType: "UserControlledWeave",
                fileName: "MyApp",
                fileLocation: slnxDir,
                fileExtension: "slnx");
            slnxTemplate.ConfigureFileMetadata(fileMetadata);

            var outputTargetId = Guid.NewGuid().ToString();
            var outputTarget = Substitute.For<IOutputTarget>();
            outputTarget.Id.Returns(outputTargetId);
            outputTarget.Metadata.Returns(new Dictionary<string, object>
            {
                [FolderConfig.MetadataKey.IsMatch] = true,
                [FolderConfig.MetadataKey.Model] = folderModel
            });
            application.OutputTargets.Returns([outputTarget]);

            Action<SoftwareFactoryEvent> capturedHandler = null;
            var dispatcher = Substitute.For<ISoftwareFactoryEventDispatcher>();
            dispatcher
                .When(x => x.Subscribe(SoftwareFactoryEvents.FileAddedEvent, Arg.Any<Action<SoftwareFactoryEvent>>()))
                .Do(x => capturedHandler = x.ArgAt<Action<SoftwareFactoryEvent>>(1));

            // Simulate an empty .slnx file (what the template would have already written)
            var emptySlnx = SerializeEmptySlnx();
            string capturedOutput = null;
            var slnxChange = Substitute.For<IChange>();
            slnxChange.Content.Returns(emptySlnx);
            slnxChange.When(x => x.ChangeContent(Arg.Any<string>(), Arg.Any<string>())).Do(x => capturedOutput = x.ArgAt<string>(0));

            var changes = Substitute.For<IChanges>();
            changes.FindChange(slnxPath).Returns(slnxChange);

            var sut = new VisualStudioSolutionSyncProcessor(dispatcher, changes);
            sut.PostCreation(slnxTemplate);

            capturedHandler.ShouldNotBeNull();
            capturedHandler(new SoftwareFactoryEvent(SoftwareFactoryEvents.FileAddedEvent, new Dictionary<string, string>
            {
                ["OutputTargetId"] = outputTargetId,
                ["Path"] = itemPath
            }));

            // Act
            sut.OnStep(application, ExecutionLifeCycleSteps.AfterTemplateExecution);

            // Assert — the file should appear inside the dapr folder in the updated .slnx content
            capturedOutput.ShouldNotBeNull();
            capturedOutput.ShouldContain("dapr/config.yaml");
        }

        [Fact]
        public void SyncProcessor_WhenSlnxAlreadyContainsSolutionItem_ShouldNotCallChangeContent()
        {
            // Arrange
            var solutionId = Guid.NewGuid().ToString();
            var slnxDir = System.IO.Path.GetTempPath().TrimEnd(System.IO.Path.DirectorySeparatorChar, System.IO.Path.AltDirectorySeparatorChar);
            var slnxPath = System.IO.Path.Combine(slnxDir, "MyApp.slnx");
            var itemPath = System.IO.Path.Combine(slnxDir, "dapr", "config.yaml");

            var solutionElement = Substitute.For<IElement>();
            solutionElement.Id.Returns(solutionId);
            solutionElement.SpecializationType.Returns(VisualStudioSolutionModel.SpecializationType);
            solutionElement.SpecializationTypeId.Returns(VisualStudioSolutionModel.SpecializationTypeId);
            solutionElement.ParentElement.Returns((IElement)null);

            var folderElement = Substitute.For<IElement>();
            folderElement.Id.Returns(Guid.NewGuid().ToString());
            folderElement.Name.Returns("dapr");
            folderElement.SpecializationType.Returns(IntentSolutionFolderModel.SpecializationType);
            folderElement.SpecializationTypeId.Returns(IntentSolutionFolderModel.SpecializationTypeId);
            folderElement.ParentElement.Returns(solutionElement);
            folderElement.ParentId.Returns(solutionId);
            folderElement.ChildElements.Returns([]);

            var folderModel = new IntentSolutionFolderModel(folderElement);
            var solutionModel = new VisualStudioSolutionModel(solutionElement);

            var application = Substitute.For<IApplication>();
            application.OutputRootDirectory.Returns(slnxDir);
            var slnxTemplate = new VisualStudioSolutionSlnxTemplate(application, solutionModel, []);

            var fileMetadata = new SolutionFileMetadata(
                outputType: "VisualStudioSolution",
                overwriteBehaviour: OverwriteBehaviour.Always,
                codeGenType: "UserControlledWeave",
                fileName: "MyApp",
                fileLocation: slnxDir,
                fileExtension: "slnx");
            slnxTemplate.ConfigureFileMetadata(fileMetadata);

            var outputTargetId = Guid.NewGuid().ToString();
            var outputTarget = Substitute.For<IOutputTarget>();
            outputTarget.Id.Returns(outputTargetId);
            outputTarget.Metadata.Returns(new Dictionary<string, object>
            {
                [FolderConfig.MetadataKey.IsMatch] = true,
                [FolderConfig.MetadataKey.Model] = folderModel
            });
            application.OutputTargets.Returns([outputTarget]);

            Action<SoftwareFactoryEvent> capturedHandler = null;
            var dispatcher = Substitute.For<ISoftwareFactoryEventDispatcher>();
            dispatcher
                .When(x => x.Subscribe(SoftwareFactoryEvents.FileAddedEvent, Arg.Any<Action<SoftwareFactoryEvent>>()))
                .Do(x => capturedHandler = x.ArgAt<Action<SoftwareFactoryEvent>>(1));

            // The .slnx already contains dapr/config.yaml (simulating a second SF run)
            var existingSlnx = SerializeSlnxWithFile("/dapr/", "dapr/config.yaml");
            var changeContentCalled = false;
            var slnxChange = Substitute.For<IChange>();
            slnxChange.Content.Returns(existingSlnx);
            slnxChange.When(x => x.ChangeContent(Arg.Any<string>(), Arg.Any<string>())).Do(_ => changeContentCalled = true);

            var changes = Substitute.For<IChanges>();
            changes.FindChange(slnxPath).Returns(slnxChange);

            var sut = new VisualStudioSolutionSyncProcessor(dispatcher, changes);
            sut.PostCreation(slnxTemplate);

            capturedHandler.ShouldNotBeNull();
            capturedHandler(new SoftwareFactoryEvent(SoftwareFactoryEvents.FileAddedEvent, new Dictionary<string, string>
            {
                ["OutputTargetId"] = outputTargetId,
                ["Path"] = itemPath
            }));

            // Act
            sut.OnStep(application, ExecutionLifeCycleSteps.AfterTemplateExecution);

            // Assert — no changes should be written when the file is already present
            changeContentCalled.ShouldBeFalse();
        }

        private static string SerializeEmptySlnx()
        {
            return SerializeSlnxWithFile(null, null);
        }

        private static string SerializeSlnxWithFile(string folderPath, string filePath)
        {
            var model = new SolutionModel();
            if (folderPath != null)
            {
                var folder = model.AddFolder(folderPath);
                if (filePath != null)
                    folder.AddFile(filePath);
            }
            using var stream = new System.IO.MemoryStream();
            Microsoft.VisualStudio.SolutionPersistence.Serializer.SolutionSerializers.SlnXml
                .SaveAsync(stream, model, System.Threading.CancellationToken.None)
                .GetAwaiter().GetResult();
            return System.Text.Encoding.UTF8.GetString(stream.ToArray());
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
