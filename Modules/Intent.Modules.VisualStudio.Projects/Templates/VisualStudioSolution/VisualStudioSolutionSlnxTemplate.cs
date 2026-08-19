using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using Intent.Engine;
using Intent.Modules.Common;
using Intent.Modules.Common.Templates;
using Intent.Modules.VisualStudio.Projects.Api;
using Intent.Modules.VisualStudio.Projects.OutputTargets;
using Intent.Templates;
using Microsoft.VisualStudio.SolutionPersistence.Model;
using Microsoft.VisualStudio.SolutionPersistence.Serializer;

namespace Intent.Modules.VisualStudio.Projects.Templates.VisualStudioSolution
{
    /// <summary>
    /// Generates the .slnx (XML Solution) file content purely from the Intent model - it has no
    /// awareness of, and never reads, any existing file on disk. Reconciling that fresh output
    /// with whatever is already on disk (preserving manual edits, detecting renames/moves) is the
    /// job of <c>VisualStudioSolutionSlnxWeaver</c>, which runs as a post-processing step over this
    /// template's raw output.
    /// </summary>
    public class VisualStudioSolutionSlnxTemplate : ITemplate, IConfigurableTemplate
    {
        public const string Identifier = "Intent.VisualStudio.Projects.VisualStudioSolution";

        private IFileMetadata _fileMetadata;
        private readonly OutputLocationOptions _outputLocationOptions;

        public VisualStudioSolutionSlnxTemplate(IApplication application, VisualStudioSolutionModel model, IEnumerable<IVisualStudioSolutionProject> projects)
            : this(application, model, projects, outputLocationOptions: null)
        {
        }

        internal VisualStudioSolutionSlnxTemplate(IApplication application, VisualStudioSolutionModel model, IEnumerable<IVisualStudioSolutionProject> projects, OutputLocationOptions outputLocationOptions)
        {
            Application = application;
            Model = model;
            BindingContext = new TemplateBindingContext(new VisualStudioSolutionTemplateModel(Application));
            Projects = projects.ToList();
            _outputLocationOptions = outputLocationOptions ?? new OutputLocationOptions(application.OutputRootDirectory, relativeLocation: "");
        }

        public string Id => Identifier;
        public IApplication Application { get; }
        public VisualStudioSolutionModel Model { get; }
        public IReadOnlyList<IVisualStudioSolutionProject> Projects { get; }

        public bool CanRunTemplate() => true;

        public string RunTemplate()
        {
            var solutionModel = new SolutionModel();
            SyncFoldersAndProjects(solutionModel, Model.Folders, Projects, Application.OutputRootDirectory, GetEffectiveSolutionOffset(), _outputLocationOptions);

            using var stream = new MemoryStream();
            SolutionSerializers.SlnXml.SaveAsync(stream, solutionModel, CancellationToken.None).GetAwaiter().GetResult();
            return Encoding.UTF8.GetString(stream.ToArray());
        }

        private string GetLegacySolutionRelativeLocation()
        {
            return Model.HasVisualStudioSolutionOptions() && !string.IsNullOrWhiteSpace(Model.GetVisualStudioSolutionOptions().SolutionRelativeLocation())
                ? Model.GetVisualStudioSolutionOptions().SolutionRelativeLocation()
                : null;
        }

        /// <remarks>
        /// The full offset from <see cref="IApplication.OutputRootDirectory"/> to the .slnx's actual
        /// resolved directory, folding together the Root Folder shift with the legacy per-solution
        /// <c>Solution Relative Location</c> shift. This is the ONE value used both for
        /// <see cref="ITemplateFileConfig.LocationInProject"/> (which is what actually moves the .slnx -
        /// always relative to <see cref="IApplication.OutputRootDirectory"/>, regardless of
        /// <c>fileLocation</c>) and by <see cref="GetProjectRelativePath"/> to place project entries
        /// correctly relative to wherever that puts the .slnx. Keeping both consumers on this single
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

        /// <remarks>
        /// <see langword="internal"/> so it can be unit tested directly.
        /// </remarks>
        internal static void SyncFoldersAndProjects(
            SolutionModel solutionModel,
            IEnumerable<Api.SolutionFolderModel> intentFolders,
            IReadOnlyList<IVisualStudioSolutionProject> allProjects,
            string outputRootDirectory = null,
            string locationInProject = null,
            OutputLocationOptions outputLocationOptions = null)
        {
            outputLocationOptions ??= OutputLocationOptions.None;

            foreach (var project in allProjects.Where(p => p.ParentFolder == null))
                AddProject(solutionModel, project, parentFolder: null, outputRootDirectory, locationInProject, outputLocationOptions);

            foreach (var folder in intentFolders)
                AddFolder(solutionModel, folder, allProjects, parentPath: "", outputRootDirectory, locationInProject, outputLocationOptions);
        }

        private static void AddFolder(
            SolutionModel solutionModel,
            Api.SolutionFolderModel intentFolder,
            IReadOnlyList<IVisualStudioSolutionProject> allProjects,
            string parentPath,
            string outputRootDirectory,
            string locationInProject,
            OutputLocationOptions outputLocationOptions)
        {
            var folderPath = $"{parentPath}/{intentFolder.Name}/";
            var slnxFolder = solutionModel.AddFolder(folderPath);
            if (Guid.TryParse(intentFolder.Id, out var folderId))
                slnxFolder.Id = folderId;

            foreach (var project in allProjects.Where(p => p.ParentFolder?.Id == intentFolder.Id))
                AddProject(solutionModel, project, slnxFolder, outputRootDirectory, locationInProject, outputLocationOptions);

            foreach (var childFolder in intentFolder.Folders)
                AddFolder(solutionModel, childFolder, allProjects, folderPath.TrimEnd('/'), outputRootDirectory, locationInProject, outputLocationOptions);
        }

        private static void AddProject(
            SolutionModel solutionModel,
            IVisualStudioSolutionProject project,
            Microsoft.VisualStudio.SolutionPersistence.Model.SolutionFolderModel parentFolder,
            string outputRootDirectory,
            string locationInProject,
            OutputLocationOptions outputLocationOptions)
        {
            var path = GetProjectRelativePath(project, outputRootDirectory, locationInProject, outputLocationOptions);
            var slnxProject = solutionModel.AddProject(path, null, parentFolder);
            if (Guid.TryParse(project.Id, out var projectId))
                slnxProject.Id = projectId;
        }

        /// <remarks>
        /// When <paramref name="locationInProject"/> is unset, this reproduces the exact historical
        /// string format byte-for-byte. It only switches to resolving via absolute paths - which
        /// normalizes that format - once the .slnx file's own location has actually been shifted away
        /// from <paramref name="outputRootDirectory"/>, since only then do the two need reconciling.
        /// </remarks>
        private static string GetProjectRelativePath(IVisualStudioSolutionProject project, string outputRootDirectory, string locationInProject, OutputLocationOptions outputLocationOptions)
        {
            var location = outputLocationOptions.GetEffectiveRelativeLocation(project.RelativeLocation, project.Name);

            if (string.IsNullOrEmpty(locationInProject) || string.IsNullOrEmpty(outputRootDirectory))
            {
                return string.IsNullOrEmpty(location)
                    ? $"{project.Name}.{project.FileExtension}"
                    : $"{location.Replace('\\', '/')}/{project.Name}.{project.FileExtension}";
            }

            var solutionDirectory = Path.GetFullPath(Path.Combine(outputRootDirectory, locationInProject));
            var projectDirectory = Path.GetFullPath(Path.Combine(outputRootDirectory, location ?? project.Name));
            var relativeToSolution = Path.GetRelativePath(solutionDirectory, projectDirectory).Replace('\\', '/');

            return $"{relativeToSolution}/{project.Name}.{project.FileExtension}";
        }

        public IFileMetadata GetMetadata()
        {
            if (_fileMetadata == null)
                throw new Exception("File Metadata must be specified.");
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
                fileLocation: Application.OutputRootDirectory,
                fileExtension: "slnx");

            // LocationInProject (not fileLocation) is what actually moves the .slnx, always relative to
            // OutputRootDirectory - so the Root Folder shift and the legacy Solution Relative Location
            // must both be folded into this single value, not split across the two.
            var offset = GetEffectiveSolutionOffset();
            if (!string.IsNullOrEmpty(offset))
                solutionFileMetadata.LocationInProject = offset;

            return solutionFileMetadata;
        }

        private string GetSolutionFilename()
        {
            if (Model.HasVisualStudioSolutionOptions() && !string.IsNullOrWhiteSpace(Model.GetVisualStudioSolutionOptions().SolutionName()))
                return Model.GetVisualStudioSolutionOptions().SolutionName();
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
    }
}
