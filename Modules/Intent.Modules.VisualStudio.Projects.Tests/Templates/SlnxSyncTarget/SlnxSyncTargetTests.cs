using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using Intent.Metadata.Models;
using Intent.Modules.Common.Templates;
using Intent.Modules.Constants;
using Intent.Modules.VisualStudio.Projects.Api;
using Intent.Modules.VisualStudio.Projects.FactoryExtensions;
using Intent.Modules.VisualStudio.Projects.Templates.VisualStudioSolution;
using Intent.Templates;
using Microsoft.VisualStudio.SolutionPersistence.Model;
using Microsoft.VisualStudio.SolutionPersistence.Serializer;
using NSubstitute;
using Shouldly;
using Xunit;
using IntentSolutionFolderModel = Intent.Modules.VisualStudio.Projects.Api.SolutionFolderModel;
using OverwriteBehaviour = Intent.Templates.OverwriteBehaviour;

namespace Intent.Modules.VisualStudio.Projects.Tests.Templates.SlnxSyncTarget
{
    public class SlnxSyncTargetTests
    {
        private static readonly string SlnxDir = Path.GetTempPath()
            .TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);

        // ── factory ───────────────────────────────────────────────────────────

        private static IVsSolutionSyncTarget CreateTarget()
        {
            var metadata = new SolutionFileMetadata(
                outputType: "VisualStudioSolution",
                overwriteBehaviour: OverwriteBehaviour.Always,
                codeGenType: "UserControlledWeave",
                fileName: "MyApp",
                fileLocation: SlnxDir,
                fileExtension: "slnx");

            return new FactoryExtensions.SlnxSyncTarget("model-id", () => metadata);
        }

        private static string EmptySlnx() => Serialize(new SolutionModel());

        private static string SlnxWithFile(string folderPath, string filePath)
        {
            var model = new SolutionModel();
            var folder = model.AddFolder(folderPath);
            folder.AddFile(filePath);
            return Serialize(model);
        }

        private static string Serialize(SolutionModel model)
        {
            using var stream = new MemoryStream();
            SolutionSerializers.SlnXml.SaveAsync(stream, model, CancellationToken.None)
                .GetAwaiter().GetResult();
            return Encoding.UTF8.GetString(stream.ToArray());
        }

        private static IReadOnlyCollection<IntentSolutionFolderModel> MakeFolderPath(params string[] names)
        {
            var folders = new List<IntentSolutionFolderModel>();
            foreach (var name in names)
            {
                var el = Substitute.For<IElement>();
                el.Id.Returns(Guid.NewGuid().ToString());
                el.Name.Returns(name);
                el.ChildElements.Returns([]);
                el.SpecializationType.Returns(IntentSolutionFolderModel.SpecializationType);
                el.SpecializationTypeId.Returns(IntentSolutionFolderModel.SpecializationTypeId);
                el.ParentElement.Returns((IElement)null);
                el.ParentId.Returns((string)null);
                folders.Add(new IntentSolutionFolderModel(el));
            }
            return folders;
        }

        private static SolutionItemAction AddAction(
            string physicalPath,
            IReadOnlyCollection<IntentSolutionFolderModel> folderPath = null) =>
            new()
            {
                EventIdentifier = SoftwareFactoryEvents.FileAddedEvent,
                PhysicalPath = physicalPath,
                FolderPath = folderPath ?? (IReadOnlyCollection<IntentSolutionFolderModel>)[]
            };

        private static SolutionItemAction RemoveAction(
            string physicalPath,
            IReadOnlyCollection<IntentSolutionFolderModel> folderPath = null) =>
            new()
            {
                EventIdentifier = SoftwareFactoryEvents.FileRemovedEvent,
                PhysicalPath = physicalPath,
                FolderPath = folderPath ?? (IReadOnlyCollection<IntentSolutionFolderModel>)[]
            };

        // ── FileAddedEvent ────────────────────────────────────────────────────

        [Fact]
        public void FileAdded_WhenSlnxIsEmpty_ShouldCreateFolderAndAddFile()
        {
            var target = CreateTarget();
            var itemPath = Path.Combine(SlnxDir, "dapr", "config.yaml");

            var result = target.ApplySolutionItems(EmptySlnx(), [AddAction(itemPath, MakeFolderPath("dapr"))]);

            result.ShouldNotBeNull();
            result.ShouldContain("dapr/config.yaml");
        }

        [Fact]
        public void FileAdded_WhenFolderAlreadyExists_ShouldAddFileWithoutDuplicatingFolder()
        {
            var target = CreateTarget();
            var existingSlnx = SlnxWithFile("/dapr/", "dapr/other.yaml");
            var itemPath = Path.Combine(SlnxDir, "dapr", "config.yaml");

            var result = target.ApplySolutionItems(existingSlnx, [AddAction(itemPath, MakeFolderPath("dapr"))]);

            result.ShouldNotBeNull();
            result.ShouldContain("dapr/config.yaml");
            result.ShouldContain("dapr/other.yaml");
            result.Split("<Folder").Length.ShouldBe(2, "only one /dapr/ folder should exist");
        }

        [Fact]
        public void FileAdded_WhenFileAlreadyPresent_ShouldReturnNull()
        {
            var target = CreateTarget();
            var existingSlnx = SlnxWithFile("/dapr/", "dapr/config.yaml");
            var itemPath = Path.Combine(SlnxDir, "dapr", "config.yaml");

            var result = target.ApplySolutionItems(existingSlnx, [AddAction(itemPath, MakeFolderPath("dapr"))]);

            result.ShouldBeNull();
        }

        [Fact]
        public void FileAdded_ToDeepNestedFolder_ShouldAddFileInCorrectPath()
        {
            var target = CreateTarget();
            var itemPath = Path.Combine(SlnxDir, "infra", "kubernetes", "deploy.yaml");

            var result = target.ApplySolutionItems(EmptySlnx(), [AddAction(itemPath, MakeFolderPath("infra", "kubernetes"))]);

            result.ShouldNotBeNull();
            result.ShouldContain("infra/kubernetes/deploy.yaml");
        }

        [Fact]
        public void FileAdded_MultipleFilesAcrossDifferentFolders_ShouldAddAll()
        {
            var target = CreateTarget();
            var path1 = Path.Combine(SlnxDir, "dapr", "config.yaml");
            var path2 = Path.Combine(SlnxDir, "docker", "compose.yml");

            var result = target.ApplySolutionItems(EmptySlnx(), [
                AddAction(path1, MakeFolderPath("dapr")),
                AddAction(path2, MakeFolderPath("docker"))
            ]);

            result.ShouldNotBeNull();
            result.ShouldContain("dapr/config.yaml");
            result.ShouldContain("docker/compose.yml");
        }

        [Fact]
        public void FileAdded_MultipleFilesToSameFolder_ShouldAddAllUnderOneFolder()
        {
            var target = CreateTarget();
            var path1 = Path.Combine(SlnxDir, "dapr", "config.yaml");
            var path2 = Path.Combine(SlnxDir, "dapr", "pubsub.yaml");

            var result = target.ApplySolutionItems(EmptySlnx(), [
                AddAction(path1, MakeFolderPath("dapr")),
                AddAction(path2, MakeFolderPath("dapr"))
            ]);

            result.ShouldNotBeNull();
            result.ShouldContain("dapr/config.yaml");
            result.ShouldContain("dapr/pubsub.yaml");
            result.Split("<Folder").Length.ShouldBe(2, "only one /dapr/ folder should be created");
        }

        // ── FileRemovedEvent ──────────────────────────────────────────────────

        [Fact]
        public void FileRemoved_WhenFilePresent_ShouldRemoveAndReturnUpdatedContent()
        {
            var target = CreateTarget();
            var existingSlnx = SlnxWithFile("/dapr/", "dapr/config.yaml");
            var itemPath = Path.Combine(SlnxDir, "dapr", "config.yaml");

            var result = target.ApplySolutionItems(existingSlnx, [RemoveAction(itemPath, MakeFolderPath("dapr"))]);

            result.ShouldNotBeNull();
            result.ShouldNotContain("dapr/config.yaml");
        }

        [Fact]
        public void FileRemoved_WhenFileNotPresent_ShouldReturnNull()
        {
            var target = CreateTarget();
            var itemPath = Path.Combine(SlnxDir, "dapr", "config.yaml");

            var result = target.ApplySolutionItems(EmptySlnx(), [RemoveAction(itemPath, MakeFolderPath("dapr"))]);

            result.ShouldBeNull();
        }

        [Fact]
        public void FileRemoved_WhenOneOfManyFilesInFolder_ShouldRemoveOnlyTargetFile()
        {
            var target = CreateTarget();
            var model = new SolutionModel();
            var folder = model.AddFolder("/dapr/");
            folder.AddFile("dapr/config.yaml");
            folder.AddFile("dapr/pubsub.yaml");
            var existingSlnx = Serialize(model);
            var itemPath = Path.Combine(SlnxDir, "dapr", "config.yaml");

            var result = target.ApplySolutionItems(existingSlnx, [RemoveAction(itemPath, MakeFolderPath("dapr"))]);

            result.ShouldNotBeNull();
            result.ShouldNotContain("dapr/config.yaml");
            result.ShouldContain("dapr/pubsub.yaml");
        }
    }
}
