using System.Collections.Generic;
using System.Linq;
using Intent.Eventing.MassTransit.Api;
using Intent.Modelers.Eventing.Api;
using Intent.Modelers.Services.Api;
using Intent.Modelers.Services.EventInteractions;
using Intent.Modules.Eventing.Contracts.Templates;

namespace Intent.Modules.Eventing.MassTransit.Templates.MassTransitConfiguration.Consumers;

internal class ServiceIntegrationCommandConsumerFactory : IConsumerFactory
{
    private readonly MassTransitConfigurationTemplate _template;

    public ServiceIntegrationCommandConsumerFactory(MassTransitConfigurationTemplate template)
    {
        _template = template;
    }
    
    public IReadOnlyCollection<Consumer> CreateConsumers()
    {
        var consumers = _template.ExecutionContext.MetadataManager
            .Services(_template.ExecutionContext.GetApplicationConfig().Id).GetIntegrationEventHandlerModels()
            .SelectMany(x => x.IntegrationCommandSubscriptions())
            .Select(subscription =>
            {
                var commandModel = subscription.TypeReference.Element.AsIntegrationCommandModel();
                var messageName = _template.GetIntegrationCommandName(commandModel);

                var consumerDefinitionType =
                    $@"{_template.GetIntegrationEventHandlerInterfaceName()}<{messageName}>, {messageName}";

                return new Consumer
                {
                    Message = new MessageDetail
                    {
                        MessageName = commandModel.Name,
                        MessageTypeFullName = _template.GetFullyQualifiedTypeName(commandModel.InternalElement),
                        TopicNameOverride = null
                    },
                    ConsumerTypeName = $@"{_template.GetIntegrationEventConsumerName()}<{consumerDefinitionType}>",
                    ConsumerDefinitionTypeName = $"{_template.GetIntegrationEventConsumerName()}Definition<{consumerDefinitionType}>",
                    IsSpecificMessageConsumer = true,
                    DestinationAddress = subscription.GetCommandConsumption()?.QueueName()
                };
            })
            .ToList();
        return consumers;
    }
}