using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using Intent.Engine;
using Intent.Metadata.Models;
using Intent.Modules.Common;
using Intent.Modules.Common.Templates;
using Intent.Modules.VisualStudio.Projects.Api;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.ProjectItemTemplate.Partial", Version = "1.0")]

namespace Intent.Modules.VisualStudio.Projects.Templates.ServiceFabric.ServiceManifest
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    partial class ServiceManifestTemplate : IntentTemplateBase<CSharpProjectNETModel>
    {
        [IntentManaged(Mode.Fully)]
        public const string TemplateId = "Intent.VisualStudio.Projects.ServiceFabric.ServiceManifest";

        [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
        public ServiceManifestTemplate(IOutputTarget outputTarget, CSharpProjectNETModel model) : base(TemplateId, outputTarget, model)
        {
            IsAspNetProject = Model.GetNETSettings().SDK().IsMicrosoftNETSdkWeb();
        }

        public XmlDocument Document { get; private set; }

        private bool IsAspNetProject { get; }
    
        public override void AfterTemplateRegistration()
        {
            if (!TryGetExistingFileContent(out var existingFileContent))
            {
                existingFileContent = TransformText().ReplaceLineEndings();
            }

            Document = new XmlDocument
            {
                PreserveWhitespace = true,
            };
            Document.LoadXml(existingFileContent);

            base.AfterTemplateRegistration();
        }

        public override string RunTemplate()
        {
            return Document.ToUtf8String();
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public override ITemplateFileConfig GetTemplateFileConfig()
        {
            return new TemplateFileConfig(
                relativeLocation: "PackageRoot",
                fileName: $"ServiceManifest",
                fileExtension: "xml"
            );
        }

    }
}