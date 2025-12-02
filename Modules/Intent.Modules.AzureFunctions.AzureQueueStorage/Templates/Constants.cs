using Intent.Eventing.AzureQueueStorage.Api;

namespace Intent.Modules.AzureFunctions.AzureQueueStorage.Templates;

internal static class Constants
{
    public static readonly string[] BrokerStereotypeIds = [
        MessageModelStereotypeExtensions.AzureQueueStorage.DefinitionId,
    ];
}
