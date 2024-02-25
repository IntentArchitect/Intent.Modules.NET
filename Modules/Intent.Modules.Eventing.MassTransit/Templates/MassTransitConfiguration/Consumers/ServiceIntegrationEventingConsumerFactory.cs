using System.Collections.Generic;
using System.Linq;
using Intent.Eventing.MassTransit.Api;
using Intent.Modelers.Eventing.Api;
using Intent.Modelers.Services.Api;
using Intent.Modelers.Services.EventInteractions;
using Intent.Modules.Eventing.Contracts.Templates;

namespace Intent.Modules.Eventing.MassTransit.Templates.MassTransitConfiguration.Consumers;

internal class ServiceIntegrationEventingConsumerFactory : IConsumerFactory
{
    private readonly MassTransitConfigurationTemplate _template;

    public ServiceIntegrationEventingConsumerFactory(MassTransitConfigurationTemplate template)
    {
        _template = template;
    }

    public IReadOnlyCollection<Consumer> CreateConsumers()
    {
        var consumers = _template.ExecutionContext.MetadataManager
            .Services(_template.ExecutionContext.GetApplicationConfig().Id).GetIntegrationEventHandlerModels()
            .SelectMany(x => x.IntegrationEventSubscriptions())
            .Select(subscription =>
            {
                var messageModel = subscription.TypeReference.Element.AsMessageModel();
                var messageName = _template.GetIntegrationEventMessageName(messageModel);

                var consumerDefinitionType =
                    $@"{_template.GetIntegrationEventHandlerInterfaceName()}<{messageName}>, {messageName}";

                return new Consumer(
                    MessageTypeFullName: _template.GetFullyQualifiedTypeName(messageModel.InternalElement),
                    ConsumerTypeName: $@"{_template.GetIntegrationEventConsumerName()}<{consumerDefinitionType}>",
                    ConsumerDefinitionTypeName: $"{_template.GetIntegrationEventConsumerName()}Definition<{consumerDefinitionType}>",
                    ConfigureConsumeTopology: true,
                    DestinationAddress: null,
                    AzureConsumerSettings: subscription.GetAzureServiceBusConsumerSettings(),
                    RabbitMqConsumerSettings: subscription.GetRabbitMQConsumerSettings());
            })
            .ToList();
        return consumers;
    }
}