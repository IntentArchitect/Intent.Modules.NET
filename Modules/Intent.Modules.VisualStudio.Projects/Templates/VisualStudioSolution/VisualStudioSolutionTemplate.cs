using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Intent.Engine;
using Intent.Modules.Common;
using Intent.Modules.Common.Templates;
using Intent.Modules.Constants;
using Intent.Modules.VisualStudio.Projects.Api;
using Intent.Modules.VisualStudio.Projects.OutputTargets;
using Intent.Templates;
using Microsoft.DotNet.Cli.Sln.Internal;

namespace Intent.Modules.VisualStudio.Projects.Templates.VisualStudioSolution
{
    // NB! Solution Project Type guids: http://www.codeproject.com/Reference/720512/List-of-Visual-Studio-Project-Type-GUIDs
    public class VisualStudioSolutionTemplate : ITemplate, IConfigurableTemplate
    {
        public const string Identifier = "Intent.VisualStudio.Projects.VisualStudioSolution";

        private IFileMetadata _fileMetadata;
        private readonly OutputLocationOptions _outputLocationOptions;

        public VisualStudioSolutionTemplate(IApplication application, VisualStudioSolutionModel model, IEnumerable<IVisualStudioSolutionProject> projects)
            : this(application, model, projects, outputLocationOptions: null)
        {
        }

        internal VisualStudioSolutionTemplate(IApplication application, VisualStudioSolutionModel model, IEnumerable<IVisualStudioSolutionProject> projects, OutputLocationOptions outputLocationOptions)
        {
            Application = application;
            Model = model;
            BindingContext = new TemplateBindingContext(new VisualStudioSolutionTemplateModel(Application));
            Projects = projects;
            _outputLocationOptions = outputLocationOptions ?? new OutputLocationOptions(application.OutputRootDirectory, relativeLocation: "");
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
                configurationPlatforms: configurationPlatforms,
                outputRootDirectory: Application.OutputRootDirectory,
                locationInProject: GetEffectiveSolutionOffset(),
                outputLocationOptions: _outputLocationOptions);

            return slnFile.Generate();
        }

        private string GetLegacySolutionRelativeLocation()
        {
            return Model.HasVisualStudioSolutionOptions() && !string.IsNullOrWhiteSpace(Model.GetVisualStudioSolutionOptions().SolutionRelativeLocation())
                ? Model.GetVisualStudioSolutionOptions().SolutionRelativeLocation()
                : null;
        }

        /// <remarks>
        /// The full offset from <see cref="IApplication.OutputRootDirectory"/> to the .sln's actual
        /// resolved directory, folding together the Root Folder shift with the legacy per-solution
        /// <c>Solution Relative Location</c> shift. This is the ONE value used both for
        /// <see cref="ITemplateFileConfig.LocationInProject"/> (which is what actually moves the .sln -
        /// always relative to <see cref="IApplication.OutputRootDirectory"/>, regardless of
        /// <c>fileLocation</c>) and by <see cref="GetProjectFilePath"/> to place project entries
        /// correctly relative to wherever that puts the .sln. Keeping both consumers on this single
        /// value is what prevents the shift being counted twice or once too few times.
        /// </remarks>
        private string GetEffectiveSolutionOffset()
        {
            var legacy = GetLegacySolutionRelativeLocation();
            var rootShift = _outputLocationOptions.RelativeLocation;

            if (string.IsNullOrEmpty(rootShift))
            {
                return legacy;
            }

            return string.IsNullOrEmpty(legacy) ? rootShift : Path.Combine(rootShift, legacy);
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
            ConfigurationPlatform[] configurationPlatforms,
            string outputRootDirectory = null,
            string locationInProject = null,
            OutputLocationOptions outputLocationOptions = null)
        {
            outputLocationOptions ??= OutputLocationOptions.None;

            foreach (var model in projectModels.Where(x => x.ParentFolder?.Id == currentFolderModel?.Id))
            {
                var filePath = GetProjectFilePath(model, outputRootDirectory, locationInProject, outputLocationOptions);

                var typeGuid = model is SQLServerDatabaseProjectModel sqlModel &&
                               sqlModel.HasSQLServerDatabaseProject() &&
                               sqlModel.GetSQLServerDatabaseProject().ProjectType().IsSDK()
                    ? VisualStudioProjectTypeIds.SdkSqlProject
                    : model.ProjectTypeId;

                var project = slnFile.Projects.GetOrCreateProject(
                    id: model.Id,
                    name: model.Name,
                    typeGuid: typeGuid,
                    filePath: filePath,
                    parent: currentSlnFolder,
                    alreadyExisted: out _);

                var propertySet = slnFile.ProjectConfigurationsSection.GetOrCreatePropertySet(project.Id);

                (string[] ProjectConfigurationSuffixes, string DefaultPlatform) config = model.ProjectTypeId switch
                {
                    VisualStudioProjectTypeIds.ServiceFabricProject => ([".ActiveCfg", ".Build.0", ".Deploy.0"], "x64"),
                    VisualStudioProjectTypeIds.SQLServerDatabaseProject => default,
                    _ => (new[] { ".ActiveCfg", ".Build.0" }, "Any CPU")
                };

                if (config == default)
                {
                    continue;
                }

                var projectConfigurations = configurationPlatforms
                    .SelectMany(_ => config.ProjectConfigurationSuffixes, (configurationPlatform, suffix) => new
                    {
                        ConfigurationPlatform = configurationPlatform,
                        Suffix = suffix,
                        Key = $"{configurationPlatform.Joined}{suffix}"
                    })
                    .OrderBy(x => x.Key)
                    .ToArray();

                if (projectConfigurations.All(x => propertySet.ContainsKey(x.Key)))
                {
                    continue;
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
                                projectConfigurations.FirstOrDefault(x => x.ConfigurationPlatform.Configuration == item.ConfigurationPlatform.Configuration && x.ConfigurationPlatform.Platform == config.DefaultPlatform)?.ConfigurationPlatform.Joined ??
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
                    configurationPlatforms: configurationPlatforms,
                    outputRootDirectory: outputRootDirectory,
                    locationInProject: locationInProject,
                    outputLocationOptions: outputLocationOptions);
            }
        }

        /// <remarks>
        /// When <paramref name="locationInProject"/> is unset, this reproduces the exact historical
        /// string format (including any leading <c>.\</c> segment from a materialized parent Solution
        /// Folder) byte-for-byte. It only switches to resolving via absolute paths - which normalizes
        /// that format - once the .sln file's own location has actually been shifted away from
        /// <paramref name="outputRootDirectory"/>, since only then do the two need reconciling.
        /// </remarks>
        private static string GetProjectFilePath(IVisualStudioSolutionProject model, string outputRootDirectory, string locationInProject, OutputLocationOptions outputLocationOptions)
        {
            var relativeLocation = outputLocationOptions.GetEffectiveRelativeLocation(model.RelativeLocation, model.Name);

            if (string.IsNullOrEmpty(locationInProject) || string.IsNullOrEmpty(outputRootDirectory))
            {
                return $"{relativeLocation}\\{model.Name}.{model.FileExtension}".Replace("/", "\\");
            }

            var solutionDirectory = Path.GetFullPath(Path.Combine(outputRootDirectory, locationInProject));
            var projectDirectory = Path.GetFullPath(Path.Combine(outputRootDirectory, relativeLocation ?? model.Name));
            var relativeToSolution = Path.GetRelativePath(solutionDirectory, projectDirectory);

            return Path.Combine(relativeToSolution, $"{model.Name}.{model.FileExtension}").Replace("/", "\\");
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

            // LocationInProject (not fileLocation) is what actually moves the .sln, always relative to
            // OutputRootDirectory - so the Root Folder shift and the legacy Solution Relative Location
            // must both be folded into this single value, not split across the two.
            var offset = GetEffectiveSolutionOffset();
            if (!string.IsNullOrEmpty(offset))
            {
                solutionFileMetadata.LocationInProject = offset;
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
