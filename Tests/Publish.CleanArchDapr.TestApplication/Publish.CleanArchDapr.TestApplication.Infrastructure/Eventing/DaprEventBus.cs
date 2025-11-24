using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Dapr.Client;
using Intent.RoslynWeaver.Attributes;
using Publish.CleanArchDapr.TestApplication.Application.Common.Eventing;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Dapr.AspNetCore.Pubsub.DaprEventBus", Version = "1.0")]

namespace Publish.CleanArchDapr.TestApplication.Infrastructure.Eventing
{
    public class DaprEventBus : IEventBus
    {
        private readonly ConcurrentQueue<MessageEntry> _messages;
        private readonly DaprClient _dapr;

        public DaprEventBus(DaprClient dapr)
        {
            _dapr = dapr;
            _messages = new ConcurrentQueue<MessageEntry>();
        }

        public void Publish<TMessage>(TMessage message)
            where TMessage : class
        {
            _messages.Enqueue(new MessageEntry((IEvent)message, null));
        }

        public void Publish<TMessage>(TMessage message, IDictionary<string, object> additionalData)
            where TMessage : class
        {
            _messages.Enqueue(new MessageEntry((IEvent)message, additionalData.ToDictionary(key => key.Key, value => value.ToString())));
        }

        public void Send<TMessage>(TMessage message)
            where TMessage : class
        {
            _messages.Enqueue(new MessageEntry((IEvent)message, null));
        }

        public void Send<TMessage>(TMessage message, IDictionary<string, object> additionalData)
            where TMessage : class
        {
            _messages.Enqueue(new MessageEntry((IEvent)message, additionalData.ToDictionary(key => key.Key, value => value.ToString())));
        }

        public async Task FlushAllAsync(CancellationToken cancellationToken = default)
        {
            while (_messages.TryDequeue(out var message))
            {
                // We need to make sure that we pass the concrete type to PublishEventAsync,
                // which can be accomplished by casting the event to dynamic. This ensures
                // that all event fields are properly serialized.

                if (message.AdditionalData != null)
                {
                    await _dapr.PublishEventAsync(message.Message.PubsubName, message.Message.TopicName, (object)message.Message, message.AdditionalData, cancellationToken);
                }
                else
                {
                    await _dapr.PublishEventAsync(message.Message.PubsubName, message.Message.TopicName, (object)message.Message, cancellationToken);
                }
            }
        }

        private record MessageEntry(IEvent Message, Dictionary<string, string>? AdditionalData);

    }
}