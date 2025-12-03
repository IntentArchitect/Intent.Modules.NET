using System.Collections.Concurrent;
using CompositeMessageBus.Application.Common.Eventing;
using Dapr.Client;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Dapr.AspNetCore.Pubsub.DaprMessageBus", Version = "1.0")]

namespace CompositeMessageBus.Infrastructure.Eventing
{
    public class DaprMessageBus : IEventBus
    {
        public const string AddressKey = "address";
        private readonly ConcurrentQueue<MessageEntry> _messages;
        private readonly DaprClient _dapr;

        public DaprMessageBus(DaprClient dapr)
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

        public void Send<TMessage>(TMessage message, Uri address)
            where TMessage : class
        {
            throw new NotSupportedException("Explicit address-based sending is not supported by this message bus provider.");
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

        public void SchedulePublish<TMessage>(TMessage message, DateTime scheduled)
            where TMessage : class
        {
            throw new NotSupportedException("Scheduled publishing is not supported by this message bus provider.");
        }

        public void SchedulePublish<TMessage>(TMessage message, TimeSpan delay)
            where TMessage : class
        {
            throw new NotSupportedException("Scheduled publishing is not supported by this message bus provider.");
        }

        private record MessageEntry(IEvent Message, Dictionary<string, string>? AdditionalData);

    }
}