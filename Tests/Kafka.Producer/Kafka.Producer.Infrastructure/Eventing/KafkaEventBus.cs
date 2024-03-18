using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using Intent.RoslynWeaver.Attributes;
using Kafka.Producer.Application.Common.Eventing;
using Microsoft.Extensions.DependencyInjection;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Eventing.Kafka.KafkaEventBus", Version = "1.0")]

namespace Kafka.Producer.Infrastructure.Eventing
{
    public class KafkaEventBus : IEventBus
    {
        private readonly ConcurrentDictionary<Type, IProducer> _producersByMessageType = new ConcurrentDictionary<Type, IProducer>();
        private readonly IServiceProvider _serviceProvider;

        public KafkaEventBus(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public void Publish<T>(T message)
            where T : class
        {
            var producer = _producersByMessageType.GetOrAdd(typeof(T), _ => new Producer<T>(_serviceProvider.GetRequiredService<IKafkaProducer<T>>()));
            producer.EnqueueMessage(message);
        }

        public async Task FlushAllAsync(CancellationToken cancellationToken = default)
        {
            var producers = _producersByMessageType.Values;

            foreach (var producer in producers)
            {
                await producer.FlushAsync(cancellationToken);
            }
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