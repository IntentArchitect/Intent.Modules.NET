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
using Intent.Modules.Integration.IaC.Shared;
using Intent.Modules.Integration.IaC.Shared.AzureServiceBus;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.TemplateRegistration.FilePerModel", Version = "1.0")]

namespace Intent.Modules.AzureFunctions.AzureServiceBus.Templates.AzureFunctionConsumer
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
            var results = new List<AzureFunctionSubscriptionModel>();
            var handlerModels = _metadataManager.Services(application).GetIntegrationEventHandlerModels();
            
            var messagesLookup = IntegrationManager.Instance.GetSubscribedAzureServiceBusMessages(application.Id)
                .ToDictionary(k => k.MessageModel);

            results.AddRange(handlerModels
                .SelectMany(x => x.IntegrationEventSubscriptions(), (handlerModel, subscription) => new
                {
                    Handler = handlerModel,
                    Message = subscription.Element.IsMessageModel() 
                        ? messagesLookup.GetValueOrDefault(subscription.Element.AsMessageModel()) 
                        : null
                })
                .Where(p => p.Message is not null)
                .Select(s => new AzureFunctionSubscriptionModel(
                    s.Handler,
                    s.Message!.QueueOrTopicName,
                    s.Message.QueueOrTopicConfigurationName,
                    s.Message.ChannelType == AzureServiceBusChannelType.Topic,
                    s.Message.QueueOrTopicSubscriptionConfigurationName))
            );

            var commandsLookup = IntegrationManager.Instance.GetSubscribedAzureServiceBusCommands(application.Id)
                .ToDictionary(k => k.CommandModel);
            results.AddRange(handlerModels
                .SelectMany(x => x.IntegrationEventSubscriptions(), (handlerModel, subscription) => new
                {
                    Handler = handlerModel,
                    Message = subscription.Element.IsIntegrationCommandModel()
                        ? commandsLookup.GetValueOrDefault(subscription.Element.AsIntegrationCommandModel())
                        : null
                })
                .Where(p => p.Message is not null)
                .Select(s => new AzureFunctionSubscriptionModel(
                    s.Handler,
                    s.Message!.QueueOrTopicName,
                    s.Message.QueueOrTopicConfigurationName,
                    s.Message.ChannelType == AzureServiceBusChannelType.Topic,
                    s.Message.QueueOrTopicSubscriptionConfigurationName))
            );
            
            results = results.DistinctBy(k => k.QueueOrTopicName).ToList();

            return results;
        }
    }
}