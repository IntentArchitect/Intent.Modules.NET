using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using Intent.Engine;
using Intent.Eventing;
using Intent.Modules.Common.Templates;
using Intent.Modules.Constants;
using Intent.Modules.VisualStudio.Projects.Api;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;
using Microsoft.Build.Construction;
using Microsoft.Build.Evaluation;

[assembly: DefaultIntentManaged(Mode.Ignore)]
[assembly: DefaultIntentManaged(Mode.Ignore, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.ModuleBuilder.ProjectItemTemplate.Partial", Version = "1.0")]

namespace Intent.Modules.VisualStudio.Projects.Templates.DirectoryPackagesProps
{
    partial class DirectoryPackagesPropsTemplate : ITemplate, ICpmTemplate
    {
        private readonly IApplication _application;
        private readonly ProjectRootElement _projectRootElement;
        private readonly bool _canRunTemplate;
        private readonly IFileMetadata _fileMetadata;
        private readonly IFileOperations _fileOperations;
        private Dictionary<string, string> _cachedPackageVersions;

        public const string TemplateId = "Intent.VisualStudio.Projects.DirectoryPackagesProps";

        public DirectoryPackagesPropsTemplate(IApplication application, VisualStudioSolutionModel model) : this(
            application: application, 
            model: model, 
            canRunTemplate: model.GetVisualStudioSolutionOptions()?.ManagePackageVersionsCentrally() == true, 
            fileOperations: new StandardFileOperations())
        {
        }

        // Allow for testability
        internal DirectoryPackagesPropsTemplate(IApplication application, VisualStudioSolutionModel model, bool canRunTemplate, IFileOperations fileOperations)
        {
            _fileOperations = fileOperations;
            _application = application;
            _fileMetadata = new FileConfig(application.OutputRootDirectory, model.Id, _fileOperations);
            _projectRootElement = CreateProjectRootElement(_fileOperations.FileExists(_fileMetadata.GetFilePath())
                ? _fileOperations.ReadAllText(_fileMetadata.GetFilePath())
                : GetInitialContent());
            _canRunTemplate = canRunTemplate;
            
            Model = model;
        }

        public VisualStudioSolutionModel Model { get; }
        
        public bool TryGetVersion(string packageId, out string version)
        {
            if (!_canRunTemplate)
            {
                version = default;
                return false;
            }

            _cachedPackageVersions ??= GetAllPrecomputedPackageVersions(_projectRootElement, Path.GetDirectoryName(_fileMetadata.GetFilePath()));

            return _cachedPackageVersions.TryGetValue(packageId, out version);
        }

        public void SetPackageVersion(
            string packageId,
            string packageVersion,
            ISoftwareFactoryEventDispatcher sfEventDispatcher)
        {
            var item = _projectRootElement.Items.FirstOrDefault(x => x.ItemType == "PackageVersion" && string.Equals(x.Include, packageId, StringComparison.OrdinalIgnoreCase));
            switch (item)
            {
                case null when TryGetVersion(packageId, out var importedPackageVersion):
                {
                    if (importedPackageVersion != packageVersion)
                    {
                        Utils.Logging.Log.Warning(
                            $"Nuget Package {packageId} with version {packageVersion} differs from one imported in the Directory.Packages.props file. Imported package version: {importedPackageVersion}.");
                    }
                    return;
                }
                case null:
                    item = _projectRootElement.AddItem("PackageVersion", packageId);
                    break;
            }

            var metadata = item.Metadata.SingleOrDefault(x => string.Equals(x.Name, "Version", StringComparison.OrdinalIgnoreCase));
            if (metadata == null)
            {
                item.AddMetadata("Version", packageVersion, expressAsAttribute: true);
            }
            else
            {
                metadata.Value = packageVersion;
            }

            UpdateChangeIfNeeded(sfEventDispatcher);
        }

        public void Remove(string packageId, ISoftwareFactoryEventDispatcher sfEventDispatcher)
        {
            var item = _projectRootElement.Items.FirstOrDefault(x => x.ItemType == "PackageVersion" && string.Equals(x.Include, packageId, StringComparison.OrdinalIgnoreCase));
            if (item == null)
            {
                if (_cachedPackageVersions.TryGetValue(packageId, out var importedPackageVersion))
                {
                    Utils.Logging.Log.Warning(
                        $"Nuget Package {packageId} is indicated to be removed from the project, though it is still present in one of the imports from the Directory.Packages.props file.");
                }
                return;
            }

            var parent = item.Parent;
            parent.RemoveChild(item);

            if (parent.Count == 0)
            {
                parent.Parent.RemoveChild(parent);
            }

            UpdateChangeIfNeeded(sfEventDispatcher);
        }

        public bool CanRunTemplate() => _canRunTemplate;

        public string SolutionModelId => Model.Id;
        
        private void UpdateChangeIfNeeded(ISoftwareFactoryEventDispatcher sfEventDispatcher)
        {
            _cachedPackageVersions = null; // Recompute Packaged Versions on next TryGetVersion
            
            if (!CanRunTemplate())
            {
                return;
            }

            var filePath = _fileMetadata.GetFilePath();

            var change = _application.ChangeManager.FindChange(filePath);
            if (change != null)
            {
                change.ChangeContent(_projectRootElement.RawXml, _projectRootElement.RawXml);
                return;
            }

            sfEventDispatcher.Publish(new SoftwareFactoryEvent(SoftwareFactoryEvents.OverwriteFileCommand, new Dictionary<string, string>
            {
                ["FullFileName"] = filePath,
                ["Context"] = ToString(),
                ["Content"] = _projectRootElement.RawXml
            }));
        }

        private static ProjectRootElement CreateProjectRootElement(string content)
        {
            return ProjectRootElement.Create(
                XmlReader.Create(new StringReader(content)),
                ProjectCollection.GlobalProjectCollection,
                preserveFormatting: true);
        }
        
        private Dictionary<string, string> GetAllPrecomputedPackageVersions(ProjectRootElement project, string projectDirectory)
        {
            var precomputedPackageVersions = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            PrecomputePackageVersions(project, precomputedPackageVersions);
            foreach (var importedProject in LoadImportedProjects(project, projectDirectory, []))
            {
                PrecomputePackageVersions(importedProject, precomputedPackageVersions);
            }
            return precomputedPackageVersions;
            
            void PrecomputePackageVersions(ProjectRootElement project, Dictionary<string, string> dict)
            {
                foreach (var item in project.Items.Where(x => x.ItemType == "PackageVersion"))
                {
                    var versionMetadata = item.Metadata.SingleOrDefault(x => string.Equals("Version", x.Name, StringComparison.OrdinalIgnoreCase));
                    if (versionMetadata != null)
                    {
                        dict[item.Include] = versionMetadata.Value;
                    }
                }
            }
            
            IEnumerable<ProjectRootElement> LoadImportedProjects(ProjectRootElement projectRootElement, string projectDirectory, HashSet<string> loadedProjects)
            {
                var importedProjects = new List<ProjectRootElement>();

                foreach (var importItem in projectRootElement.Imports)
                {
                    var importPath = importItem.Project;
                    if (!Path.IsPathRooted(importPath))
                    {
                        importPath = Path.Combine(projectDirectory, importPath);
                    }
                    //Try and avoid possible circular references
                    if (!loadedProjects.Add(importPath))
                    {
                        continue;
                    }
                    if (!_fileOperations.FileExists(importPath))
                    {
                        continue;
                    }
                    var importedProject = CreateProjectRootElement(_fileOperations.ReadAllText(importPath));
                    importedProjects.Add(importedProject);
                    importedProjects.AddRange(LoadImportedProjects(importedProject, Path.GetDirectoryName(importPath), loadedProjects));
                }

                return importedProjects;
            }
        }


        #region ITemplate implementation

        public IFileMetadata GetMetadata() => _fileMetadata ?? throw new InvalidOperationException($"{nameof(_fileMetadata)} is null");
        public string RunTemplate() => _projectRootElement.RawXml.ReplaceLineEndings();
        public string Id => TemplateId;

        private class FileConfig : IFileMetadata, ITemplateFileConfig
        {
            private readonly string _fullLocationPath;

            public FileConfig(string fullLocationPath, string modelId, IFileOperations fileOperations)
            {
                var currentDir = fullLocationPath;

                while (true)
                {
                    if (string.IsNullOrWhiteSpace(currentDir))
                    {
                        _fullLocationPath = fullLocationPath;
                        break;
                    }

                    var pathToCheck = Path.Combine(currentDir, $"{FileName}.{FileExtension}");
                    if (fileOperations.FileExists(pathToCheck))
                    {
                        _fullLocationPath = currentDir;
                        break;
                    }

                    currentDir = Directory.GetParent(currentDir)?.FullName;
                }

                CustomMetadata.Add("CorrelationId", $"{TemplateId}#{modelId}");
            }

            public string GetFullLocationPath() => Path.GetFullPath(_fullLocationPath).Replace(@"\", "/");
            public string GetRelativeFilePath() => string.Empty;
            public string CodeGenType => Common.CodeGenType.UserControlledWeave;
            public string FileExtension => "props";
            public OverwriteBehaviour OverwriteBehaviour => OverwriteBehaviour.Always;
            public string FileName { get; set; } = "Directory.Packages";
            public string LocationInProject { get; set; } = string.Empty;
            public IDictionary<string, string> CustomMetadata { get; } = new Dictionary<string, string>();
        }

        #endregion
    }

    // Allow for testability
    internal interface IFileOperations
    {
        bool FileExists(string path);
        string ReadAllText(string path);
    }

    internal class StandardFileOperations : IFileOperations
    {
        public bool FileExists(string path)
        {
            return File.Exists(path);
        }

        public string ReadAllText(string path)
        {
            return File.ReadAllText(path);
        }
    }
    
    public interface ICpmTemplate : ITemplate
    {
        string SolutionModelId { get; }
        bool TryGetVersion(string packageId, out string version);
        void SetPackageVersion(
            string packageId,
            string packageVersion,
            ISoftwareFactoryEventDispatcher sfEventDispatcher);
    }
}