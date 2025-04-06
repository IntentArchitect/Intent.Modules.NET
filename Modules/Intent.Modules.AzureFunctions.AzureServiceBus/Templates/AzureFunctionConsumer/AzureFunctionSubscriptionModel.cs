using Intent.Metadata.Models;
using Intent.Modelers.Services.EventInteractions;

namespace Intent.Modules.AzureFunctions.AzureServiceBus.Templates.AzureFunctionConsumer;

public record AzureFunctionSubscriptionModel(
    IntegrationEventHandlerModel HandlerModel, 
    string QueueOrTopicName, 
    string QueueOrTopicConfigurationName, 
    bool NeedsSubscription,
    string? SubscriptionName) : IMetadataModel
{
    public string Id => QueueOrTopicName;
}