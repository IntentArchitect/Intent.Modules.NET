using Intent.Metadata.Models;
using Intent.Modelers.Services.EventInteractions;

namespace Intent.Modules.AzureFunctions.AzureEventGrid.Templates.AzureFunctionConsumer;

public record AzureFunctionSubscriptionModel(
    IntegrationEventHandlerModel HandlerModel,
    string TopicName) : IMetadataModel
{
    public string Id => TopicName;
}