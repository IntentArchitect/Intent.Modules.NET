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

        public const string TemplateId = "Intent.VisualStudio.Projects.DirectoryPackagesProps";

        public DirectoryPackagesPropsTemplate(IApplication application, VisualStudioSolutionModel model)
        {
            _application = application;
            _fileMetadata = new FileConfig(application.OutputRootDirectory, model.Id);
            _projectRootElement = CreateProjectRootElement(File.Exists(_fileMetadata.GetFilePath())
                ? File.ReadAllText(_fileMetadata.GetFilePath())
                : GetInitialContent());
            _canRunTemplate = model.GetVisualStudioSolutionOptions()?.ManagePackageVersionsCentrally() == true;

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

            var item = _projectRootElement.Items.FirstOrDefault(x => x.ItemType == "PackageVersion" && string.Equals(x.Include, packageId, StringComparison.OrdinalIgnoreCase));
            if (item == null)
            {
                version = default;
                return false;
            }

            var versionMetadata = item.Metadata.SingleOrDefault(x => string.Equals("Version", x.Name, StringComparison.OrdinalIgnoreCase));
            if (versionMetadata == null)
            {
                version = default;
                return false;
            }

            version = versionMetadata.Value;
            return true;
        }

        public void SetPackageVersion(
            string packageId,
            string packageVersion,
            ISoftwareFactoryEventDispatcher sfEventDispatcher)
        {
            var item = _projectRootElement.Items.FirstOrDefault(x => x.ItemType == "PackageVersion" && string.Equals(x.Include, packageId, StringComparison.OrdinalIgnoreCase)) ??
                       _projectRootElement.AddItem("PackageVersion", packageId);

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
            if (!CanRunTemplate())
            {
                return;
            }

            var filePath = _fileMetadata.GetFilePath();

            var change = _application.ChangeManager.FindChange(filePath);
            if (change != null)
            {
                change.ChangeContent(_projectRootElement.RawXml);
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

        #region ITemplate implementation

        public IFileMetadata GetMetadata() => _fileMetadata ?? throw new InvalidOperationException($"{nameof(_fileMetadata)} is null");
        public string RunTemplate() => _projectRootElement.RawXml.ReplaceLineEndings();
        public string Id => TemplateId;

        private class FileConfig : IFileMetadata, ITemplateFileConfig
        {
            private readonly string _fullLocationPath;

            public FileConfig(string fullLocationPath, string modelId)
            {
                _fullLocationPath = fullLocationPath;

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