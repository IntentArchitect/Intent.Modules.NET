using Intent.Aws.Sqs.Api;

namespace Intent.Modules.Aws.Sqs.Templates;

internal static class Constants
{
    public static readonly string[] BrokerStereotypeIds = [
        MessageModelStereotypeExtensions.AwsSqs.DefinitionId,
        EventingPackageModelStereotypeExtensions.AwsSqsPackageSettings.DefinitionId,
        FolderModelStereotypeExtensions.AwsSqsFolderSettings.DefinitionId
    ];
}
