using Intent.Engine;
using System.Collections.Generic;
using System.Linq;
using Intent.Modelers.Eventing.Api;
using Intent.Modules.Common.Templates;

namespace Intent.Modules.Dapr.AspNetCore.Pubsub
{
    internal static class ExtensionMethods
    {
        public static string GetMessageFolderName(this MessageModel model) => model.Name.RemoveSuffix("Event", "Message");

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
