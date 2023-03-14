using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Modelers.Eventing.Api;

namespace Intent.Modules.Dapr.AspNetCore.Pubsub
{
    internal static class ExtensionMethods
    {
        public static IEnumerable<MessageModel> GetSubscribedToMessageModels(this IMetadataManager metadataManager, IApplication application)
        {
            return metadataManager.Eventing(application).GetApplicationModels()
                .SelectMany(
                    s => s.SubscribedMessages(),
                    (_, m) => m.Association.TargetEnd.Element.AsMessageModel());
        }

        public static IEnumerable<MessageModel> GetPublishedMessageModels(this IMetadataManager metadataManager, IApplication application)
        {
            return metadataManager.Eventing(application).GetApplicationModels()
                .SelectMany(
                    s => s.PublishedMessages(),
                    (_, m) => m.Association.TargetEnd.Element.AsMessageModel());
        }
    }
}
