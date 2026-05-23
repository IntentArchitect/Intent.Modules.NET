using Intent.Eventing.NServiceBus.Api;

namespace Intent.Modules.Eventing.NServiceBus.Templates;

internal static class Constants
{
    public const string NServiceBusMessageBusId = "2c3a7ef0-099a-4b8c-8d7a-79526bd3cf6f";

    public static readonly string[] BrokerStereotypeIds =
    [
        MessageModelStereotypeExtensions.NServiceBus.DefinitionId
    ];
}
