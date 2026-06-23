using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using Intent.Engine;
using Intent.Eventing;
using Intent.Metadata.Models;
using Intent.Modules.Common.Plugins;
using Intent.Plugins.FactoryExtensions;
using Intent.Modules.Common.Templates;
using Intent.Modules.Constants;
using Intent.Modules.VisualStudio.Projects.Api;
using Intent.Modules.VisualStudio.Projects.FactoryExtensions;
using Intent.Modules.VisualStudio.Projects.Templates.VisualStudioSolution;
using Intent.Templates;
using Microsoft.DotNet.Cli.Sln.Internal;
using Microsoft.VisualStudio.SolutionPersistence.Model;
using Microsoft.VisualStudio.SolutionPersistence.Serializer;
using NSubstitute;
using Shouldly;
using Xunit;
using IntentSolutionFolderModel = Intent.Modules.VisualStudio.Projects.Api.SolutionFolderModel;
using OverwriteBehaviour = Intent.Templates.OverwriteBehaviour;

namespace Intent.Modules.VisualStudio.Projects.Tests.Sync
{
    public class VisualStudioSolutionSyncProcessorTests
    {
        private static readonly string WorkDir = Path.GetTempPath()
            .TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);

        // ── shared wiring helpers ─────────────────────────────────────────────

        private static (IApplication Application, string OutputTargetId) MakeApplication(string solutionId)
        {
            var solutionElement = Substitute.For<IElement>();
            solutionElement.Id.Returns(solutionId);
            solutionElement.SpecializationType.Returns(VisualStudioSolutionModel.SpecializationType);
            solutionElement.SpecializationTypeId.Returns(VisualStudioSolutionModel.SpecializationTypeId);
            solutionElement.ParentElement.Returns((IElement)null);

            var folderElement = Substitute.For<IElement>();
            var folderId = Guid.NewGuid().ToString();
            folderElement.Id.Returns(folderId);
            folderElement.Name.Returns("dapr");
            folderElement.SpecializationType.Returns(IntentSolutionFolderModel.SpecializationType);
            folderElement.SpecializationTypeId.Returns(IntentSolutionFolderModel.SpecializationTypeId);
            folderElement.ParentElement.Returns(solutionElement);
            folderElement.ParentId.Returns(solutionId);
            folderElement.ChildElements.Returns([]);

            var folderModel = new IntentSolutionFolderModel(folderElement);
            var outputTargetId = Guid.NewGuid().ToString();
            var outputTarget = Substitute.For<IOutputTarget>();
            outputTarget.Id.Returns(outputTargetId);
            outputTarget.Metadata.Returns(new Dictionary<string, object>
            {
                [FolderConfig.MetadataKey.IsMatch] = true,
                [FolderConfig.MetadataKey.Model] = folderModel
            });

            var application = Substitute.For<IApplication>();
            application.OutputRootDirectory.Returns(WorkDir);
            application.OutputTargets.Returns([outputTarget]);

            return (application, outputTargetId);
        }

        private static (ISoftwareFactoryEventDispatcher Dispatcher, Action<SoftwareFactoryEvent> FireAdded) MakeDispatcher(string outputTargetId)
        {
            Action<SoftwareFactoryEvent> capturedHandler = null;
            var dispatcher = Substitute.For<ISoftwareFactoryEventDispatcher>();
            dispatcher
                .When(x => x.Subscribe(SoftwareFactoryEvents.FileAddedEvent, Arg.Any<Action<SoftwareFactoryEvent>>()))
                .Do(x => capturedHandler = x.ArgAt<Action<SoftwareFactoryEvent>>(1));

            void FireAdded() => capturedHandler!(new SoftwareFactoryEvent(
                SoftwareFactoryEvents.FileAddedEvent, new Dictionary<string, string>
                {
                    ["OutputTargetId"] = outputTargetId,
                    ["Path"] = Path.Combine(WorkDir, "dapr", "config.yaml")
                }));

            return (dispatcher, _ => FireAdded());
        }

        private static (IChanges Changes, Func<string> CapturedOutput) MakeChanges(string slnPath, string content)
        {
            string capturedOutput = null;
            var change = Substitute.For<IChange>();
            change.Content.Returns(content);
            change.When(x => x.ChangeContent(Arg.Any<string>(), Arg.Any<string>())).Do(x => capturedOutput = x.ArgAt<string>(0));

            var changes = Substitute.For<IChanges>();
            changes.FindChange(slnPath).Returns(change);

            return (changes, () => capturedOutput);
        }

        private static string EmptySlnx()
        {
            using var stream = new MemoryStream();
            SolutionSerializers.SlnXml.SaveAsync(stream, new SolutionModel(), CancellationToken.None)
                .GetAwaiter().GetResult();
            return Encoding.UTF8.GetString(stream.ToArray());
        }

        private static string SlnxWithFile(string folderPath, string filePath)
        {
            var model = new SolutionModel();
            model.AddFolder(folderPath).AddFile(filePath);
            using var stream = new MemoryStream();
            SolutionSerializers.SlnXml.SaveAsync(stream, model, CancellationToken.None)
                .GetAwaiter().GetResult();
            return Encoding.UTF8.GetString(stream.ToArray());
        }

        // ── .slnx processor ───────────────────────────────────────────────────

        [Fact]
        public void Slnx_WhenFileAddedToEmptySolution_ShouldUpdateContent()
        {
            var solutionId = Guid.NewGuid().ToString();
            var slnxPath = Path.Combine(WorkDir, "MyApp.slnx");

            var (application, outputTargetId) = MakeApplication(solutionId);
            var (dispatcher, fireAdded) = MakeDispatcher(outputTargetId);
            var (changes, capturedOutput) = MakeChanges(slnxPath, EmptySlnx());

            var solutionModel = new VisualStudioSolutionModel(
                BuildSolutionElement(solutionId));
            var template = new VisualStudioSolutionSlnxTemplate(application, solutionModel, []);
            template.ConfigureFileMetadata(new SolutionFileMetadata(
                "VisualStudioSolution", OverwriteBehaviour.Always, "UserControlledWeave",
                "MyApp", WorkDir, "slnx"));

            var sut = new VisualStudioSolutionSyncProcessor(dispatcher, changes);
            sut.PostCreation(template);

            fireAdded(null);
            sut.OnStep(application, ExecutionLifeCycleSteps.AfterTemplateExecution);

            capturedOutput().ShouldNotBeNull();
            capturedOutput().ShouldContain("dapr/config.yaml");
        }

        [Fact]
        public void Slnx_WhenFileAlreadyPresent_ShouldNotCallChangeContent()
        {
            var solutionId = Guid.NewGuid().ToString();
            var slnxPath = Path.Combine(WorkDir, "MyApp.slnx");

            var (application, outputTargetId) = MakeApplication(solutionId);
            var (dispatcher, fireAdded) = MakeDispatcher(outputTargetId);
            var (changes, capturedOutput) = MakeChanges(slnxPath, SlnxWithFile("/dapr/", "dapr/config.yaml"));

            var solutionModel = new VisualStudioSolutionModel(BuildSolutionElement(solutionId));
            var template = new VisualStudioSolutionSlnxTemplate(application, solutionModel, []);
            template.ConfigureFileMetadata(new SolutionFileMetadata(
                "VisualStudioSolution", OverwriteBehaviour.Always, "UserControlledWeave",
                "MyApp", WorkDir, "slnx"));

            var sut = new VisualStudioSolutionSyncProcessor(dispatcher, changes);
            sut.PostCreation(template);

            fireAdded(null);
            sut.OnStep(application, ExecutionLifeCycleSteps.AfterTemplateExecution);

            capturedOutput().ShouldBeNull();
        }

        // ── .sln processor ────────────────────────────────────────────────────

        [Fact]
        public void Sln_WhenFileAddedToEmptySolution_ShouldUpdateContent()
        {
            var solutionId = Guid.NewGuid().ToString();
            var slnPath = Path.Combine(WorkDir, "MyApp.sln");

            var (application, outputTargetId) = MakeApplication(solutionId);
            var (dispatcher, fireAdded) = MakeDispatcher(outputTargetId);
            var emptySln = SlnFile.CreateEmpty(slnPath).Generate();
            var (changes, capturedOutput) = MakeChanges(slnPath, emptySln);

            var solutionModel = new VisualStudioSolutionModel(BuildSolutionElement(solutionId));
            var template = new VisualStudioSolutionTemplate(application, solutionModel, []);
            template.ConfigureFileMetadata(new SolutionFileMetadata(
                "VisualStudioSolution", OverwriteBehaviour.Always, "UserControlledWeave",
                "MyApp", WorkDir));

            var sut = new VisualStudioSolutionSyncProcessor(dispatcher, changes);
            sut.PostCreation(template);

            fireAdded(null);
            sut.OnStep(application, ExecutionLifeCycleSteps.AfterTemplateExecution);

            capturedOutput().ShouldNotBeNull();
            capturedOutput().ShouldContain("dapr\\config.yaml");
        }

        [Fact]
        public void Sln_WhenFileAlreadyPresent_ShouldNotCallChangeContent()
        {
            // Simulate a genuine second SF run: first run produces the .sln, second run
            // must not re-write it. The folder GUID in the .sln comes from the Intent model,
            // so we must use the first run's output as the starting content for the second run.
            var solutionId = Guid.NewGuid().ToString();
            var slnPath = Path.Combine(WorkDir, "MyApp.sln");

            var (application, outputTargetId) = MakeApplication(solutionId);
            var solutionModel = new VisualStudioSolutionModel(BuildSolutionElement(solutionId));
            var metadata = new SolutionFileMetadata(
                "VisualStudioSolution", OverwriteBehaviour.Always, "UserControlledWeave",
                "MyApp", WorkDir);

            // ── First run (adds the file) ──────────────────────────────────────
            var (dispatcher1, fireAdded1) = MakeDispatcher(outputTargetId);
            var (changes1, firstRunOutput) = MakeChanges(slnPath, SlnFile.CreateEmpty(slnPath).Generate());

            var template1 = new VisualStudioSolutionTemplate(application, solutionModel, []);
            template1.ConfigureFileMetadata(metadata);
            var sut1 = new VisualStudioSolutionSyncProcessor(dispatcher1, changes1);
            sut1.PostCreation(template1);
            fireAdded1(null);
            sut1.OnStep(application, ExecutionLifeCycleSteps.AfterTemplateExecution);

            firstRunOutput().ShouldNotBeNull("first run must produce an updated .sln");

            // ── Second run (file already present — should be no-op) ────────────
            var (dispatcher2, fireAdded2) = MakeDispatcher(outputTargetId);
            var (changes2, secondRunOutput) = MakeChanges(slnPath, firstRunOutput());

            var template2 = new VisualStudioSolutionTemplate(application, solutionModel, []);
            template2.ConfigureFileMetadata(metadata);
            var sut2 = new VisualStudioSolutionSyncProcessor(dispatcher2, changes2);
            sut2.PostCreation(template2);
            fireAdded2(null);
            sut2.OnStep(application, ExecutionLifeCycleSteps.AfterTemplateExecution);

            secondRunOutput().ShouldBeNull("second run must not re-write the .sln when nothing changed");
        }

        // ── element builder ───────────────────────────────────────────────────

        private static IElement BuildSolutionElement(string id)
        {
            var el = Substitute.For<IElement>();
            el.Id.Returns(id);
            el.SpecializationType.Returns(VisualStudioSolutionModel.SpecializationType);
            el.SpecializationTypeId.Returns(VisualStudioSolutionModel.SpecializationTypeId);
            el.ParentElement.Returns((IElement)null);
            return el;
        }
    }
}
