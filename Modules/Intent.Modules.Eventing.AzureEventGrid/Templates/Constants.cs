using Intent.Eventing.AzureEventGrid.Api;

namespace Intent.Modules.Eventing.AzureEventGrid.Templates;

internal static class Constants
{
    public static readonly string[] BrokerStereotypeIds = [
        MessageModelStereotypeExtensions.AzureEventGrid.DefinitionId,
        EventingPackageModelStereotypeExtensions.EventDomain.DefinitionId,
        FolderModelStereotypeExtensions.AzureEventGridFolderSettings.DefinitionId
    ];
}