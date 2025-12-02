using Intent.Aws.Sqs.Api;

namespace Intent.Modules.Aws.Sqs.Templates;

internal static class Constants
{
    public static readonly string[] BrokerStereotypeIds = [
        MessageModelStereotypeExtensions.AWSSQS.DefinitionId,
    ];
}
