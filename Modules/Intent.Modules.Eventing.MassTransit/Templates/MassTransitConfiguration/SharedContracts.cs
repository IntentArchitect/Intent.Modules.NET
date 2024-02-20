using Intent.Eventing.MassTransit.Api;
using Intent.Modelers.Eventing.Api;

namespace Intent.Modules.Eventing.MassTransit.Templates.MassTransitConfiguration;
  
internal record Consumer(
    string MessageTypeFullName,
    string ConsumerTypeName,
    string ConsumerDefinitionTypeName,
    bool ConfigureConsumeTopology,
    string? DestinationAddress,
    IAzureServiceBusConsumerSettings? AzureConsumerSettings,
    IRabbitMQConsumerSettings? RabbitMqConsumerSettings);