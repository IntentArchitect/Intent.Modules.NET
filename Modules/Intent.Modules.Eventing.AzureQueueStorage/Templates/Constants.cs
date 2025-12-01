using Intent.Eventing.AzureQueueStorage.Api;

namespace Intent.Modules.Eventing.AzureQueueStorage.Templates;

internal static class Constants
{
    public static readonly string[] BrokerStereotypeIds = [
        MessageModelStereotypeExtensions.AzureQueueStorage.DefinitionId,
        EventingPackageModelStereotypeExtensions.AzureQueueStoragePackageSettings.DefinitionId,
        FolderModelStereotypeExtensions.AzureQueueStorageFolderSettings.DefinitionId
    ];
}
