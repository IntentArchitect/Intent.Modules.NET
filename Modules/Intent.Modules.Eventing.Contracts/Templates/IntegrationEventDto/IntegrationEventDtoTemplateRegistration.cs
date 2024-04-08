using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Metadata.Models;
using Intent.Modelers.Eventing.Api;
using Intent.Modelers.Services.EventInteractions;
using Intent.Modules.Common;
using Intent.Modules.Common.Registrations;
using Intent.Modules.Modelers.Eventing;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.TemplateRegistration.FilePerModel", Version = "1.0")]

namespace Intent.Modules.Eventing.Contracts.Templates.IntegrationEventDto
{
    [IntentManaged(Mode.Merge, Body = Mode.Merge, Signature = Mode.Fully)]
    public class IntegrationEventDtoTemplateRegistration : FilePerModelTemplateRegistration<EventingDTOModel>
    {
        private readonly IMetadataManager _metadataManager;

        public IntegrationEventDtoTemplateRegistration(IMetadataManager metadataManager)
        {
            _metadataManager = metadataManager;
        }

        public override string TemplateId => IntegrationEventDtoTemplate.TemplateId;

        [IntentManaged(Mode.Fully)]
        public override ITemplate CreateTemplateInstance(IOutputTarget outputTarget, EventingDTOModel model)
        {
            return new IntegrationEventDtoTemplate(outputTarget, model);
        }

        [IntentManaged(Mode.Merge, Body = Mode.Ignore, Signature = Mode.Fully)]
        public override IEnumerable<EventingDTOModel> GetModels(IApplication application)
        {
            return Enumerable.Empty<EventingDTOModel>()
                .Union(_metadataManager.GetSubscribedToDtoModels(application))
                .Union(_metadataManager.GetPublishedDtoModels(application))
                .Union(_metadataManager.GetAssociatedMessageDtoModels(application));
        }
    }
}