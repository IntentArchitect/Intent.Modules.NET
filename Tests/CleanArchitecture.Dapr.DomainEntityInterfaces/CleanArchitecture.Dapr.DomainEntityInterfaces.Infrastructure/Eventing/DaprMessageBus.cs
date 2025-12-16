using System.Collections.Concurrent;
using CleanArchitecture.Dapr.DomainEntityInterfaces.Application.Common.Eventing;
using Dapr.Client;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Dapr.AspNetCore.Pubsub.DaprMessageBus", Version = "1.0")]

namespace CleanArchitecture.Dapr.DomainEntityInterfaces.Infrastructure.Eventing
{
    public class DaprMessageBus : IEventBus
    {
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

        public void Send<TMessage>(TMessage message)
            where TMessage : class
        {
            _messages.Enqueue(new MessageEntry((IEvent)message, null));
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