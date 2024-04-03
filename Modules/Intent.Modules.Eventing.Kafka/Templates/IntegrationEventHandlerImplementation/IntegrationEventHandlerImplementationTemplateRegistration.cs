using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Metadata.Models;
using Intent.Modelers.Eventing.Api;
using Intent.Modules.Common;
using Intent.Modules.Common.Registrations;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.TemplateRegistration.FilePerModel", Version = "1.0")]

namespace Intent.Modules.Eventing.Kafka.Templates.IntegrationEventHandlerImplementation
{
    [IntentManaged(Mode.Merge, Body = Mode.Merge, Signature = Mode.Fully)]
    public class IntegrationEventHandlerImplementationTemplateRegistration : FilePerModelTemplateRegistration<MessageSubscribeAssocationTargetEndModel>
    {
        private readonly IMetadataManager _metadataManager;

        public IntegrationEventHandlerImplementationTemplateRegistration(IMetadataManager metadataManager)
        {
            _metadataManager = metadataManager;
        }

        public override string TemplateId => IntegrationEventHandlerImplementationTemplate.TemplateId;

        [IntentManaged(Mode.Fully)]
        public override ITemplate CreateTemplateInstance(IOutputTarget outputTarget, MessageSubscribeAssocationTargetEndModel model)
        {
            return new IntegrationEventHandlerImplementationTemplate(outputTarget, model);
        }

        [IntentManaged(Mode.Merge, Body = Mode.Ignore, Signature = Mode.Fully)]
        public override IEnumerable<MessageSubscribeAssocationTargetEndModel> GetModels(IApplication application)
        {
            return _metadataManager.Eventing(application).GetApplicationModels().SelectMany(s => s.SubscribedMessages());
        }
    }
}