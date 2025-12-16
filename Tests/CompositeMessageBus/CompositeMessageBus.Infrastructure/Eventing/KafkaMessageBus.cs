using System.Collections.Concurrent;
using CompositeMessageBus.Application.Common.Eventing;
using Intent.RoslynWeaver.Attributes;
using Microsoft.Extensions.DependencyInjection;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Eventing.Kafka.KafkaMessageBus", Version = "1.0")]

namespace CompositeMessageBus.Infrastructure.Eventing
{
    public class KafkaMessageBus : IEventBus
    {
        public const string AddressKey = "address";
        private readonly ConcurrentDictionary<Type, IProducer> _producersByMessageType = new ConcurrentDictionary<Type, IProducer>();
        private readonly IServiceProvider _serviceProvider;

        public KafkaMessageBus(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public void Publish<TMessage>(TMessage message)
            where TMessage : class
        {
            var producer = _producersByMessageType.GetOrAdd(typeof(TMessage), _ => new Producer<TMessage>(_serviceProvider.GetRequiredService<IKafkaProducer<TMessage>>()));
            producer.EnqueueMessage(message);
        }

        public void Publish<TMessage>(TMessage message, IDictionary<string, object> additionalData)
            where TMessage : class
        {
            throw new NotSupportedException("Publishing with additional data is not supported by this message bus provider.");
        }

        public void Send<TMessage>(TMessage message)
            where TMessage : class
        {
            Publish(message);
        }

        public void Send<TMessage>(TMessage message, IDictionary<string, object> additionalData)
            where TMessage : class
        {
            throw new NotSupportedException("Sending with additional data is not supported by this message bus provider.");
        }

        public void Send<TMessage>(TMessage message, Uri address)
            where TMessage : class
        {
            throw new NotSupportedException("Explicit address-based sending is not supported by this message bus provider.");
        }

        public async Task FlushAllAsync(CancellationToken cancellationToken = default)
        {
            var producers = _producersByMessageType.Values;

            foreach (var producer in producers)
            {
                await producer.FlushAsync(cancellationToken);
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

        private class Producer<T> : IProducer
            where T : class
        {
            private readonly ConcurrentQueue<T> _messageQueue = new ConcurrentQueue<T>();
            private readonly IKafkaProducer<T> _kafkaProducer;

            public Producer(IKafkaProducer<T> kafkaProducer)
            {
                _kafkaProducer = kafkaProducer;
            }

            public void EnqueueMessage(object message) => _messageQueue.Enqueue((T)message);

            public async Task FlushAsync(CancellationToken cancellationToken) => await _kafkaProducer.Produce(_messageQueue, cancellationToken);
        }
        private interface IProducer
        {
            void EnqueueMessage(object message);
            Task FlushAsync(CancellationToken cancellationToken);
        }
    }
}