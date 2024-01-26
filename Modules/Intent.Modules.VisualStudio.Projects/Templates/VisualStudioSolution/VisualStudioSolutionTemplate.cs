using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Intent.Engine;
using Intent.Modules.Common;
using Intent.Modules.Common.Templates;
using Intent.Modules.VisualStudio.Projects.Api;
using Intent.Templates;
using Microsoft.DotNet.Cli.Sln.Internal;

namespace Intent.Modules.VisualStudio.Projects.Templates.VisualStudioSolution
{
    // NB! Solution Project Type guids: http://www.codeproject.com/Reference/720512/List-of-Visual-Studio-Project-Type-GUIDs
    public class VisualStudioSolutionTemplate : ITemplate, IConfigurableTemplate
    {
        public const string Identifier = "Intent.VisualStudio.Projects.VisualStudioSolution";

        private IFileMetadata _fileMetadata;

        public VisualStudioSolutionTemplate(IApplication application, VisualStudioSolutionModel model, IEnumerable<IVisualStudioSolutionProject> projects)
        {
            Application = application;
            Model = model;
            BindingContext = new TemplateBindingContext(new VisualStudioSolutionTemplateModel(Application));
            Projects = projects;
        }

        public string Id => Identifier;
        public IApplication Application { get; }
        public VisualStudioSolutionModel Model { get; }
        public IEnumerable<IVisualStudioSolutionProject> Projects { get; }

        public bool CanRunTemplate() => true;

        public string RunTemplate()
        {
            var targetFile = GetMetadata().GetFilePath();

            var slnFile = File.Exists(targetFile)
                ? SlnFile.Read(targetFile, File.ReadAllText(targetFile))
                : SlnFile.CreateEmpty(targetFile);

            var solutionConfigurationPlatforms = slnFile.SolutionConfigurationsSection;
            if (solutionConfigurationPlatforms.IsEmpty)
            {
                solutionConfigurationPlatforms.TryAdd("Debug|Any CPU", "Debug|Any CPU");
                solutionConfigurationPlatforms.TryAdd("Release|Any CPU", "Release|Any CPU");
            }

            var solutionProperties = slnFile.GetOrCreateSolutionPropertiesSection(out var alreadyAlreadyExisted);
            if (!alreadyAlreadyExisted)
            {
                solutionProperties.TryAdd("HideSolutionNode", "FALSE");
            }

            SyncProjectsAndFolders(
                slnFile: slnFile,
                currentSlnFolder: null,
                currentFolderModel: null,
                childFolderModels: Model.Folders,
                projectModels: Projects.ToArray());

            return slnFile.Generate();
        }

        /// <remarks>
        /// <see langword="internal"/> so can be unit tested.
        /// </remarks>>
        internal static void SyncProjectsAndFolders(
            SlnFile slnFile,
            SlnProject currentSlnFolder,
            SolutionFolderModel currentFolderModel,
            IEnumerable<SolutionFolderModel> childFolderModels,
            IReadOnlyCollection<IVisualStudioSolutionProject> projectModels)
        {
            foreach (var model in projectModels.Where(x => x.ParentFolder?.Id == currentFolderModel?.Id))
            {
                var filePath = $"{model.ToOutputTargetConfig().RelativeLocation}\\{model.Name}.{model.FileExtension}".Replace("/", "\\");

                var project = slnFile.Projects.GetOrCreateProject(
                    id: model.Id,
                    name: model.Name,
                    typeGuid: model.ProjectTypeId,
                    filePath: filePath,
                    parent: currentSlnFolder,
                    alreadyExisted: out var alreadyExisted);

                if (alreadyExisted)
                {
                    continue;
                }

                foreach (var configuration in slnFile.SolutionConfigurationsSection.Values)
                {
                    var propertySet = slnFile.ProjectConfigurationsSection.GetOrCreatePropertySet(project.Id);
                    propertySet.TryAdd($"{configuration}.ActiveCfg", configuration);
                    propertySet.TryAdd($"{configuration}.Build.0", configuration);
                }
            }

            foreach (var childFolderModel in childFolderModels)
            {
                var childSlnFolder = currentSlnFolder == null
                    ? slnFile.GetOrCreateFolder(childFolderModel.Id, childFolderModel.Name)
                    : currentSlnFolder.GetOrCreateFolder(childFolderModel.Id, childFolderModel.Name);

                SyncProjectsAndFolders(
                    slnFile: slnFile,
                    currentSlnFolder: childSlnFolder,
                    currentFolderModel: childFolderModel,
                    childFolderModels: childFolderModel.Folders,
                    projectModels: projectModels);
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
                outputType: "VisualStudioSolution",
                overwriteBehaviour: OverwriteBehaviour.Always,
                codeGenType: CodeGenType.UserControlledWeave,
                fileName: $"{Model.Name}",
                fileLocation: Application.OutputRootDirectory);
        }

        public ITemplateBindingContext BindingContext { get; }

        private class VisualStudioSolutionTemplateModel
        {
            public VisualStudioSolutionTemplateModel(IApplication application)
            {
                Application = application;
            }

            public IApplication Application { get; }
        }
    }
}
