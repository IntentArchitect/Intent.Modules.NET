using System;
using Confluent.SchemaRegistry;
using Intent.RoslynWeaver.Attributes;
using Kafka.Consumer.Application.Common.Eventing;
using Kafka.Consumer.Infrastructure.Eventing;
using Kafka.Producer.Eventing.Messages;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Eventing.Kafka.KafkaConfiguration", Version = "1.0")]

namespace Kafka.Consumer.Infrastructure.Configuration
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
            services.AddTransient<IKafkaConsumer, KafkaConsumer<InvoiceCreatedEvent>>();
            services.AddTransient<IKafkaConsumer, KafkaConsumer<InvoiceUpdatedEvent>>();
        }
    }
}