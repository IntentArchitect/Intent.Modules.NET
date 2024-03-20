using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using Confluent.Kafka;
using Confluent.Kafka.SyncOverAsync;
using Confluent.SchemaRegistry;
using Confluent.SchemaRegistry.Serdes;
using Intent.RoslynWeaver.Attributes;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Eventing.Kafka.KafkaProducer", Version = "1.0")]

namespace Kafka.Producer.Infrastructure.Eventing
{
    public sealed class KafkaProducer<TKey, TValue> : IKafkaProducer<TValue>, IDisposable
        where TValue : class
    {
        private readonly IProducer<TKey, TValue> _producer;
        private readonly string _topic;
        private readonly Func<TValue, TKey>? _keyProvider;

        public KafkaProducer(ISchemaRegistryClient schemaRegistryClient,
            Func<TValue, TKey>? keyProvider,
            IConfiguration configuration,
            ILogger<KafkaProducer<TKey, TValue>> logger)
        {
            _keyProvider = keyProvider;
            var messageType = $"{typeof(TValue).Namespace}.{typeof(TValue).Name}";
            var producerConfig = configuration
                .GetSection($"Kafka:MessageTypes:{messageType}:ProducerConfig")
                .Get<ProducerConfig>();

            logger.LogInformation(producerConfig != null
                ? "Using message type specific configuration"
                : "Using default configuration");

            producerConfig ??= configuration
                .GetSection("Kafka:DefaultProducerConfig")
                .Get<ProducerConfig>();

            _topic = configuration[$"Kafka:MessageTypes:{messageType}:Topic"] ?? typeof(TValue).Name;

            _producer = new ProducerBuilder<TKey, TValue>(producerConfig)
                .SetValueSerializer(new JsonSerializer<TValue>(schemaRegistryClient).AsSyncOverAsync())
                .Build();
        }

        public async Task Produce(ConcurrentQueue<TValue> messageQueue, CancellationToken cancellationToken = default)
        {
            while (!cancellationToken.IsCancellationRequested && messageQueue.TryDequeue(out var message))
            {
                await _producer.ProduceAsync(
                    topic: _topic,
                    message: new Message<TKey, TValue>
                    {
                        Key = _keyProvider != null ? _keyProvider(message) : default!,
                        Value = message
                    },
                    cancellationToken: cancellationToken);
            }
            _producer.Flush(cancellationToken);
        }

        public void Dispose()
        {
            _producer.Dispose();
        }
    }
}