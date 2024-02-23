using Intent.Eventing.MassTransit.Api;
using Intent.Modelers.Eventing.Api;

namespace Intent.Modules.Eventing.MassTransit.Templates.MassTransitConfiguration;
  
public record Consumer(
    string MessageTypeFullName,
    string ConsumerTypeName,
    string ConsumerDefinitionTypeName,
    bool ConfigureConsumeTopology,
    string? DestinationAddress,
    IAzureServiceBusConsumerSettings? AzureConsumerSettings,
    IRabbitMQConsumerSettings? RabbitMqConsumerSettings);
    
public record Producer(
    string MessageTypeName,
    string Urn);