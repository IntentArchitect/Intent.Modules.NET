using Intent.Eventing.AzureEventGrid.Api;
using Intent.Modelers.Eventing.Api;
using Intent.Modules.Common.Templates;

namespace Intent.Modules.Eventing.AzureEventGrid.Templates;

public static class EventGridHelper
{
    public static string GetTopicName(this MessageModel model)
    {
        return model.GetAzureEventGrid()?.TopicName() ?? "NO TOPIC";
    }

    public static string GetTopicConfigurationName(this MessageModel model)
    {
        var topic = model.GetTopicName();
        return topic.ToPascalCase();
    }
}