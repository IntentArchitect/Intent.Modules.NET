using Intent.Eventing.AzureEventGrid.Api;
using Intent.Modelers.Eventing.Api;

namespace Intent.Modules.Eventing.AzureEventGrid.Templates;

public static class EventGridHelper
{
    public static string GetTopicName(this MessageModel model)
    {
        return model.GetAzureEventGrid()?.TopicName() ?? "NO TOPIC";
    }
}