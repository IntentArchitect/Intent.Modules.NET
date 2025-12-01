using Intent.Eventing.Kafka.Api;

namespace Intent.Modules.Eventing.Kafka.Templates;

internal static class Constants
{
    public static readonly string[] BrokerStereotypeIds = [
        MessageModelStereotypeExtensions.KafkaMessageSettings.DefinitionId,
        EventingPackageModelStereotypeExtensions.KafkaPackageSettings.DefinitionId,
        FolderModelStereotypeExtensions.KafkaFolderSettings.DefinitionId
    ];
}
