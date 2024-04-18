using System.Linq;
using Intent.Eventing.GoogleCloud.PubSub.Api;
using Intent.Modelers.Eventing.Api;
using Intent.Modules.Common.Templates;

namespace Intent.Modules.Eventing.GoogleCloud.PubSub.Templates;

public static class Helper
{
    public static string GetSubscriptionId(string applicationName, EventingPackageModel packageModel)
    {
        return $"{applicationName} {packageModel.GetGoogleCloudSettings().TopicId()}".ToKebabCase();
    }

    public static bool HasMessages(EventingPackageModel packageModel)
    {
        return packageModel.Messages.Any();
    }
}