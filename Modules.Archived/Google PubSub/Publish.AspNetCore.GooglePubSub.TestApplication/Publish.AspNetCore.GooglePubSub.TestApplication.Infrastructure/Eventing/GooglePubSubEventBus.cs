using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Google.Cloud.PubSub.V1;
using Google.Protobuf;
using Intent.RoslynWeaver.Attributes;
using Microsoft.Extensions.Options;
using Publish.AspNetCore.GooglePubSub.TestApplication.Application.Common.Eventing;
using Publish.AspNetCore.GooglePubSub.TestApplication.Application.IntegrationEvents;
using Publish.AspNetCore.GooglePubSub.TestApplication.Infrastructure.Configuration;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Eventing.GoogleCloud.PubSub.EventingTemplates.GooglePubSubEventBus", Version = "1.0")]

namespace Publish.AspNetCore.GooglePubSub.TestApplication.Infrastructure.Eventing;

public class GooglePubSubEventBus : IEventBus
{
    private readonly IEventBusTopicEventManager _topicEventManager;

    private readonly List<PubsubMessage> _messagesToPublish = new();
    private readonly PubSubOptions _pubSubOptions;

    public GooglePubSubEventBus(IEventBusTopicEventManager topicEventManager, IOptions<PubSubOptions> pubSubOptions)
    {
        _topicEventManager = topicEventManager;
        _pubSubOptions = pubSubOptions.Value;
    }

    public void Publish<T>(T message) where T : class
    {
        if (typeof(T) == typeof(GenericMessage))
        {
            throw new ArgumentException($"{nameof(GenericMessage)} is not meant to be published. Create a new Message type intended for your given use case.");
        }
        _messagesToPublish.Add(new PubsubMessage
        {
            Attributes = { { "MessageType", typeof(T).FullName } },
            Data = ByteString.CopyFromUtf8(JsonSerializer.Serialize(message))
        });
    }

    public async Task FlushAllAsync(CancellationToken cancellationToken = default)
    {
        await Task.WhenAll(_messagesToPublish.Select(async message =>
        {
            var topicName = _topicEventManager.GetTopicName(message);
            var publisher = await new PublisherClientBuilder()
            {
                TopicName = topicName,
                EmulatorDetection = _pubSubOptions.GetEmulatorDetectionMode()
            }.BuildAsync(cancellationToken);
            await publisher.PublishAsync(message);
        }));
        _messagesToPublish.Clear();
    }
}