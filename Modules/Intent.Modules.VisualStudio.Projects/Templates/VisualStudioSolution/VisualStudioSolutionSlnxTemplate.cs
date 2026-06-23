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
            var targetFile = GetMetadata().GetFilePath();

            var slnxModel = File.Exists(targetFile)
                ? ReadExisting(targetFile)
                : new SolutionModel();

            SyncFoldersAndProjects(slnxModel, Model.Folders, Projects.ToList());

            var serializer = SolutionSerializers.SlnXml;
            using var stream = new MemoryStream();
            serializer.SaveAsync(stream, slnxModel, CancellationToken.None).GetAwaiter().GetResult();
            return Encoding.UTF8.GetString(stream.ToArray());
        }

        private static SolutionModel ReadExisting(string targetFile)
        {
            using var stream = File.OpenRead(targetFile);
            return SolutionSerializers.SlnXml.OpenAsync(stream, CancellationToken.None).GetAwaiter().GetResult();
        }

        internal static void SyncFoldersAndProjects(
            SolutionModel slnxModel,
            IEnumerable<Api.SolutionFolderModel> intentFolders,
            IReadOnlyList<IVisualStudioSolutionProject> allProjects)
        {
            var intentProjectPaths = new HashSet<string>(
                allProjects.Select(GetProjectRelativePath),
                StringComparer.OrdinalIgnoreCase);

            // Projects and folders are NOT removed when absent from the Intent model so that
            // manually-added projects and solution folders are preserved across SF runs.

            // Add root-level projects (no parent folder)
            foreach (var project in allProjects.Where(p => p.ParentFolder == null))
            {
                var path = GetProjectRelativePath(project);
                if (slnxModel.SolutionProjects.All(p => !string.Equals(p.FilePath, path, StringComparison.OrdinalIgnoreCase)))
                    slnxModel.AddProject(path);
            }

            // Add folders and their projects recursively
            foreach (var folder in intentFolders)
                SyncIntentFolder(slnxModel, folder, allProjects, parentPath: "");
        }

        private static void SyncIntentFolder(
            SolutionModel slnxModel,
            Api.SolutionFolderModel intentFolder,
            IReadOnlyList<IVisualStudioSolutionProject> allProjects,
            string parentPath)
        {
            var folderPath = $"{parentPath}/{intentFolder.Name}/";

            var slnxFolder = slnxModel.SolutionFolders.FirstOrDefault(
                f => string.Equals(f.Path, folderPath, StringComparison.OrdinalIgnoreCase))
                ?? slnxModel.AddFolder(folderPath);

            foreach (var project in allProjects.Where(p => p.ParentFolder?.Id == intentFolder.Id))
            {
                var path = GetProjectRelativePath(project);
                var alreadyExists = slnxModel.SolutionProjects.Any(p =>
                    string.Equals(p.FilePath, path, StringComparison.OrdinalIgnoreCase) ||
                    (p.Parent == slnxFolder && string.Equals(
                        Path.GetFileNameWithoutExtension(p.FilePath),
                        project.Name,
                        StringComparison.OrdinalIgnoreCase)));
                if (!alreadyExists)
                    slnxModel.AddProject(path, null, slnxFolder);
            }

            foreach (var childFolder in intentFolder.Folders)
                SyncIntentFolder(slnxModel, childFolder, allProjects, folderPath.TrimEnd('/'));
        }

        private static IEnumerable<string> GetAllFolderPaths(IEnumerable<Api.SolutionFolderModel> folders, string parentPath)
        {
            foreach (var folder in folders)
            {
                var path = $"{parentPath}/{folder.Name}/";
                yield return path;
                foreach (var child in GetAllFolderPaths(folder.Folders, path.TrimEnd('/')))
                    yield return child;
            }
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
