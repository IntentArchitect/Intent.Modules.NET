using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Metadata.Models;
using Intent.Modelers.Eventing.Api;
using Intent.Modelers.Services.EventInteractions;
using Intent.Modules.Common;
using Intent.Modules.Common.Registrations;
using Intent.Modules.Common.Types.Api;
using Intent.Modules.Modelers.Eventing;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.TemplateRegistration.FilePerModel", Version = "1.0")]

namespace Intent.Modules.Eventing.Contracts.Templates.IntegrationEventEnum
{
    [IntentManaged(Mode.Merge, Body = Mode.Merge, Signature = Mode.Fully)]
    public class IntegrationEventEnumTemplateRegistration : FilePerModelTemplateRegistration<EnumModel>
    {
        private readonly IMetadataManager _metadataManager;

        public IntegrationEventEnumTemplateRegistration(IMetadataManager metadataManager)
        {
            _metadataManager = metadataManager;
        }

        public override string TemplateId => IntegrationEventEnumTemplate.TemplateId;

        [IntentManaged(Mode.Fully)]
        public override ITemplate CreateTemplateInstance(IOutputTarget outputTarget, EnumModel model)
        {
            return new IntegrationEventEnumTemplate(outputTarget, model);
        }

        [IntentManaged(Mode.Merge, Body = Mode.Ignore, Signature = Mode.Fully)]
        public override IEnumerable<EnumModel> GetModels(IApplication application)
        {
            return Enumerable.Empty<EnumModel>()
                .Union(_metadataManager.GetSubscribedToEnumModels(application))
                .Union(_metadataManager.GetPublishedEnumModels(application))
                .Union(_metadataManager.GetAssociatedMessageEnumModels(application))
                .Except(_metadataManager
                    .GetDesigner(application.Id, "Domain")
                    .GetEnumModels())
                /*
                .Except(_metadataManager
                    .GetDesigner(application.Id, "Services")
                    .GetEnumModels())*/
                .ToArray();
        }
    }
}