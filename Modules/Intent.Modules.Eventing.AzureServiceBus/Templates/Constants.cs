using Intent.Eventing.AzureServiceBus.Api;

namespace Intent.Modules.Eventing.AzureServiceBus.Templates;

internal static class Constants
{
    public static readonly string[] BrokerStereotypeIds = [
        MessageModelStereotypeExtensions.AzureServiceBus.DefinitionId,
        EventingPackageModelStereotypeExtensions.AzureServiceBusPackageSettings.DefinitionId,
        FolderModelStereotypeExtensions.AzureServiceBusFolderSettings.DefinitionId
    ];
}
