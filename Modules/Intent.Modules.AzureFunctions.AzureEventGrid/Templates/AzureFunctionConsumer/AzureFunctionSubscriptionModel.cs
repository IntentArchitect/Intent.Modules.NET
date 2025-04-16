using System.Collections.Generic;
using Intent.Metadata.Models;
using Intent.Modelers.Eventing.Api;
using Intent.Modelers.Services.EventInteractions;

namespace Intent.Modules.AzureFunctions.AzureEventGrid.Templates.AzureFunctionConsumer;

public record AzureFunctionSubscriptionModel(
    IntegrationEventHandlerModel HandlerModel,
    IReadOnlyCollection<MessageModel> MessageModels,
    string TopicName) : IMetadataModel
{
    public string Id => TopicName;
}