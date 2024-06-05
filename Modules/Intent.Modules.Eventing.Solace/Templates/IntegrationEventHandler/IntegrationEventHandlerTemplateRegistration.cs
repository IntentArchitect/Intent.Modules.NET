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

namespace Intent.Modules.Eventing.Solace.Templates.IntegrationEventHandler
{
    [IntentManaged(Mode.Merge, Body = Mode.Merge, Signature = Mode.Fully)]
    public class IntegrationEventHandlerTemplateRegistration : FilePerModelTemplateRegistration<IntegrationEventHandlerModel>
    {
        private readonly IMetadataManager _metadataManager;

        public IntegrationEventHandlerTemplateRegistration(IMetadataManager metadataManager)
        {
            _metadataManager = metadataManager;
        }

        public override string TemplateId => IntegrationEventHandlerTemplate.TemplateId;

        [IntentManaged(Mode.Fully)]
        public override ITemplate CreateTemplateInstance(IOutputTarget outputTarget, IntegrationEventHandlerModel model)
        {
            return new IntegrationEventHandlerTemplate(outputTarget, model);
        }

        [IntentManaged(Mode.Merge, Body = Mode.Ignore, Signature = Mode.Fully)]
        public override IEnumerable<IntegrationEventHandlerModel> GetModels(IApplication application)
        {
            return _metadataManager.Services(application).GetIntegrationEventHandlerModels();
        }
    }
}