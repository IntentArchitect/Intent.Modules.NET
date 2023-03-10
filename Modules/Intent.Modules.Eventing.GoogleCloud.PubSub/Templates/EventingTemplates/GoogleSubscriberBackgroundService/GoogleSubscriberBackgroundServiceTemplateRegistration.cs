using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Eventing.GoogleCloud.PubSub.Api;
using Intent.Metadata.Models;
using Intent.Modelers.Eventing.Api;
using Intent.Modules.Common;
using Intent.Modules.Common.Registrations;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.TemplateRegistration.SingleFileNoModel", Version = "1.0")]

namespace Intent.Modules.Eventing.GoogleCloud.PubSub.Templates.EventingTemplates.GoogleSubscriberBackgroundService
{
    [IntentManaged(Mode.Merge, Body = Mode.Merge, Signature = Mode.Fully)]
    public class GoogleSubscriberBackgroundServiceTemplateRegistration : SingleFileTemplateRegistration
    {
        private readonly IMetadataManager _metadataManager;

        public GoogleSubscriberBackgroundServiceTemplateRegistration(IMetadataManager metadataManager)
        {
            _metadataManager = metadataManager;
        }

        public override string TemplateId => GoogleSubscriberBackgroundServiceTemplate.TemplateId;

        public override ITemplate CreateTemplateInstance(IOutputTarget outputTarget)
        {
            return new GoogleSubscriberBackgroundServiceTemplate(outputTarget);
        }
    }
}