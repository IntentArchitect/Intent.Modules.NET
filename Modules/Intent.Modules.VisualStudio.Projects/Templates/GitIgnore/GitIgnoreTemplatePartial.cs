using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Intent.Engine;
using Intent.Metadata.Models;
using Intent.Modules.Common;
using Intent.Modules.Common.Templates;
using Intent.Modules.VisualStudio.Projects.Api;
using Intent.Modules.VisualStudio.Projects.Settings;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Ignore)]
[assembly: DefaultIntentManaged(Mode.Ignore, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.ModuleBuilder.ProjectItemTemplate.Partial", Version = "1.0")]

namespace Intent.Modules.VisualStudio.Projects.Templates.GitIgnore
{
    partial class GitIgnoreTemplate : ITemplate
    {
        [IntentManaged(Mode.Fully)]
        public const string TemplateId = "Intent.VisualStudio.Projects.GitIgnore";

        private readonly bool _canRunTemplate;
        private readonly IFileMetadata _fileMetadata;

        [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
        public GitIgnoreTemplate(IApplication application, VisualStudioSolutionModel model)
        {
            _canRunTemplate = application.Settings.GetVisualStudioSettings().GenerateGitignoreFile();
            _fileMetadata = new FileConfig(application.OutputRootDirectory, model.Id);

            Model = model;
        }

        public VisualStudioSolutionModel Model { get; }

        public string Id => Model.Id;

        public bool CanRunTemplate() => _canRunTemplate;

        public IFileMetadata GetMetadata() => _fileMetadata ?? throw new InvalidOperationException($"{nameof(_fileMetadata)} is null");

        public string RunTemplate() => GetGitIgnoreContent();

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
            public string FileExtension => "gitIgnore";
            public OverwriteBehaviour OverwriteBehaviour => OverwriteBehaviour.OverwriteDisabled;
            public string FileName { get; set; } = string.Empty;
            public string LocationInProject { get; set; } = string.Empty;
            public IDictionary<string, string> CustomMetadata { get; } = new Dictionary<string, string>();
        }

    }
}