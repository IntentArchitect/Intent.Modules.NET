using System;
using System.Threading;
using System.Threading.Tasks;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Eventing.GoogleCloud.PubSub.InterfaceTemplates.CloudResourceManagerInterface", Version = "1.0")]

namespace Publish.AspNetCore.GooglePubSub.TestApplication.Infrastructure.Eventing;

public interface ICloudResourceManager
{
    string ProjectId { get; }
    bool ShouldSetupCloudResources { get; }
    Task CreateTopicIfNotExistAsync(string topicId, CancellationToken cancellationToken = default);
    Task CreateSubscriptionIfNotExistAsync((string SubscriptionId, string TopicId) subscription, CancellationToken cancellationToken = default);
}