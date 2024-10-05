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
