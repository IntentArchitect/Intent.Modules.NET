using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
using Intent.Modules.VisualStudio.Projects.Api;
using Intent.Modules.VisualStudio.Projects.Templates.VisualStudioSolution;
using Microsoft.DotNet.Cli.Sln.Internal;
using NSubstitute;
using Shouldly;
using Xunit;

namespace Intent.Modules.VisualStudio.Projects.Tests.SolutionFile
{
    public class SlnFileTests
    {
        /// <summary>
        /// If any part of the path is just ".", we want that stripped out.
        /// </summary>
        [Fact]
        public void WhenNew_ItShouldRemoveCurrentFolderComponents()
        {
            // Arrange
            var slnFile = SlnFile.CreateEmpty(".");

            // Act
            slnFile.Projects.GetOrCreateProject(
                id: "ID",
                name: "NAME",
                typeGuid: "TYPE_ID",
                filePath: "./Project.csproj",
                parent: null,
                alreadyExisted: out _);

            var result = slnFile.Generate();

            // Assert
            result.ReplaceLineEndings().ShouldBe("""
                Microsoft Visual Studio Solution File, Format Version 12.00
                # Visual Studio 17
                VisualStudioVersion = 17.8.34322.80
                MinimumVisualStudioVersion = 10.0.40219.1
                Project("{TYPE_ID}") = "NAME", "Project.csproj", "{ID}"
                EndProject
                Global
                EndGlobal
                
                """
				.ReplaceLineEndings());
        }

        /// <summary>
        /// If any part of the path is just ".", we want that stripped out.
        /// </summary>
        [Fact]
        public void WhenExisting_ItShouldRemoveCurrentFolderComponents()
        {
            // Arrange
            var slnFile = SlnFile.CreateEmpty(".");
            var project = slnFile.Projects.GetOrCreateProject(id: "{ID}");
            project.FilePath = "Project.csproj";

            // Act
            slnFile.Projects.GetOrCreateProject(
                id: "ID",
                name: "NAME",
                typeGuid: "TYPE_ID",
                filePath: "./Project.csproj",
                parent: null,
                alreadyExisted: out _);

            var result = slnFile.Generate();

            // Assert
            result.ReplaceLineEndings().ShouldBe("""
                Microsoft Visual Studio Solution File, Format Version 12.00
                # Visual Studio 17
                VisualStudioVersion = 17.8.34322.80
                MinimumVisualStudioVersion = 10.0.40219.1
                Project("{TYPE_ID}") = "NAME", "Project.csproj", "{ID}"
                EndProject
                Global
                EndGlobal

                """
				.ReplaceLineEndings());
        }

        [Fact]
        public void WhenDuplicateSubFolders_ItShouldNotThrow()
        {
            // Arrange
            var slnFile = SlnFile.Read(".", """
                Microsoft Visual Studio Solution File, Format Version 12.00
                # Visual Studio Version 17
                VisualStudioVersion = 17.2.32526.322
                MinimumVisualStudioVersion = 10.0.40219.1
                Project("{2150E333-8FDC-42A3-9474-1A3956D46DE8}") = "RootFolder1", "RootFolder1", "{22E3E890-8EF7-4B51-AF89-E9CF3FE69485}"
                EndProject
                Project("{2150E333-8FDC-42A3-9474-1A3956D46DE8}") = "RootFolder2", "RootFolder2", "{1CE4A8D9-E651-464B-9D6B-809FA2583B5F}"
                EndProject
                Project("{2150E333-8FDC-42A3-9474-1A3956D46DE8}") = "SubFolder", "SubFolder", "{21A95BEB-AFE5-477F-AB6B-44E2FCFCEC93}"
                EndProject
                Project("{2150E333-8FDC-42A3-9474-1A3956D46DE8}") = "SubFolder", "SubFolder", "{44E63A9E-BD17-40FA-AABA-6C5BE6C7B0B7}"
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
                
                """);
            var solutionFolderModels = new[]
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
            var exception = Record.Exception(() => VisualStudioSolutionTemplate.SyncProjectsAndFolders(
                slnFile: slnFile,
                currentSlnFolder: default,
                currentFolderModel: default,
                childFolderModels: solutionFolderModels,
                projectModels: Array.Empty<IVisualStudioSolutionProject>()));

            // Assert
            exception.ShouldBeNull();

            return;

            static SolutionFolderModel CreateFolder(string name, IEnumerable<SolutionFolderModel> subFolders = null)
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
        }

        [Fact]
        public void ItShouldNotThrow()
        {
            // Arrange
            var slnContents = """
                              Microsoft Visual Studio Solution File, Format Version 12.00
                              # Visual Studio 14
                              VisualStudioVersion = 14.0.25420.1
                              MinimumVisualStudioVersion = 10.0.40219.1
                              Project("{2150E333-8FDC-42A3-9474-1A3956D46DE8}") = "1 - Domain", "1 - Domain", "{CD2261EE-1D28-423D-A69A-BE4998B4A276}"
                              EndProject
                              Project("{52B49DB5-3EA9-4095-B1A7-DF1AC22D7DAE}") = "Catalyst.Documents", "Catalyst.Documents\Catalyst.Documents.csproj", "{C647D424-2187-4705-B20F-086A7DFD445C}"
                              EndProject
                              Project("{52B49DB5-3EA9-4095-B1A7-DF1AC22D7DAE}") = "Catalyst.Documents.Application", "Catalyst.Documents.Application\Catalyst.Documents.Application.csproj", "{7F5A32B1-8161-4FCD-8AFC-FD725590C3BD}"
                              EndProject
                              Project("{2150E333-8FDC-42A3-9474-1A3956D46DE8}") = "2 - Infrastructure", "2 - Infrastructure", "{4FA5B178-4EEE-42AF-9182-F491A27DAC44}"
                              EndProject
                              Project("{52B49DB5-3EA9-4095-B1A7-DF1AC22D7DAE}") = "Catalyst.Documents.EntityFrameworkCore", "Catalyst.Documents.EntityFrameworkCore\Catalyst.Documents.EntityFrameworkCore.csproj", "{A331433B-F6A4-44CF-A1ED-09B9F0D70C00}"
                              EndProject
                              Project("{2150E333-8FDC-42A3-9474-1A3956D46DE8}") = "3 - Templates", "3 - Templates", "{0BC707DB-76F7-4695-A2C0-3750F879CFC4}"
                              EndProject
                              Project("{52B49DB5-3EA9-4095-B1A7-DF1AC22D7DAE}") = "Catalyst.Documents.Templates", "Catalyst.Documents.Templates\Catalyst.Documents.Templates.csproj", "{B1A1C5A5-144F-40E9-BE2F-F74D00BB7CA5}"
                              EndProject
                              Project("{2150E333-8FDC-42A3-9474-1A3956D46DE8}") = "4 - Tests", "4 - Tests", "{03A42811-459C-4087-805B-0C4A080A62A4}"
                              EndProject
                              Project("{52B49DB5-3EA9-4095-B1A7-DF1AC22D7DAE}") = "Catalyst.Documents.IntegrationTests", "Catalyst.Documents.IntegrationTests\Catalyst.Documents.IntegrationTests.csproj", "{9F64BD8E-B8E8-4AAA-9B5B-307318F6126B}"
                              EndProject
                              Project("{52B49DB5-3EA9-4095-B1A7-DF1AC22D7DAE}") = "Catalyst.Documents.UnitTests", "Catalyst.Documents.UnitTests\Catalyst.Documents.UnitTests.csproj", "{14DBB675-CA18-4F48-9A77-A97BA5710852}"
                              EndProject
                              Project("{2150E333-8FDC-42A3-9474-1A3956D46DE8}") = "5 - Documentation", "5 - Documentation", "{4EEB6571-56B3-45A4-B1F8-C120B12139F4}"
                              EndProject
                              Project("{52B49DB5-3EA9-4095-B1A7-DF1AC22D7DAE}") = "Catalyst.Documents.Documentation", "Catalyst.Documents.Documentation\Catalyst.Documents.Documentation.csproj", "{CBB7AA51-DA77-4023-BA0B-3096D9B83A1F}"
                              EndProject
                              Project("{FAE04EC0-301F-11D3-BF4B-00C04F79EFBC}") = "Catalyst.Documents.UploadService", "Catalyst.Documents.UploadService\Catalyst.Documents.UploadService.csproj", "{1180DF1F-F2C6-4C5F-860C-9960628DF681}"
                              EndProject
                              Project("{FAE04EC0-301F-11D3-BF4B-00C04F79EFBC}") = "Catalyst.Documents.FluentStorage", "Catalyst.Documents.FluentStorage\Catalyst.Documents.FluentStorage.csproj", "{6304D700-A647-4E5A-AD1A-C19634D8083D}"
                              EndProject
                              Project("{FAE04EC0-301F-11D3-BF4B-00C04F79EFBC}") = "Catalyst.Documents.IntegrationTests.AzureManagedIdentities", "Catalyst.Documents.IntegrationTests.AzureManagedIdentities\Catalyst.Documents.IntegrationTests.AzureManagedIdentities.csproj", "{AFFED016-BDA9-49F1-ABB7-B829816B7A15}"
                              EndProject
                              Project("{FAE04EC0-301F-11D3-BF4B-00C04F79EFBC}") = "Catalyst.Documents.IntegrationTests.Aws", "Catalyst.Documents.IntegrationTests.Aws\Catalyst.Documents.IntegrationTests.Aws.csproj", "{69F42808-5316-4FD9-91DE-B2A8AD4DF113}"
                              EndProject
                              Project("{FAE04EC0-301F-11D3-BF4B-00C04F79EFBC}") = "Catalyst.Documents.IntegrationTests.AzureConnectionString", "Catalyst.Documents.IntegrationTests.AzureConnectionString\Catalyst.Documents.IntegrationTests.AzureConnectionString.csproj", "{907C8F7C-7FFA-41CB-A2ED-D0FFD2D57FBE}"
                              EndProject
                              Project("{2150E333-8FDC-42A3-9474-1A3956D46DE8}") = "Solution Items", "Solution Items", "{5B92DBB0-CF84-4180-AA52-250CA68FB170}"
                                  ProjectSection(SolutionItems) = preProject
                                      Directory.Build.props = Directory.Build.props
                                  EndProjectSection
                              EndProject
                              Global
                              	GlobalSection(SolutionConfigurationPlatforms) = preSolution
                              		Debug|Any CPU = Debug|Any CPU
                              		Release|Any CPU = Release|Any CPU
                              	EndGlobalSection
                              	GlobalSection(ProjectConfigurationPlatforms) = postSolution
                              		{C647D424-2187-4705-B20F-086A7DFD445C}.Debug|Any CPU.ActiveCfg = Debug|Any CPU
                              		{C647D424-2187-4705-B20F-086A7DFD445C}.Debug|Any CPU.Build.0 = Debug|Any CPU
                              		{C647D424-2187-4705-B20F-086A7DFD445C}.Release|Any CPU.ActiveCfg = Release|Any CPU
                              		{C647D424-2187-4705-B20F-086A7DFD445C}.Release|Any CPU.Build.0 = Release|Any CPU
                              		{7F5A32B1-8161-4FCD-8AFC-FD725590C3BD}.Debug|Any CPU.ActiveCfg = Debug|Any CPU
                              		{7F5A32B1-8161-4FCD-8AFC-FD725590C3BD}.Debug|Any CPU.Build.0 = Debug|Any CPU
                              		{7F5A32B1-8161-4FCD-8AFC-FD725590C3BD}.Release|Any CPU.ActiveCfg = Release|Any CPU
                              		{7F5A32B1-8161-4FCD-8AFC-FD725590C3BD}.Release|Any CPU.Build.0 = Release|Any CPU
                              		{A331433B-F6A4-44CF-A1ED-09B9F0D70C00}.Debug|Any CPU.ActiveCfg = Debug|Any CPU
                              		{A331433B-F6A4-44CF-A1ED-09B9F0D70C00}.Debug|Any CPU.Build.0 = Debug|Any CPU
                              		{A331433B-F6A4-44CF-A1ED-09B9F0D70C00}.Release|Any CPU.ActiveCfg = Release|Any CPU
                              		{A331433B-F6A4-44CF-A1ED-09B9F0D70C00}.Release|Any CPU.Build.0 = Release|Any CPU
                              		{B1A1C5A5-144F-40E9-BE2F-F74D00BB7CA5}.Debug|Any CPU.ActiveCfg = Debug|Any CPU
                              		{B1A1C5A5-144F-40E9-BE2F-F74D00BB7CA5}.Debug|Any CPU.Build.0 = Debug|Any CPU
                              		{B1A1C5A5-144F-40E9-BE2F-F74D00BB7CA5}.Release|Any CPU.ActiveCfg = Release|Any CPU
                              		{B1A1C5A5-144F-40E9-BE2F-F74D00BB7CA5}.Release|Any CPU.Build.0 = Release|Any CPU
                              		{9F64BD8E-B8E8-4AAA-9B5B-307318F6126B}.Debug|Any CPU.ActiveCfg = Debug|Any CPU
                              		{9F64BD8E-B8E8-4AAA-9B5B-307318F6126B}.Debug|Any CPU.Build.0 = Debug|Any CPU
                              		{9F64BD8E-B8E8-4AAA-9B5B-307318F6126B}.Release|Any CPU.ActiveCfg = Release|Any CPU
                              		{9F64BD8E-B8E8-4AAA-9B5B-307318F6126B}.Release|Any CPU.Build.0 = Release|Any CPU
                              		{14DBB675-CA18-4F48-9A77-A97BA5710852}.Debug|Any CPU.ActiveCfg = Debug|Any CPU
                              		{14DBB675-CA18-4F48-9A77-A97BA5710852}.Debug|Any CPU.Build.0 = Debug|Any CPU
                              		{14DBB675-CA18-4F48-9A77-A97BA5710852}.Release|Any CPU.ActiveCfg = Release|Any CPU
                              		{14DBB675-CA18-4F48-9A77-A97BA5710852}.Release|Any CPU.Build.0 = Release|Any CPU
                              		{CBB7AA51-DA77-4023-BA0B-3096D9B83A1F}.Debug|Any CPU.ActiveCfg = Debug|Any CPU
                              		{CBB7AA51-DA77-4023-BA0B-3096D9B83A1F}.Debug|Any CPU.Build.0 = Debug|Any CPU
                              		{CBB7AA51-DA77-4023-BA0B-3096D9B83A1F}.Release|Any CPU.ActiveCfg = Release|Any CPU
                              		{CBB7AA51-DA77-4023-BA0B-3096D9B83A1F}.Release|Any CPU.Build.0 = Release|Any CPU
                              		{1180DF1F-F2C6-4C5F-860C-9960628DF681}.Debug|Any CPU.ActiveCfg = Debug|Any CPU
                              		{1180DF1F-F2C6-4C5F-860C-9960628DF681}.Debug|Any CPU.Build.0 = Debug|Any CPU
                              		{1180DF1F-F2C6-4C5F-860C-9960628DF681}.Release|Any CPU.ActiveCfg = Release|Any CPU
                              		{1180DF1F-F2C6-4C5F-860C-9960628DF681}.Release|Any CPU.Build.0 = Release|Any CPU
                              		{6304D700-A647-4E5A-AD1A-C19634D8083D}.Debug|Any CPU.ActiveCfg = Debug|Any CPU
                              		{6304D700-A647-4E5A-AD1A-C19634D8083D}.Debug|Any CPU.Build.0 = Debug|Any CPU
                              		{6304D700-A647-4E5A-AD1A-C19634D8083D}.Release|Any CPU.ActiveCfg = Release|Any CPU
                              		{6304D700-A647-4E5A-AD1A-C19634D8083D}.Release|Any CPU.Build.0 = Release|Any CPU
                              		{AFFED016-BDA9-49F1-ABB7-B829816B7A15}.Debug|Any CPU.ActiveCfg = Debug|Any CPU
                              		{AFFED016-BDA9-49F1-ABB7-B829816B7A15}.Debug|Any CPU.Build.0 = Debug|Any CPU
                              		{AFFED016-BDA9-49F1-ABB7-B829816B7A15}.Release|Any CPU.ActiveCfg = Release|Any CPU
                              		{AFFED016-BDA9-49F1-ABB7-B829816B7A15}.Release|Any CPU.Build.0 = Release|Any CPU
                              		{69F42808-5316-4FD9-91DE-B2A8AD4DF113}.Debug|Any CPU.ActiveCfg = Debug|Any CPU
                              		{69F42808-5316-4FD9-91DE-B2A8AD4DF113}.Debug|Any CPU.Build.0 = Debug|Any CPU
                              		{69F42808-5316-4FD9-91DE-B2A8AD4DF113}.Release|Any CPU.ActiveCfg = Release|Any CPU
                              		{69F42808-5316-4FD9-91DE-B2A8AD4DF113}.Release|Any CPU.Build.0 = Release|Any CPU
                              		{907C8F7C-7FFA-41CB-A2ED-D0FFD2D57FBE}.Debug|Any CPU.ActiveCfg = Debug|Any CPU
                              		{907C8F7C-7FFA-41CB-A2ED-D0FFD2D57FBE}.Debug|Any CPU.Build.0 = Debug|Any CPU
                              		{907C8F7C-7FFA-41CB-A2ED-D0FFD2D57FBE}.Release|Any CPU.ActiveCfg = Release|Any CPU
                              		{907C8F7C-7FFA-41CB-A2ED-D0FFD2D57FBE}.Release|Any CPU.Build.0 = Release|Any CPU
                              	EndGlobalSection
                              	GlobalSection(SolutionProperties) = preSolution
                              		HideSolutionNode = FALSE
                              	EndGlobalSection
                              	GlobalSection(NestedProjects) = preSolution
                              		{C647D424-2187-4705-B20F-086A7DFD445C} = {CD2261EE-1D28-423D-A69A-BE4998B4A276}
                              		{7F5A32B1-8161-4FCD-8AFC-FD725590C3BD} = {CD2261EE-1D28-423D-A69A-BE4998B4A276}
                              		{A331433B-F6A4-44CF-A1ED-09B9F0D70C00} = {4FA5B178-4EEE-42AF-9182-F491A27DAC44}
                              		{B1A1C5A5-144F-40E9-BE2F-F74D00BB7CA5} = {0BC707DB-76F7-4695-A2C0-3750F879CFC4}
                              		{9F64BD8E-B8E8-4AAA-9B5B-307318F6126B} = {03A42811-459C-4087-805B-0C4A080A62A4}
                              		{14DBB675-CA18-4F48-9A77-A97BA5710852} = {03A42811-459C-4087-805B-0C4A080A62A4}
                              		{CBB7AA51-DA77-4023-BA0B-3096D9B83A1F} = {4EEB6571-56B3-45A4-B1F8-C120B12139F4}
                              		{1180DF1F-F2C6-4C5F-860C-9960628DF681} = {4FA5B178-4EEE-42AF-9182-F491A27DAC44}
                              		{6304D700-A647-4E5A-AD1A-C19634D8083D} = {4FA5B178-4EEE-42AF-9182-F491A27DAC44}
                              		{AFFED016-BDA9-49F1-ABB7-B829816B7A15} = {03A42811-459C-4087-805B-0C4A080A62A4}
                              		{69F42808-5316-4FD9-91DE-B2A8AD4DF113} = {03A42811-459C-4087-805B-0C4A080A62A4}
                              		{907C8F7C-7FFA-41CB-A2ED-D0FFD2D57FBE} = {03A42811-459C-4087-805B-0C4A080A62A4}
                              	EndGlobalSection
                              EndGlobal
                              
                              """;

            // Act
            var j = SlnFile.Read(".", slnContents);

            // Assert
        }

        [Fact]
        public void ItShouldCorrectlyPutIntoSubFolders()
        {
            // Arrange
            var slnFile = SlnFile.CreateEmpty("/File.sln");

            // Act
            slnFile.AddSolutionItem(
                parentProject: null,
                solutionItemAbsolutePath: "/Subfolder/File.ext",
                relativeOutputPathPrefix: null,
                idProvider: () => "ID");

            var result = slnFile.Generate();

            // Assert
            result.ReplaceLineEndings().ShouldBe("""
                Microsoft Visual Studio Solution File, Format Version 12.00
                # Visual Studio 17
                VisualStudioVersion = 17.8.34322.80
                MinimumVisualStudioVersion = 10.0.40219.1
                Project("{2150E333-8FDC-42A3-9474-1A3956D46DE8}") = "Subfolder", "Subfolder", "{ID}"
                	ProjectSection(SolutionItems) = preProject
                		Subfolder\File.ext = Subfolder\File.ext
                	EndProjectSection
                EndProject
                Global
                EndGlobal

                """
				.ReplaceLineEndings());
        }

        [Fact]
        public void ItShouldMoveExistingFilesWhenFolderIsDifferent()
        {
            // Arrange
            var idCounter = 0;
            var slnFile = SlnFile.CreateEmpty("/File.sln");
            var project = slnFile.Projects.GetOrCreateProject("OLD", "OldFolder", SlnFileExtensions.TypeGuid.Folder[1..^1], "OldFolder", null, out _);
            var section = project.Sections.GetOrCreateSection(SlnFileExtensions.SectionId.SolutionItems, SlnSectionType.PreProcess);
            section.Properties.TryAdd("NewFolder\\File.ext", "NewFolder\\File.ext");

            // Act
            slnFile.AddSolutionItem(
                parentProject: null,
                solutionItemAbsolutePath: "/NewFolder/File.ext",
                relativeOutputPathPrefix: null,
                idProvider: () => idCounter++.ToString());

            var result = slnFile.Generate();

            // Assert
            result.ReplaceLineEndings().ShouldBe("""
                Microsoft Visual Studio Solution File, Format Version 12.00
                # Visual Studio 17
                VisualStudioVersion = 17.8.34322.80
                MinimumVisualStudioVersion = 10.0.40219.1
                Project("{2150E333-8FDC-42A3-9474-1A3956D46DE8}") = "OldFolder", "OldFolder", "{OLD}"
                EndProject
                Project("{2150E333-8FDC-42A3-9474-1A3956D46DE8}") = "NewFolder", "NewFolder", "{0}"
                	ProjectSection(SolutionItems) = preProject
                		NewFolder\File.ext = NewFolder\File.ext
                	EndProjectSection
                EndProject
                Global
                EndGlobal

                """
				.ReplaceLineEndings());
        }
    }
}
