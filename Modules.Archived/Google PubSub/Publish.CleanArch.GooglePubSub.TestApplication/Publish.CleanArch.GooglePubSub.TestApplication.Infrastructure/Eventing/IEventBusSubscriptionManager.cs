using System;
using System.Threading;
using System.Threading.Tasks;
using Google.Cloud.PubSub.V1;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Eventing.GoogleCloud.PubSub.InterfaceTemplates.EventBusSubscriptionManagerInterface", Version = "1.0")]

namespace Publish.CleanArch.GooglePubSub.TestApplication.Infrastructure.Eventing;

public interface IEventBusSubscriptionManager
{
    void RegisterEventHandler<TMessage>() where TMessage : class;
    Task DispatchAsync(IServiceProvider serviceProvider, PubsubMessage message, CancellationToken token);
}