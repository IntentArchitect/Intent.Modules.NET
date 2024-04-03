using System;
using System.Threading;
using System.Threading.Tasks;
using Confluent.Kafka;
using Confluent.Kafka.SyncOverAsync;
using Confluent.SchemaRegistry.Serdes;
using Intent.RoslynWeaver.Attributes;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Eventing.Kafka.KafkaConsumer", Version = "1.0")]

namespace Kafka.Consumer.Infrastructure.Eventing
{
    public class KafkaConsumer<T> : IKafkaConsumer
        where T : class
    {
        private readonly ILogger<KafkaConsumer<T>> _logger;
        private readonly IServiceProvider _serviceProvider;
        private readonly IConfiguration _configuration;

        public KafkaConsumer(ILogger<KafkaConsumer<T>> logger,
            IServiceProvider serviceProvider,
            IConfiguration configuration)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
            _configuration = configuration;
        }

        public async Task DoWork(CancellationToken stoppingToken)
        {
            var messageType = $"{typeof(T).Namespace}.{typeof(T).Name}";

            var consumerConfig = _configuration
                .GetSection($"Kafka:MessageTypes:{messageType}:ConsumerConfig")
                .Get<ConsumerConfig>();

            _logger.LogInformation(consumerConfig != null
                ? "Using message type specific configuration"
                : "Using default configuration");

            consumerConfig ??= _configuration
                .GetSection("Kafka:DefaultConsumerConfig")
                .Get<ConsumerConfig>();

            var topic = _configuration[$"Kafka:MessageTypes:{messageType}:Topic"] ?? typeof(T).Name;

            _logger.LogInformation($"Topic: {topic}");

            try
            {
                using (var consumer = new ConsumerBuilder<string, T>(consumerConfig).SetValueDeserializer(new JsonDeserializer<T>().AsSyncOverAsync()).Build())
                {
                    consumer.Subscribe(topic);

                    try
                    {
                        while (!stoppingToken.IsCancellationRequested)
                        {
                            using (var scope = _serviceProvider.CreateScope())
                            {
                                var consumeResult = consumer.Consume(stoppingToken);
                                var dispatcher = scope.ServiceProvider.GetRequiredService<IKafkaEventDispatcher<T>>();

                                try
                                {
                                    await dispatcher.Dispatch(consumeResult.Message.Value, stoppingToken);
                                }
                                catch (Exception exception)
                                {
                                    _logger.LogError(exception, "Error processing incoming message");
                                }
                            }
                        }
                    }
                    catch (OperationCanceledException)
                    {
                        // NOP
                    }
                    finally
                    {
                        consumer.Close();
                    }
                }
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, $"Error creating consumer for {messageType}");
            }
        }
    }
}