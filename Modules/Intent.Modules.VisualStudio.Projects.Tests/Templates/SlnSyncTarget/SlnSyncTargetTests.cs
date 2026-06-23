using System;
using System.Collections.Generic;
using System.IO;
using Intent.Metadata.Models;
using Intent.Modules.Common.Templates;
using Intent.Modules.Constants;
using Intent.Modules.VisualStudio.Projects.Api;
using Intent.Modules.VisualStudio.Projects.FactoryExtensions;
using Intent.Modules.VisualStudio.Projects.Templates.VisualStudioSolution;
using Intent.Templates;
using Microsoft.DotNet.Cli.Sln.Internal;
using NSubstitute;
using Shouldly;
using Xunit;
using IntentSolutionFolderModel = Intent.Modules.VisualStudio.Projects.Api.SolutionFolderModel;
using OverwriteBehaviour = Intent.Templates.OverwriteBehaviour;

namespace Intent.Modules.VisualStudio.Projects.Tests.Templates.SlnSyncTarget
{
    public class SlnSyncTargetTests
    {
        private static readonly string SlnDir = Path.GetTempPath()
            .TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);

        private static readonly string SlnPath = Path.Combine(SlnDir, "MyApp.sln");

        // ── factory ───────────────────────────────────────────────────────────

        private static IVsSolutionSyncTarget CreateTarget()
        {
            var metadata = new SolutionFileMetadata(
                outputType: "VisualStudioSolution",
                overwriteBehaviour: OverwriteBehaviour.Always,
                codeGenType: "UserControlledWeave",
                fileName: "MyApp",
                fileLocation: SlnDir);

            return new FactoryExtensions.SlnSyncTarget("model-id", () => metadata);
        }

        private static string EmptySln() => SlnFile.CreateEmpty(SlnPath).Generate();

        private static string SlnWithSolutionItem(string physicalPath)
        {
            var sln = SlnFile.CreateEmpty(SlnPath);
            sln.AddSolutionItem(
                parentProject: null,
                solutionItemPhysicalPath: physicalPath,
                relativeOutputPathPrefix: null,
                hasMaterializedFolder: false);
            return sln.Generate();
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
        public void FileAdded_AtRootLevel_ShouldCreateSolutionFolderWithItem()
        {
            var target = CreateTarget();
            var itemPath = Path.Combine(SlnDir, "dapr", "config.yaml");

            var result = target.ApplySolutionItems(EmptySln(), [AddAction(itemPath)]);

            result.ShouldNotBeNull();
            result.ShouldContain("dapr\\config.yaml");
        }

        [Fact]
        public void FileAdded_WithFolderPath_ShouldNestUnderIntentSolutionFolder()
        {
            var target = CreateTarget();
            var itemPath = Path.Combine(SlnDir, "dapr", "config.yaml");

            var result = target.ApplySolutionItems(EmptySln(), [AddAction(itemPath, MakeFolderPath("Infrastructure"))]);

            result.ShouldNotBeNull();
            result.ShouldContain("dapr\\config.yaml");
            result.ShouldContain("Infrastructure");
        }

        [Fact]
        public void FileAdded_WhenFileAlreadyPresent_ShouldReturnNull()
        {
            var target = CreateTarget();
            var itemPath = Path.Combine(SlnDir, "dapr", "config.yaml");
            var existingSln = SlnWithSolutionItem(itemPath);

            var result = target.ApplySolutionItems(existingSln, [AddAction(itemPath)]);

            result.ShouldBeNull();
        }

        [Fact]
        public void FileAdded_MultipleFiles_ShouldAddAll()
        {
            var target = CreateTarget();
            var path1 = Path.Combine(SlnDir, "dapr", "config.yaml");
            var path2 = Path.Combine(SlnDir, "docker", "compose.yml");

            var result = target.ApplySolutionItems(EmptySln(), [
                AddAction(path1),
                AddAction(path2)
            ]);

            result.ShouldNotBeNull();
            result.ShouldContain("dapr\\config.yaml");
            result.ShouldContain("docker\\compose.yml");
        }

        // ── Disk item preservation ────────────────────────────────────────────

        [Fact]
        public void FileAdded_WhenDiskHasManuallyAddedItem_ShouldPreserveIt()
        {
            var target = CreateTarget();
            var intentPath = Path.Combine(SlnDir, "dapr", "config.yaml");
            var diskContent = SlnWithSolutionItem(Path.Combine(SlnDir, "docs", "readme.md"));

            var result = target.ApplySolutionItems(EmptySln(), [AddAction(intentPath)], diskContent);

            result.ShouldNotBeNull();
            result.ShouldContain("dapr\\config.yaml");
            result.ShouldContain("docs\\readme.md");
        }

        [Fact]
        public void FileAdded_WhenDiskHasManualItemBeingExplicitlyRemoved_ShouldRemoveIt()
        {
            var target = CreateTarget();
            var intentPath = Path.Combine(SlnDir, "dapr", "config.yaml");
            var manualItemPath = Path.Combine(SlnDir, "docs", "readme.md");
            var diskContent = SlnWithSolutionItem(manualItemPath);

            var result = target.ApplySolutionItems(
                EmptySln(),
                [AddAction(intentPath), RemoveAction(manualItemPath)],
                diskContent);

            result.ShouldNotBeNull();
            result.ShouldContain("dapr\\config.yaml");
            result.ShouldNotContain("docs\\readme.md");
        }

        [Fact]
        public void WhenDiskHasManualItem_NoIntentActions_ShouldPreserveItAndReturnUpdatedContent()
        {
            var target = CreateTarget();
            var diskContent = SlnWithSolutionItem(Path.Combine(SlnDir, "docs", "readme.md"));

            var result = target.ApplySolutionItems(EmptySln(), [], diskContent);

            result.ShouldNotBeNull();
            result.ShouldContain("docs\\readme.md");
        }

        [Fact]
        public void WhenDiskAndTemplateContentMatch_ShouldReturnNull()
        {
            var target = CreateTarget();
            var content = SlnWithSolutionItem(Path.Combine(SlnDir, "docs", "readme.md"));

            var result = target.ApplySolutionItems(content, [], content);

            result.ShouldBeNull();
        }

        // ── FileRemovedEvent ──────────────────────────────────────────────────

        [Fact]
        public void FileRemoved_WhenFilePresent_ShouldRemoveAndReturnUpdatedContent()
        {
            var target = CreateTarget();
            var itemPath = Path.Combine(SlnDir, "dapr", "config.yaml");
            var existingSln = SlnWithSolutionItem(itemPath);

            var result = target.ApplySolutionItems(existingSln, [RemoveAction(itemPath)]);

            result.ShouldNotBeNull();
            result.ShouldNotContain("dapr\\config.yaml");
        }

        [Fact]
        public void FileRemoved_WhenFileNotPresent_ShouldReturnNull()
        {
            var target = CreateTarget();
            var itemPath = Path.Combine(SlnDir, "dapr", "config.yaml");

            var result = target.ApplySolutionItems(EmptySln(), [RemoveAction(itemPath)]);

            result.ShouldBeNull();
        }
    }
}
