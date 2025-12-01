using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using Intent.Engine;
using Intent.Metadata.Models;
using Intent.Modules.Common;
using Intent.Modules.Common.Templates;
using Intent.Modules.VisualStudio.Projects.Api;
using Intent.Modules.VisualStudio.Projects.Events.ServiceFabric;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.ProjectItemTemplate.Partial", Version = "1.0")]

namespace Intent.Modules.VisualStudio.Projects.Templates.ServiceFabric.StartupServices
{
    [IntentManaged(Mode.Merge, Signature = Mode.Merge)]
    partial class StartupServicesTemplate : IntentTemplateBase<ServiceFabricProjectModel>, IXmlDocumentTemplate
    {
        [IntentManaged(Mode.Fully)]
        public const string TemplateId = "Intent.VisualStudio.Projects.ServiceFabric.StartupServices";

        private readonly List<Action> _onAfterTemplateRegistrations = [];

        [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
        public StartupServicesTemplate(IOutputTarget outputTarget, ServiceFabricProjectModel model) : base(TemplateId, outputTarget, model)
        {
            OutputTarget.On<ServiceRegistrationRequiredBase>(e =>
            {
                if (Document != null)
                {
                    OnServiceRegistrationRequired(e.Data);
                }
                else
                {
                    _onAfterTemplateRegistrations.Add(() => OnServiceRegistrationRequired(e.Data));
                }
            });
        }

        public XmlDocument Document { get; private set; }

        public XmlNamespaceManager NamespaceManager { get; private set; }

        public string Namespace => "http://schemas.microsoft.com/2011/01/fabric";

        private void OnServiceRegistrationRequired(ServiceRegistrationRequiredBase @event)
        {
            var existing = Document.SelectSingleNode(
                $"/f:StartupServicesManifest/f:Services/f:Service[@Name='{@event.Name}']",
                NamespaceManager);

            if (existing != null)
            {
                return;
            }

            var serviceElement = Helpers.CreateServiceElement(@event: @event, template: this);

            serviceElement.AppendChild(Document.CreateWhitespace($"{Environment.NewLine}    "));

            var servicesNode = Document.SelectSingleNode("/f:StartupServicesManifest/f:Services", NamespaceManager)!;
            servicesNode.AppendChild(Document.CreateWhitespace("  "));
            servicesNode.AppendChild(serviceElement);
            servicesNode.AppendChild(Document.CreateWhitespace($"{Environment.NewLine}  "));
        }

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
            NamespaceManager = new XmlNamespaceManager(Document.NameTable);
            NamespaceManager.AddNamespace("f", Namespace);

            foreach (var action in _onAfterTemplateRegistrations)
            {
                action();
            }

            _onAfterTemplateRegistrations.Clear();

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
                fileName: $"StartupServices",
                fileExtension: "xml"
            );
        }
    }
}