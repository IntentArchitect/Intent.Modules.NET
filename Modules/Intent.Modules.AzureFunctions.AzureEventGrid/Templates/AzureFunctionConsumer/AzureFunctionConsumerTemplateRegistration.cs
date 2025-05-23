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
using Intent.Modules.Integration.IaC.Shared;
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
            var eventGridMessages = IntegrationManager.Instance.GetSubscribedAzureEventGridMessages(application.Id)
                .ToDictionary(k => k.MessageModel);
            
            var results = handlerModels
                .SelectMany(x => x.IntegrationEventSubscriptions(), (handlerModel, subscription) => new
                {
                    Handler = handlerModel,
                    Message = eventGridMessages[subscription.Element.AsMessageModel()]
                })
                .GroupBy(x => x.Message.TopicConfigurationEndpointName)
                .Select(grouping => new AzureFunctionSubscriptionModel(
                    HandlerModel: grouping.First().Handler,
                    MessageModels: grouping.Select(x => x.Message.MessageModel).ToArray(),
                    TopicName: grouping.Key))
                .OrderBy(x => x.TopicName)
                .ToArray();

            return results;
        }
    }
}