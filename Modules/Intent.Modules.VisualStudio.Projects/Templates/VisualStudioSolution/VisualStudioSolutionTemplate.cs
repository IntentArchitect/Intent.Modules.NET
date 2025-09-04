using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Intent.Engine;
using Intent.Modules.Common;
using Intent.Modules.Common.Templates;
using Intent.Modules.Constants;
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

            SyncSolutionConfigurationPlatforms(slnFile, out var configurationPlatforms);

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
                projectModels: Projects.ToArray(),
                configurationPlatforms: configurationPlatforms);

            return slnFile.Generate();
        }

        private void SyncSolutionConfigurationPlatforms(SlnFile slnFile, out ConfigurationPlatform[] configurationPlatforms)
        {
            var section = slnFile.SolutionConfigurationsSection;
            IEnumerable<string> configurations;
            IEnumerable<string> platforms;

            if (section.IsEmpty)
            {
                configurations = ["Debug", "Release"];
                platforms = GetRequiredPlatforms().Union(["Any CPU"]);
            }
            else
            {
                var items = section.Keys.Select(x => x.Split('|')).ToArray();
                configurations = items.Select(x => x[0]);
                platforms = GetRequiredPlatforms().Union(items.Select(x => x[1]));
            }

            configurationPlatforms = configurations
                .SelectMany(x => platforms.Select(y => new ConfigurationPlatform(x, y)))
                .OrderBy(x => x.Joined)
                .ToArray();

            if (configurationPlatforms.All(x => section.ContainsKey(x.Joined)))
            {
                return;
            }

            // Clear first so that re-added in sorted order
            section.Clear();

            foreach (var item in configurationPlatforms)
            {
                section.TryAdd(item.Joined, item.Joined);
            }
        }

        private IReadOnlyCollection<string> GetRequiredPlatforms()
        {
            if (Projects.Any(x => x.ProjectTypeId == VisualStudioProjectTypeIds.ServiceFabricProject))
            {
                return ["x64"];
            }

            return [];
        }

        /// <remarks>
        /// <see langword="internal"/> so can be unit tested.
        /// </remarks>>
        internal static void SyncProjectsAndFolders(
            SlnFile slnFile,
            SlnProject currentSlnFolder,
            SolutionFolderModel currentFolderModel,
            IEnumerable<SolutionFolderModel> childFolderModels,
            IReadOnlyCollection<IVisualStudioSolutionProject> projectModels,
            ConfigurationPlatform[] configurationPlatforms)
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
                    alreadyExisted: out _);

                var propertySet = slnFile.ProjectConfigurationsSection.GetOrCreatePropertySet(project.Id);

                var (projectConfigurationSuffixes, defaultPlatform) = model.ProjectTypeId switch
                {
                    VisualStudioProjectTypeIds.ServiceFabricProject => ([".ActiveCfg", ".Build.0", ".Deploy.0"], "x64"),
                    _ => (new[] { ".ActiveCfg", ".Build.0" }, "Any CPU")
                };

                var projectConfigurations = configurationPlatforms
                    .SelectMany(_ => projectConfigurationSuffixes, (configurationPlatform, suffix) => new
                    {
                        ConfigurationPlatform = configurationPlatform,
                        Suffix = suffix,
                        Key = $"{configurationPlatform.Joined}{suffix}"
                    })
                    .OrderBy(x => x.Key)
                    .ToArray();

                if (projectConfigurations.All(x => propertySet.ContainsKey(x.Key)))
                {
                    return;
                }

                var oldValues = propertySet
                    .OrderBy(x => x.Key)
                    .Select(x => new { x.Key, x.Value })
                    .ToArray();

                // Clear first so that re-added in sorted order
                propertySet.Values.Clear();

                foreach (var item in projectConfigurations)
                {
                    var value = oldValues.FirstOrDefault(x => x.Key == item.Key)?.Value ??
                                oldValues.FirstOrDefault(x => x.Key.StartsWith(item.ConfigurationPlatform.ConfigurationWithPipe) && x.Key.EndsWith(item.Suffix))?.Value ??
                                projectConfigurations.FirstOrDefault(x => x.ConfigurationPlatform.Configuration == item.ConfigurationPlatform.Configuration && x.ConfigurationPlatform.Platform == defaultPlatform)?.ConfigurationPlatform.Joined ??
                                item.ConfigurationPlatform.Joined;

                    propertySet.TryAdd(item.Key, value);
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
                    projectModels: projectModels,
                    configurationPlatforms: configurationPlatforms);
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
            var solutionFileMetadata = new SolutionFileMetadata(
                outputType: "VisualStudioSolution",
                overwriteBehaviour: OverwriteBehaviour.Always,
                codeGenType: CodeGenType.UserControlledWeave,
                fileName: GetSolutionFilename(),
                fileLocation: Application.OutputRootDirectory);

            if (Model.HasVisualStudioSolutionOptions() && !string.IsNullOrWhiteSpace(Model.GetVisualStudioSolutionOptions().SolutionRelativeLocation()))
            {
                solutionFileMetadata.LocationInProject = Model.GetVisualStudioSolutionOptions().SolutionRelativeLocation();
            }

            return solutionFileMetadata;
        }

        private string GetSolutionFilename()
        {
            if (Model.HasVisualStudioSolutionOptions() && !string.IsNullOrWhiteSpace(Model.GetVisualStudioSolutionOptions().SolutionName()))
            {
                return Model.GetVisualStudioSolutionOptions().SolutionName();
            }
            return $"{Model.Name}";
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

        internal record ConfigurationPlatform(string Configuration, string Platform)
        {
            public string Joined { get; } = $"{Configuration}|{Platform}";
            public string ConfigurationWithPipe { get; } = $"{Configuration}";
        }
    }
}
