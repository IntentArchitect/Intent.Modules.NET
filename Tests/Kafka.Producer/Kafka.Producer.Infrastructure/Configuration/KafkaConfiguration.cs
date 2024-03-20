using System;
using Confluent.SchemaRegistry;
using Intent.RoslynWeaver.Attributes;
using Kafka.Producer.Application.Common.Eventing;
using Kafka.Producer.Eventing.Messages;
using Kafka.Producer.Infrastructure.Eventing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Eventing.Kafka.KafkaConfiguration", Version = "1.0")]

namespace Kafka.Producer.Infrastructure.Configuration
{
    public static class KafkaConfiguration
    {
        public static void AddKafkaConfiguration(this IServiceCollection services)
        {
            services.AddSingleton<ISchemaRegistryClient>(serviceProvider =>
            {
                var schemaRegistryConfig = serviceProvider
                    .GetRequiredService<IConfiguration>()
                    .GetSection("Kafka:SchemaRegistryConfig")
                    .Get<SchemaRegistryConfig>();

                return new CachedSchemaRegistryClient(schemaRegistryConfig);
            });
            services.AddScoped<IEventBus, KafkaEventBus>();
            services.AddScoped(typeof(IKafkaEventDispatcher<>), typeof(KafkaEventDispatcher<>));
            services.AddHostedService<KafkaConsumerBackgroundService>();
            services.AddSingleton(serviceProvider => CreateProducer<string, InvoiceCreatedEvent>(serviceProvider, message => message.Id.ToString()));
            services.AddSingleton(serviceProvider => CreateProducer<string, InvoiceUpdatedEvent>(serviceProvider, message => message.Id.ToString()));
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