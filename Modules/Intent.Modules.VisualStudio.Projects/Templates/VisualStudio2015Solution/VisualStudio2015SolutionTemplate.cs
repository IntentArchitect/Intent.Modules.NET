using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Intent.Engine;
using Intent.Modules.Common;
using Intent.Modules.Common.Templates;
using Intent.Modules.VisualStudio.Projects.Api;
using Intent.Modules.VisualStudio.Projects.SolutionFile;
using Intent.Templates;

namespace Intent.Modules.VisualStudio.Projects.Templates.VisualStudio2015Solution
{
    // NB! Solution Project Type guids: http://www.codeproject.com/Reference/720512/List-of-Visual-Studio-Project-Type-GUIDs
    public class VisualStudio2015SolutionTemplate : ITemplate, IConfigurableTemplate
    {
        public const string Identifier = "Intent.VisualStudio.Projects.VisualStudio2015Solution";
        private static readonly string[] SolutionConfigurationPlatformTypes = { "ActiveCfg", "Build.0" };

        private IFileMetadata _fileMetadata;

        public VisualStudio2015SolutionTemplate(IApplication application, VisualStudioSolutionModel model, IEnumerable<IVisualStudioProject> projects)
        {
            Application = application;
            Model = model;
            BindingContext = new TemplateBindingContext(new VisualStudio2015SolutionTemplateModel(Application));
            Projects = projects;
        }

        public string Id => Identifier;
        public IApplication Application { get; }
        public VisualStudioSolutionModel Model { get; }
        public IEnumerable<IVisualStudioProject> Projects { get; }

        public bool CanRunTemplate() => true;

        public string RunTemplate()
        {
            var targetFile = GetMetadata().GetFilePath();
            var content = File.Exists(targetFile)
                ? File.ReadAllText(targetFile)
                : "Microsoft Visual Studio Solution File, Format Version 12.00" + Environment.NewLine +
                  "# Visual Studio 14" + Environment.NewLine +
                  "VisualStudioVersion = 14.0.25420.1" + Environment.NewLine +
                  "MinimumVisualStudioVersion = 10.0.40219.1";
            var slnFile = new SlnFile(targetFile, content);

            slnFile.GetOrCreateGlobalNode("Global", out var globalNode);

            globalNode.GetOrCreateSection(
                name: "SolutionConfigurationPlatforms",
                value: "preSolution",
                childNodes: new List<Node>
                {
                    new KeyValueNode("Debug|Any CPU", "Debug|Any CPU"),
                    new KeyValueNode("Release|Any CPU", "Release|Any CPU")
                },
                sectionNode: out var solutionConfigurationPlatforms);
            globalNode.GetOrCreateSection(
                name: "ProjectConfigurationPlatforms",
                value: "postSolution",
                childNodes: new List<Node>(),
                sectionNode: out var projectConfigurationPlatforms);
            globalNode.GetOrCreateSection(
                name: "SolutionProperties",
                value: "preSolution",
                childNodes: new List<Node>
                {
                    new KeyValueNode("HideSolutionNode", "FALSE")
                },
                sectionNode: out _);
            globalNode.GetOrCreateSection(
                name: "NestedProjects",
                value: "preSolution",
                childNodes: new List<Node>(),
                sectionNode: out _);

            SyncProjectsAndFolders(
                slnFile: slnFile,
                slnParent: null,
                modelParent: null,
                modelFolders: Model.Folders,
                projectConfigurationPlatforms: projectConfigurationPlatforms.ChildNodes,
                solutionConfigurationPlatforms: solutionConfigurationPlatforms.ChildNodes
                    .OfType<KeyValueNode>()
                    .Select(x => x.Key)
                    .ToArray());

            return slnFile.ToString();
        }

        private void SyncProjectsAndFolders(
            SlnFile slnFile,
            SolutionItemComplexNode slnParent,
            SolutionFolderModel modelParent,
            IEnumerable<SolutionFolderModel> modelFolders,
            List<Node> projectConfigurationPlatforms,
            IReadOnlyCollection<string> solutionConfigurationPlatforms)
        {
            foreach (var projectModel in Projects.Where(x => x.ParentFolder?.Id == modelParent?.Id))
            {
                var path = $"{projectModel.ToOutputTargetConfig().RelativeLocation}\\{projectModel.Name}.{projectModel.FileExtension}".Replace("/", "\\");

                if (!slnFile.GetOrCreateProjectNode(
                        typeId: projectModel.ProjectTypeId,
                        name: projectModel.Name,
                        path: path,
                        id: projectModel.Id,
                        parentId: slnParent?.Id,
                        project: out var slnProject))
                {
                    projectConfigurationPlatforms.AddRange(solutionConfigurationPlatforms
                        .SelectMany(
                            _ => SolutionConfigurationPlatformTypes,
                            (platform, platformType) => new KeyValueNode($"{slnProject.Id}.{platform}.{platformType}", platform)));
                    continue;
                }

                slnProject.Name = projectModel.Name;
                slnProject.Path = path;
                slnFile.SetParent(slnProject.UnderlyingNode, slnParent);
            }

            foreach (var folderModel in modelFolders)
            {
                if (slnFile.GetOrCreateProjectNode(
                        typeId: VisualStudioSolution.ProjectTypeIds.SolutionFolder,
                        name: folderModel.Name,
                        path: folderModel.Name,
                        id: folderModel.Id,
                        parentId: slnParent?.Id,
                        project: out var slnFolder))
                {
                    slnFolder.Name = folderModel.Name;
                    slnFolder.Path = folderModel.Name;
                    slnFile.SetParent(slnFolder.UnderlyingNode, slnParent);
                }

                SyncProjectsAndFolders(
                    slnFile: slnFile,
                    slnParent: slnFolder.UnderlyingNode,
                    modelParent: folderModel,
                    modelFolders: folderModel.Folders,
                    projectConfigurationPlatforms: projectConfigurationPlatforms,
                    solutionConfigurationPlatforms: solutionConfigurationPlatforms);
            }
        }

        public IFileMetadata GetMetadata()
        {
            if (_fileMetadata == null)
            {
                throw new Exception("File Metadata must be specified.");
            }
            return _fileMetadata;
        }

        public void ConfigureFileMetadata(IFileMetadata fileMetadata)
        {
            _fileMetadata = fileMetadata;
            _fileMetadata.CustomMetadata.TryAdd("CorrelationId", $"{Identifier}#{Model.Id}");
        }

        public ITemplateFileConfig GetTemplateFileConfig()
        {
            return new SolutionFileMetadata(
                outputType: "VisualStudio2015Solution",
                overwriteBehaviour: OverwriteBehaviour.Always,
                codeGenType: CodeGenType.UserControlledWeave,
                fileName: $"{Model.Name}",
                fileLocation: Application.OutputRootDirectory);
        }

        public ITemplateBindingContext BindingContext { get; }

        private class VisualStudio2015SolutionTemplateModel
        {
            public VisualStudio2015SolutionTemplateModel(IApplication application)
            {
                Application = application;
            }

            public IApplication Application { get; }
        }
    }
}
