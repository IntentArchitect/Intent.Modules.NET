using System;
using System.Collections.Generic;
using Intent.Engine;
using Intent.Eventing;
using Intent.Modules.Common.CSharp.VisualStudio;
using Intent.Modules.Common.VisualStudio;
using Intent.Modules.Constants;
using Intent.Modules.VisualStudio.Projects.Sync;
using Intent.Modules.VisualStudio.Projects.Templates;
using Intent.Utils;
using NSubstitute;
using Shouldly;
using Xunit;

namespace Intent.Modules.VisualStudio.Projects.Tests.Sync
{
    public class CoreProjectSyncProcessorTests
    {
        [Fact]
        public void ItShouldWork()
        {
            // Arrange
            const string content = @"<Project Sdk=""Microsoft.NET.Sdk"">

  <PropertyGroup>
    <TargetFramework>net6.0</TargetFramework>
  </PropertyGroup>

</Project>";
            var events = new List<SoftwareFactoryEvent>
            {
                new(SoftwareFactoryEvents.FileAddedEvent, new Dictionary<string, string>
                {
                    ["Path"] = "/Folder/File.cs",
                    [CustomMetadataKeys.ItemType] = "EmbeddedResource",
                    [CustomMetadataKeys.RemoveItemType] = "Compile",
                    [CustomMetadataKeys.MsBuildFileItemGenerationBehaviour] = "Always"
                })
            };

            // Act
            var updatedContent = RunProcessor(content, events);

            // Assert
            updatedContent.ShouldBe(@"<Project Sdk=""Microsoft.NET.Sdk"">

  <PropertyGroup>
    <TargetFramework>net6.0</TargetFramework>
  </PropertyGroup>

  <ItemGroup>
    <Compile Remove=""File.cs"" />
  </ItemGroup>

  <ItemGroup>
    <EmbeddedResource Include=""File.cs"" />
  </ItemGroup>

</Project>");
        }

        // What the sync writes for a managed Worker project's appsettings.json (RemoveItemType "Content"
        // plus CopyToOutputDirectory): the SDK's own Content item removed, then re-included.
        private const string WorkerWithAppSettingsItems = @"<Project Sdk=""Microsoft.NET.Sdk.Worker"">

  <PropertyGroup>
    <TargetFramework>net8.0</TargetFramework>
  </PropertyGroup>

  <ItemGroup>
    <Content Remove=""appsettings.json"" />
  </ItemGroup>

  <ItemGroup>
    <Content Include=""appsettings.json"">
      <CopyToOutputDirectory>PreserveNewest</CopyToOutputDirectory>
    </Content>
  </ItemGroup>

</Project>";

        private const string WorkerWithoutItems = @"<Project Sdk=""Microsoft.NET.Sdk.Worker"">

  <PropertyGroup>
    <TargetFramework>net8.0</TargetFramework>
  </PropertyGroup>

</Project>";

        [Fact]
        public void WhenHandsOff_ExistingItemsAreKeptExactlyAsTheyAre()
        {
            var updatedContent = RunProcessor(WorkerWithAppSettingsItems, [HandsOffAppSettingsAdded()]);

            updatedContent.ShouldBe(WorkerWithAppSettingsItems);
        }

        [Fact]
        public void WhenHandsOff_NoItemsAreAdded()
        {
            var updatedContent = RunProcessor(WorkerWithoutItems, [HandsOffAppSettingsAdded()]);

            updatedContent.ShouldBe(WorkerWithoutItems);
        }

        [Fact]
        public void WhenAFileNoLongerRequestsItems_ItsRemoveAndReIncludePairAreBothRemoved()
        {
            // Dropping only the re-include left <Content Remove> behind, so appsettings.json stopped
            // being Content at all (and was no longer copied to the output), plus an empty ItemGroup.
            var events = new List<SoftwareFactoryEvent>
            {
                new(SoftwareFactoryEvents.FileAddedEvent, new Dictionary<string, string>
                {
                    ["Path"] = "/Folder/appsettings.json"
                })
            };

            var updatedContent = RunProcessor(WorkerWithAppSettingsItems, events);

            updatedContent.ShouldBe(WorkerWithoutItems);
        }

        [Fact]
        public void WhenAFileNoLongerRequestsItems_ADeliberateExclusionIsKept()
        {
            const string content = @"<Project Sdk=""Microsoft.NET.Sdk"">

  <PropertyGroup>
    <TargetFramework>net8.0</TargetFramework>
  </PropertyGroup>

  <ItemGroup>
    <Compile Remove=""File.cs"" />
  </ItemGroup>

</Project>";
            var events = new List<SoftwareFactoryEvent>
            {
                new(SoftwareFactoryEvents.FileAddedEvent, new Dictionary<string, string>
                {
                    ["Path"] = "/Folder/File.cs"
                })
            };

            var updatedContent = RunProcessor(content, events);

            updatedContent.ShouldBe(content);
        }

        private static SoftwareFactoryEvent HandsOffAppSettingsAdded() =>
            new(SoftwareFactoryEvents.FileAddedEvent, new Dictionary<string, string>
            {
                ["Path"] = "/Folder/appsettings.json",
                [FileAddedData.LeaveProjectItemsUnchangedKey] = "true"
            });

        private static string RunProcessor(string content, List<SoftwareFactoryEvent> events)
        {
            var sut = CreateSut(content, x => content = x);
            sut.Process(events);
            return content;
        }

        private static CoreProjectSyncProcessor CreateSut(string content, Action<string> onSetContent)
        {
            Logging.SetTracing(Substitute.For<ITracing>());

            var outputTarget = Substitute.For<IOutputTarget>();
            outputTarget.Metadata.Returns(new Dictionary<string, object>
            {
                ["VS.Dependencies"] = new List<IOutputTarget>(),
                ["VS.References"] = new List<IAssemblyReference>(),
                ["VS.FrameworkReferences"] = new HashSet<string>()
            });

            var template = Substitute.For<IVisualStudioProjectTemplate>();
            template.FilePath.Returns(@"/Folder/Project.csproj");
            template.OutputTarget.Returns(outputTarget);

            var sfEventDispatcher = Substitute.For<ISoftwareFactoryEventDispatcher>();

            var change = Substitute.For<IChange>();
            change.Content.Returns(content);
            change.When(x => x.ChangeContent(Arg.Any<string>())).Do(x =>
            {
                onSetContent(x.Arg<string>());
            });

            var changes = Substitute.For<IChanges>();
            changes.FindChange(Arg.Any<string>()).ReturnsForAnyArgs(change);

            var sut = new CoreProjectSyncProcessor(template, sfEventDispatcher, changes);
            return sut;
        }
    }
}
