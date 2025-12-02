using Intent.Eventing.AzureServiceBus.Api;

namespace Intent.Modules.AzureFunctions.AzureServiceBus.Templates;

internal static class Constants
{
    public static readonly string[] BrokerStereotypeIds = [
        MessageModelStereotypeExtensions.AzureServiceBus.DefinitionId,
    ];
}

