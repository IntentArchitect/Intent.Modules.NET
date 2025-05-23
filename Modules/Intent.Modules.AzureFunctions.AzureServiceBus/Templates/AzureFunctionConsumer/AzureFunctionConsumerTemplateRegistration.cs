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

            var messagesLookup = handlerModels.Where(p => p.InternalElement.IsMessageModel())
                .ToDictionary(k => k.InternalElement.AsMessageModel());
            var messages = IntegrationManager.Instance.GetSubscribedAzureServiceBusMessages(application.Id);
            foreach (var message in messages)
            {
                var handlerModel = messagesLookup[message.MessageModel];
                results.Add(new AzureFunctionSubscriptionModel(
                    handlerModel,
                    message.QueueOrTopicName,
                    message.QueueOrTopicConfigurationName,
                    message.ChannelType == AzureServiceBusChannelType.Topic,
                    message.QueueOrTopicSubscriptionConfigurationName));
            }

            var commandsLookup = handlerModels.Where(p => p.InternalElement.IsCommentModel())
                .ToDictionary(k => k.InternalElement.AsIntegrationCommandModel());
            var commands = IntegrationManager.Instance.GetSubscribedAzureServiceBusCommands(application.Id);
            foreach (var command in commands)
            {
                var handlerModel = commandsLookup[command.CommandModel];
                results.Add(new AzureFunctionSubscriptionModel(
                    handlerModel,
                    command.QueueOrTopicName,
                    command.QueueOrTopicConfigurationName,
                    command.ChannelType == AzureServiceBusChannelType.Topic,
                    command.QueueOrTopicSubscriptionConfigurationName
                ));
            }

            results = results.DistinctBy(k => k.QueueOrTopicName).ToList();

            return results;
        }
    }
}