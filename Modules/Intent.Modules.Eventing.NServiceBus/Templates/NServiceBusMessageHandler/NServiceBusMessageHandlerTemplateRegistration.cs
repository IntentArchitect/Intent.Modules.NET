using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Metadata.Models;
using Intent.Modelers.Services.Api;
using Intent.Modelers.Services.EventInteractions;
using Intent.Modules.Common;
using Intent.Modules.Common.Registrations;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.TemplateRegistration.FilePerModel", Version = "1.0")]

namespace Intent.Modules.Eventing.NServiceBus.Templates.NServiceBusMessageHandler
{
    [IntentManaged(Mode.Merge, Body = Mode.Merge, Signature = Mode.Merge)]
    public class NServiceBusMessageHandlerTemplateRegistration : FilePerModelTemplateRegistration<IList<IntegrationEventHandlerModel>>
    {
        private readonly IMetadataManager _metadataManager;

        public NServiceBusMessageHandlerTemplateRegistration(IMetadataManager metadataManager)
        {
            _metadataManager = metadataManager;
        }

        public override string TemplateId => NServiceBusMessageHandlerTemplate.TemplateId;

        [IntentManaged(Mode.Merge)]
        public override ITemplate CreateTemplateInstance(IOutputTarget outputTarget, IList<IntegrationEventHandlerModel> model)
        {
            return new NServiceBusMessageHandlerTemplate(outputTarget, model);
        }

        [IntentManaged(Mode.Merge, Body = Mode.Ignore, Signature = Mode.Merge)]
        public override IEnumerable<IList<IntegrationEventHandlerModel>> GetModels(IApplication application)
        {
            return _metadataManager.Services(application)
                .GetIntegrationEventHandlerModels()
                .GroupBy(h => h.InternalElement.ParentElement.Id)
                .Select(g => (IList<IntegrationEventHandlerModel>)g.ToList());
        }
    }
}