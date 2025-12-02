using Intent.Eventing.AzureEventGrid.Api;

namespace Intent.Modules.AzureFunctions.AzureEventGrid.Templates;

internal static class Constants
{
    public static readonly string[] BrokerStereotypeIds = [
        MessageModelStereotypeExtensions.AzureEventGrid.DefinitionId,
        EventingPackageModelStereotypeExtensions.EventDomain.DefinitionId,
    ];
}

