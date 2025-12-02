using System.Collections.Generic;
using Intent.Eventing.MassTransit.Api;

namespace Intent.Modules.Eventing.MassTransit.Templates;

internal static class Constants
{
    public static readonly string[] BrokerStereotypeIds = 
    [
        MessageModelStereotypeExtensions.MessageTopologySettings.DefinitionId,
        MessageModelStereotypeExtensions.MassTransitMessage.DefinitionId
    ];
}