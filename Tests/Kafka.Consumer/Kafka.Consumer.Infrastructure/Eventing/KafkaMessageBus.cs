using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Intent.RoslynWeaver.Attributes;
using Kafka.Consumer.Application.Common.Eventing;
using Microsoft.Extensions.DependencyInjection;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Eventing.Kafka.KafkaMessageBus", Version = "1.0")]

namespace Kafka.Consumer.Infrastructure.Eventing
{
    public class KafkaMessageBus : IEventBus
    {
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
            throw new NotSupportedException("Additional data is not supported in Kafka event publishing.");
        }

        public void Send<TMessage>(TMessage message)
            where TMessage : class
        {
            Publish(message);
        }

        public void Send<TMessage>(TMessage message, IDictionary<string, object> additionalData)
            where TMessage : class
        {
            throw new NotSupportedException("Additional data is not supported in Kafka event publishing.");
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