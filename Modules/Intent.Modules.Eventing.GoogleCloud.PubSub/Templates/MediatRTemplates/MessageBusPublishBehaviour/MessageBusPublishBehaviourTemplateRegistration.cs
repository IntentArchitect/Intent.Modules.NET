using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Metadata.Models;
using Intent.Modules.Common;
using Intent.Modules.Common.Registrations;
using Intent.Registrations;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.TemplateRegistration.Custom", Version = "1.0")]

namespace Intent.Modules.Eventing.GoogleCloud.PubSub.Templates.MediatRTemplates.MessageBusPublishBehaviour
{
    [IntentManaged(Mode.Merge, Body = Mode.Merge, Signature = Mode.Fully)]
    public class MessageBusPublishBehaviourTemplateRegistration : ITemplateRegistration
    {
        private readonly IMetadataManager _metadataManager;

        public MessageBusPublishBehaviourTemplateRegistration(IMetadataManager metadataManager)
        {
            _metadataManager = metadataManager;
        }

        public string TemplateId => MessageBusPublishBehaviourTemplate.TemplateId;

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public void DoRegistration(ITemplateInstanceRegistry registry, IApplication applicationManager)
        {
            if (!IntegrationCoordinator.ShouldInstallMediatRIntegration(applicationManager))
            {
                return;
            }
            registry.RegisterTemplate(TemplateId, project => new MessageBusPublishBehaviourTemplate(project, null));
        }
    }
}