using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Metadata.Models;
using Intent.Modelers.Eventing.Api;
using Intent.Modelers.Services.Api;
using Intent.Modelers.Services.EventInteractions;
using Intent.Modules.Common;
using Intent.Modules.Common.Registrations;
using Intent.Modules.Modelers.Eventing;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.TemplateRegistration.FilePerModel", Version = "1.0")]

namespace Intent.Modules.Eventing.Contracts.Templates.IntegrationEventMessage
{
    [IntentManaged(Mode.Merge, Body = Mode.Merge, Signature = Mode.Fully)]
    public class IntegrationEventMessageTemplateRegistration : FilePerModelTemplateRegistration<MessageModel>
    {
        private readonly IMetadataManager _metadataManager;

        public IntegrationEventMessageTemplateRegistration(IMetadataManager metadataManager)
        {
            _metadataManager = metadataManager;
        }

        public override string TemplateId => IntegrationEventMessageTemplate.TemplateId;

        public override ITemplate CreateTemplateInstance(IOutputTarget outputTarget, MessageModel model)
        {
            return new IntegrationEventMessageTemplate(outputTarget, model);
        }

        [IntentManaged(Mode.Merge, Body = Mode.Ignore, Signature = Mode.Fully)]
        public override IEnumerable<MessageModel> GetModels(IApplication application)
        {
            return _metadataManager.GetSubscribedToMessageModels(application)
                .Union(_metadataManager.GetPublishedMessageModels(application))
                .Union(_metadataManager.Eventing(application).GetMessageModels()
                    .Where(x => x.IntegrationEventHandlers().Any() || x.IntegrationEventsSources().Any()))
                .Union(_metadataManager.Services(application).GetMessageModels()
                    .Where(x => x.IntegrationEventHandlers().Any() || x.IntegrationEventsSources().Any()));
        }
    }
}