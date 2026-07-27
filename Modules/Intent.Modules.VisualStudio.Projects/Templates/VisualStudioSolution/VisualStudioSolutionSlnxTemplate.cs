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

        public VisualStudioSolutionSlnxTemplate(IApplication application, VisualStudioSolutionModel model, IEnumerable<IVisualStudioSolutionProject> projects)
        {
            Application = application;
            Model = model;
            BindingContext = new TemplateBindingContext(new VisualStudioSolutionTemplateModel(Application));
            Projects = projects.ToList();
        }

        public string Id => Identifier;
        public IApplication Application { get; }
        public VisualStudioSolutionModel Model { get; }
        public IReadOnlyList<IVisualStudioSolutionProject> Projects { get; }

        public bool CanRunTemplate() => true;

        public string RunTemplate()
        {
            var solutionModel = new SolutionModel();
            SyncFoldersAndProjects(solutionModel, Model.Folders, Projects);

            using var stream = new MemoryStream();
            SolutionSerializers.SlnXml.SaveAsync(stream, solutionModel, CancellationToken.None).GetAwaiter().GetResult();
            return Encoding.UTF8.GetString(stream.ToArray());
        }

        /// <remarks>
        /// <see langword="internal"/> so it can be unit tested directly.
        /// </remarks>
        internal static void SyncFoldersAndProjects(
            SolutionModel solutionModel,
            IEnumerable<Api.SolutionFolderModel> intentFolders,
            IReadOnlyList<IVisualStudioSolutionProject> allProjects)
        {
            foreach (var project in allProjects.Where(p => p.ParentFolder == null))
                AddProject(solutionModel, project, parentFolder: null);

            foreach (var folder in intentFolders)
                AddFolder(solutionModel, folder, allProjects, parentPath: "");
        }

        private static void AddFolder(
            SolutionModel solutionModel,
            Api.SolutionFolderModel intentFolder,
            IReadOnlyList<IVisualStudioSolutionProject> allProjects,
            string parentPath)
        {
            var folderPath = $"{parentPath}/{intentFolder.Name}/";
            var slnxFolder = solutionModel.AddFolder(folderPath);
            if (Guid.TryParse(intentFolder.Id, out var folderId))
                slnxFolder.Id = folderId;

            foreach (var project in allProjects.Where(p => p.ParentFolder?.Id == intentFolder.Id))
                AddProject(solutionModel, project, slnxFolder);

            foreach (var childFolder in intentFolder.Folders)
                AddFolder(solutionModel, childFolder, allProjects, folderPath.TrimEnd('/'));
        }

        private static void AddProject(
            SolutionModel solutionModel,
            IVisualStudioSolutionProject project,
            Microsoft.VisualStudio.SolutionPersistence.Model.SolutionFolderModel parentFolder)
        {
            var path = GetProjectRelativePath(project);
            var slnxProject = solutionModel.AddProject(path, null, parentFolder);
            if (Guid.TryParse(project.Id, out var projectId))
                slnxProject.Id = projectId;
        }

        private static string GetProjectRelativePath(IVisualStudioSolutionProject project)
        {
            var location = project.ToOutputTargetConfig().RelativeLocation;
            return string.IsNullOrEmpty(location)
                ? $"{project.Name}.{project.FileExtension}"
                : $"{location.Replace('\\', '/')}/{project.Name}.{project.FileExtension}";
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

            if (Model.HasVisualStudioSolutionOptions() && !string.IsNullOrWhiteSpace(Model.GetVisualStudioSolutionOptions().SolutionRelativeLocation()))
                solutionFileMetadata.LocationInProject = Model.GetVisualStudioSolutionOptions().SolutionRelativeLocation();

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
