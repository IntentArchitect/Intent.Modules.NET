using Intent.Dapr.AspNetCore.Pubsub.Api;

namespace Intent.Modules.Dapr.AspNetCore.Pubsub.Templates;

internal static class Constants
{
    public static readonly string[] BrokerStereotypeIds = [
        MessageModelStereotypeExtensions.DaprSettings.DefinitionId
    ];
}
