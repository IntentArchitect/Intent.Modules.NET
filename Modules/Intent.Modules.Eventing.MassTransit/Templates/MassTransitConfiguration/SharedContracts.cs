using Intent.Eventing.MassTransit.Api;
using Intent.Modelers.Eventing.Api;

namespace Intent.Modules.Eventing.MassTransit.Templates.MassTransitConfiguration;

internal record Subscription(
    MessageModel Message,
    IAzureServiceBusConsumerSettings? AzureConsumerSettings,
    IRabbitMQConsumerSettings? RabbitMqConsumerSettings);