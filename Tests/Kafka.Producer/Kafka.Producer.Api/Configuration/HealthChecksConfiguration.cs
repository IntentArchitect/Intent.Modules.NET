using System;
using System.Net.Http;
using Confluent.Kafka;
using HealthChecks.Kafka;
using HealthChecks.UI.Client;
using Intent.RoslynWeaver.Attributes;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.AspNetCore.HealthChecks.HealthChecksConfiguration", Version = "1.0")]

namespace Kafka.Producer.Api.Configuration
{
    public static class HealthChecksConfiguration
    {
        public static IServiceCollection ConfigureHealthChecks(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            var hcBuilder = services.AddHealthChecks();
            hcBuilder.AddKafka(new ProducerConfig
            {
                BootstrapServers = configuration["Kafka:DefaultProducerConfig:BootstrapServers"]!
            }, name: "DefaultProducerConfig", tags: new[] { "integration", "Kafka" });
            hcBuilder.AddKafka(new ProducerConfig
            {
                BootstrapServers = configuration["Kafka:DefaultConsumerConfig:BootstrapServers"]!
            }, name: "DefaultConsumerConfig", tags: new[] { "integration", "Kafka" });
            hcBuilder.AddUrlGroup(options => options
                .AddUri(configuration.GetValue<Uri>("Kafka:SchemaRegistryConfig:Url")!)
                .UseHttpMethod(HttpMethod.Get), name: "SchemaRegistryConfig");

            return services;
        }

        public static IEndpointRouteBuilder MapDefaultHealthChecks(this IEndpointRouteBuilder endpoints)
        {
            endpoints.MapHealthChecks("/hc", new HealthCheckOptions
            {
                Predicate = _ => true,
                ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
            });
            return endpoints;
        }
    }
}