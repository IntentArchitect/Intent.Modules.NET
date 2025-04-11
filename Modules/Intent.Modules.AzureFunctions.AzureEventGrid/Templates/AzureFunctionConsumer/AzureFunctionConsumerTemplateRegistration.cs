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
using Intent.Modules.Eventing.AzureEventGrid.Templates;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.TemplateRegistration.FilePerModel", Version = "1.0")]

namespace Intent.Modules.AzureFunctions.AzureEventGrid.Templates.AzureFunctionConsumer
{
    [IntentManaged(Mode.Merge, Body = Mode.Merge, Signature = Mode.Fully)]
    public class AzureFunctionConsumerTemplateRegistration : FilePerModelTemplateRegistration<AzureFunctionSubscriptionModel>
    {
        private readonly IMetadataManager _metadataManager;

        public AzureFunctionConsumerTemplateRegistration(IMetadataManager metadataManager)
        {
            _metadataManager = metadataManager;
        }

        public override string TemplateId => AzureFunctionConsumerTemplate.TemplateId;

        [IntentManaged(Mode.Fully)]
        public override ITemplate CreateTemplateInstance(IOutputTarget outputTarget, AzureFunctionSubscriptionModel model)
        {
            return new AzureFunctionConsumerTemplate(outputTarget, model);
        }

        [IntentManaged(Mode.Merge, Body = Mode.Ignore, Signature = Mode.Fully)]
        public override IEnumerable<AzureFunctionSubscriptionModel> GetModels(IApplication application)
        {
            var handlerModels = _metadataManager.Services(application).GetIntegrationEventHandlerModels();
            var results = new List<AzureFunctionSubscriptionModel>();
            foreach (var handlerModel in handlerModels)
            {
                results.AddRange(handlerModel.IntegrationEventSubscriptions()
                    .Select(sub => sub.Element?.AsMessageModel())
                    .Where(p => p is not null)
                    .Cast<MessageModel>()
                    .Select(message => new AzureFunctionSubscriptionModel(HandlerModel: handlerModel, TopicName: message.GetTopicConfigurationName())));
            }

            results = results.DistinctBy(k => k.TopicName).ToList();

            return results;
        }
    }
}