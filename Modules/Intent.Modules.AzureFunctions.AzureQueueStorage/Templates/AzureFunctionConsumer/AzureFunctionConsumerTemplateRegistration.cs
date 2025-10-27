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

namespace Intent.Modules.AzureFunctions.AzureQueueStorage.Templates.AzureFunctionConsumer
{
    [IntentManaged(Mode.Merge, Body = Mode.Merge, Signature = Mode.Fully)]
    public class AzureFunctionConsumerTemplateRegistration : FilePerModelTemplateRegistration<IntegrationEventHandlerModel>
    {
        private readonly IMetadataManager _metadataManager;

        public AzureFunctionConsumerTemplateRegistration(IMetadataManager metadataManager)
        {
            _metadataManager = metadataManager;
        }

        public override string TemplateId => AzureFunctionConsumerTemplate.TemplateId;

        [IntentManaged(Mode.Fully)]
        public override ITemplate CreateTemplateInstance(IOutputTarget outputTarget, IntegrationEventHandlerModel model)
        {
            return new AzureFunctionConsumerTemplate(outputTarget, model);
        }

        [IntentManaged(Mode.Merge, Body = Mode.Ignore, Signature = Mode.Fully)]
        public override IEnumerable<IntegrationEventHandlerModel> GetModels(IApplication application)
        {
            return _metadataManager.Services(application).GetIntegrationEventHandlerModels();
        }
    }
}