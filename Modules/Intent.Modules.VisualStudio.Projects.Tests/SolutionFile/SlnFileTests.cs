using Intent.Modules.VisualStudio.Projects.SolutionFile;
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
        }
    }
}
