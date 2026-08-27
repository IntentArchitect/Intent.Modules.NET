using System.Collections.Generic;
using Intent.Eventing.Wolverine.Api;

namespace Intent.Modules.Eventing.Wolverine.Templates;

internal static class Constants
{
    // The single stereotype this module defines - `Wolverine Message` - is both the broker
    // designation marker and the carrier of the `Topic Name` / `Destination Queue Name`
    // overrides, so there is exactly one DefinitionId to list here.
    public static readonly string[] BrokerStereotypeIds =
    [
        MessageModelStereotypeExtensions.WolverineMessage.DefinitionId
    ];
}
