using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Linq;
using Intent.Engine;
using Intent.Eventing;
using Intent.Modules.Common;
using Intent.Modules.Common.Templates;
using Intent.Modules.Common.VisualStudio;
using Intent.Modules.Constants;
using Intent.Modules.VisualStudio.Projects.Api;
using Intent.Modules.VisualStudio.Projects.Events;
using Intent.Templates;
using JetBrains.Annotations;

namespace Intent.Modules.VisualStudio.Projects.Templates
{
    public abstract class VisualStudioProjectTemplateBase : IntentFileTemplateBase<IVisualStudioProject>, ITemplate, IVisualStudioProjectTemplate
    {
        private string _fileContent;
        protected VisualStudioProjectTemplateBase([NotNull] string templateId, [NotNull] IOutputTarget project, [NotNull] IVisualStudioProject model) : base(templateId, project, model)
        {
        }

        public string ProjectId => Model.Id;
        public string Name => Model.Name;
        public string FilePath => GetMetadata().GetFilePath();
        IVisualStudioProject IVisualStudioProjectTemplate.Project => Model;

        public string LoadContent()
        {
            var change = ExecutionContext.ChangeManager.FindChange(FilePath);

            return (change != null
                ? change.Content
                : _fileContent ??= File.ReadAllText(Path.GetFullPath(FilePath)));
        }

        public void UpdateContent(string content, ISoftwareFactoryEventDispatcher sfEventDispatcher)
        {
            // Normalize the content of both by parsing with no whitespace and calling .ToString()
            var targetContent = XDocument.Parse(content).ToString();
            var existingContent = LoadContent();

            if (existingContent == targetContent)
            {
                return;
            }

            var change = ExecutionContext.ChangeManager.FindChange(FilePath);
            if (change != null)
            {
                change.ChangeContent(content);
                return;
            }

            sfEventDispatcher.Publish(new SoftwareFactoryEvent(SoftwareFactoryEvents.OverwriteFileCommand, new Dictionary<string, string>
            {
                { "FullFileName", FilePath },
                { "Content", content },
            }));
        }

        public IEnumerable<INugetPackageInfo> RequestedNugetPackages()
        {
            return OutputTarget.NugetPackages();
        }

        public IEnumerable<string> GetTargetFrameworks()
        {
            return Model.TargetFrameworkVersion();
        }

        public override void OnCreated()
        {
            base.OnCreated();
            Project.Application.EventDispatcher.Publish(new VisualStudioProjectCreatedEvent(Project.Id, this));
        }

        public override ITemplateFileConfig GetTemplateFileConfig()
        {
            return new TemplateFileConfig(
                overwriteBehaviour: OverwriteBehaviour.OnceOff,
                codeGenType: CodeGenType.Basic,
                fileName: Project.Name,
                fileExtension: "csproj",
                relativeLocation: ""
            );
        }

        public override string RunTemplate()
        {
            if (GetTemplateFileConfig().OverwriteBehaviour != OverwriteBehaviour.OnceOff)
            {
                // Unless onceOff, then on subsequent SF runs, the SF shows two outputs for the same .csproj file.
                throw new Exception("Template must be configured with OverwriteBehaviour.OnceOff.");
            }

            return TransformText();
        }
    }
}