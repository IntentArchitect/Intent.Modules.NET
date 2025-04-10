using Intent.Metadata.Models;
using Intent.Modelers.Services.EventInteractions;

namespace Intent.Modules.AzureFunctions.AzureEventGrid.Templates.AzureFunctionConsumer;

public record AzureFunctionSubscriptionModel(
    IntegrationEventHandlerModel HandlerModel) : IMetadataModel
{
    public string Id => HandlerModel.Id;
}