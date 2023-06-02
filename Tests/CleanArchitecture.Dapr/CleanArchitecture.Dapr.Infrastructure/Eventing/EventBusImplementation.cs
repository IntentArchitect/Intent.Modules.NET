using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using CleanArchitecture.Dapr.Application.Common.Eventing;
using Dapr.Client;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Dapr.AspNetCore.Pubsub.EventBusImplementation", Version = "1.0")]

namespace CleanArchitecture.Dapr.Infrastructure.Eventing
{
    public class EventBusImplementation : IEventBus
    {
        private readonly ConcurrentQueue<IEvent> _events;
        private readonly DaprClient _dapr;

        public EventBusImplementation(DaprClient dapr)
        {
            _dapr = dapr;
            _events = new ConcurrentQueue<IEvent>();
        }

        public void Publish<T>(T @event)
            where T : class, IEvent
        {
            _events.Enqueue(@event);
        }

        public async Task FlushAllAsync(CancellationToken cancellationToken = default)
        {
            while (_events.TryDequeue(out var @event))
            {
                // We need to make sure that we pass the concrete type to PublishEventAsync,
                // which can be accomplished by casting the event to dynamic. This ensures
                // that all event fields are properly serialized.
                await _dapr.PublishEventAsync(@event.PubsubName, @event.TopicName, (object)@event, cancellationToken);
            }
        }
    }
}