using CompositeMessageBus.Eventing.Messages;
using CompositeMessageBus.Infrastructure.Eventing;
using Confluent.Kafka;
using Confluent.SchemaRegistry;
using Intent.RoslynWeaver.Attributes;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Eventing.Kafka.KafkaConfiguration", Version = "1.0")]

namespace CompositeMessageBus.Infrastructure.Configuration
{
    public static class KafkaConfiguration
    {
        public static IServiceCollection AddKafkaConfiguration(
            this IServiceCollection services,
            IConfiguration configuration,
            MessageBrokerRegistry registry)
        {
            services.AddSingleton<ISchemaRegistryClient>(serviceProvider =>
            {
                var schemaRegistryConfig = serviceProvider
                    .GetRequiredService<IConfiguration>()
                    .GetSection("Kafka:SchemaRegistryConfig")
                    .Get<SchemaRegistryConfig>();

                return new CachedSchemaRegistryClient(schemaRegistryConfig);
            });
            services.AddScoped<KafkaMessageBus>();
            services.AddScoped(typeof(IKafkaEventDispatcher<>), typeof(KafkaEventDispatcher<>));
            services.AddHostedService<KafkaConsumerBackgroundService>();
            services.AddSingleton(serviceProvider => CreateProducer<Null, MsgKafkaEvent>(serviceProvider));

            // Register message types with the composite message bus registry
            registry.Register<MsgKafkaEvent, KafkaMessageBus>();
            return services;
        }

        private static IKafkaProducer<TValue> CreateProducer<TKey, TValue>(
            IServiceProvider serviceProvider,
            Func<TValue, TKey>? keyProvider = null)
            where TValue : class
        {
            return new KafkaProducer<TKey, TValue>(
                schemaRegistryClient: serviceProvider.GetRequiredService<ISchemaRegistryClient>(),
                keyProvider: keyProvider,
                configuration: serviceProvider.GetRequiredService<IConfiguration>(),
                logger: serviceProvider.GetRequiredService<ILogger<KafkaProducer<TKey, TValue>>>());
        }
    }
}