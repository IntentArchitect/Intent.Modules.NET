using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
using Intent.Modules.VisualStudio.Projects.Api;
using Intent.Modules.VisualStudio.Projects.SolutionFile;
using Intent.Modules.VisualStudio.Projects.Templates.VisualStudio2015Solution;
using NSubstitute;
using Shouldly;
using Xunit;

namespace Intent.Modules.VisualStudio.Projects.Tests.SolutionFile
{
    public class SlnFileTests
    {
        public class DescribeGetOrCreateProjectNode
        {
            /// <summary>
            /// If any part of the path is just ".", we want that stripped out.
            /// </summary>
            [Fact]
            public void WhenNew_ItShouldRemoveCurrentFolderComponents()
            {
                // Arrange
                var slnFile = new SlnFile(@"C:\Folder1\Folder2\SolutionFile.sln", "");

                // Act
                slnFile.GetOrCreateProjectNode(
                    typeId: "TYPE_ID",
                    name: "NAME",
                    path: "./Project.csproj",
                    id: "ID",
                    parentId: null,
                    project: out _);

                // Assert
                slnFile.ToString().ShouldBe(@"
Project(""{TYPE_ID}"") = ""NAME"", ""Project.csproj"", ""{ID}""
EndProject
");
            }

            /// <summary>
            /// If any part of the path is just ".", we want that stripped out.
            /// </summary>
            [Fact]
            public void WhenExisting_ItShouldRemoveCurrentFolderComponents()
            {
                // Arrange
                var slnFile = new SlnFile(@"C:\Folder1\Folder2\SolutionFile.sln", @"
Project(""{TYPE_ID}"") = ""NAME"", ""Project.csproj"", ""{ID}""
EndProject
");

                // Act
                slnFile.GetOrCreateProjectNode(
                    typeId: "TYPE_ID",
                    name: "NAME",
                    path: "./Project.csproj",
                    id: "DIFFERENT_ID",
                    parentId: null,
                    project: out _);

                // Assert
                slnFile.ToString().ShouldBe(@"
Project(""{TYPE_ID}"") = ""NAME"", ""Project.csproj"", ""{ID}""
EndProject
");
            }

            [Fact]
            public void WhenDuplicateSubFolders_ItShouldNotThrow()
            {
                SolutionFolderModel CreateFolder(string name, IEnumerable<SolutionFolderModel> subFolders = null)
                {
                    subFolders ??= Enumerable.Empty<SolutionFolderModel>();
                    var childElements = subFolders.Select(x => x.InternalElement).ToArray();

                    var element = Substitute.For<IElement>();
                    var elementId = Guid.NewGuid().ToString();
                    element.Id.Returns(elementId);
                    element.Name.Returns(name);
                    element.ChildElements.Returns(childElements);
                    element.SpecializationType.Returns(SolutionFolderModel.SpecializationType);
                    element.SpecializationTypeId.Returns(SolutionFolderModel.SpecializationTypeId);

                    return new SolutionFolderModel(element);
                }

                // Arrange
                var slnFile = new SlnFile(@"C:\Folder1\Folder2", @"
Microsoft Visual Studio Solution File, Format Version 12.00
# Visual Studio Version 17
VisualStudioVersion = 17.2.32526.322
MinimumVisualStudioVersion = 10.0.40219.1
Project(""{2150E333-8FDC-42A3-9474-1A3956D46DE8}"") = ""RootFolder1"", ""RootFolder1"", ""{22E3E890-8EF7-4B51-AF89-E9CF3FE69485}""
EndProject
Project(""{2150E333-8FDC-42A3-9474-1A3956D46DE8}"") = ""RootFolder2"", ""RootFolder2"", ""{1CE4A8D9-E651-464B-9D6B-809FA2583B5F}""
EndProject
Project(""{2150E333-8FDC-42A3-9474-1A3956D46DE8}"") = ""SubFolder"", ""SubFolder"", ""{21A95BEB-AFE5-477F-AB6B-44E2FCFCEC93}""
EndProject
Project(""{2150E333-8FDC-42A3-9474-1A3956D46DE8}"") = ""SubFolder"", ""SubFolder"", ""{44E63A9E-BD17-40FA-AABA-6C5BE6C7B0B7}""
EndProject
Global
	GlobalSection(SolutionConfigurationPlatforms) = preSolution
		Debug|Any CPU = Debug|Any CPU
		Release|Any CPU = Release|Any CPU
	EndGlobalSection
	GlobalSection(SolutionProperties) = preSolution
		HideSolutionNode = FALSE
	EndGlobalSection
	GlobalSection(NestedProjects) = preSolution
		{21A95BEB-AFE5-477F-AB6B-44E2FCFCEC93} = {22E3E890-8EF7-4B51-AF89-E9CF3FE69485}
		{44E63A9E-BD17-40FA-AABA-6C5BE6C7B0B7} = {1CE4A8D9-E651-464B-9D6B-809FA2583B5F}
	EndGlobalSection
	GlobalSection(ExtensibilityGlobals) = postSolution
		SolutionGuid = {879976D8-5D91-4FC4-AA12-EB57797560C1}
	EndGlobalSection
EndGlobal
");
                var solutionFolderModels = new []
                {
                    CreateFolder("RootFolder1", new []
                    {
                        CreateFolder("SubFolder")
                    }),
                    CreateFolder("RootFolder2", new []
                    {
                        CreateFolder("SubFolder")
                    }),
                    CreateFolder("RootFolder3", new []
                    {
                        CreateFolder("SubFolder")
                    })
                };

                // Act
                var exception = Record.Exception(() => VisualStudio2015SolutionTemplate.SyncProjectsAndFolders(
                    slnFile: slnFile,
                    slnParent: null,
                    modelParent: null,
                    modelFolders: solutionFolderModels,
                    projectConfigurationPlatforms: new List<Node>(),
                    solutionConfigurationPlatforms: Array.Empty<string>(),
                    projects: Array.Empty<IVisualStudioProject>()));

                // Assert
                exception.ShouldBeNull();
            }
        }
    }
}
