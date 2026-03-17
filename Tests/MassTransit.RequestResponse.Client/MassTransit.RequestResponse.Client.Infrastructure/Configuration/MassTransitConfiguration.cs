using System.Reflection;
using Intent.RoslynWeaver.Attributes;
using MassTransit;
using MassTransit.Configuration;
using MassTransit.RequestResponse.Client.Application.Common.Eventing;
using MassTransit.RequestResponse.Client.Infrastructure.Eventing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Eventing.MassTransit.MassTransitConfiguration", Version = "1.0")]

namespace MassTransit.RequestResponse.Client.Infrastructure.Configuration
{
    public static class MassTransitConfiguration
    {
        public static IServiceCollection AddMassTransitConfiguration(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            services.AddScoped<MassTransitMessageBus>();
            services.AddScoped<IMessageBus>(provider => provider.GetRequiredService<MassTransitMessageBus>());

            services.AddMassTransit(x =>
            {
                x.SetKebabCaseEndpointNameFormatter();
                x.AddConsumers();
                //IntentIgnore
                x.UsingRabbitMq((context, cfg) =>
                {
                    cfg.Host(configuration["RabbitMq:Host"], configuration["RabbitMq:VirtualHost"], host =>
                    {
                        host.Username(configuration["RabbitMq:Username"]);
                        host.Password(configuration["RabbitMq:Password"]);
                    });

                    cfg.UseMessageRetry(r => r.Interval(
                        configuration.GetValue<int?>("MassTransit:RetryInterval:RetryCount") ?? 10,
                        configuration.GetValue<TimeSpan?>("MassTransit:RetryInterval:Interval") ?? TimeSpan.FromSeconds(5)));

                    cfg.ConfigureEndpoints(context);
                    cfg.UseInMemoryOutbox(context);
                    EndpointConventionRegistration();
                });
                //IntentIgnore
                //x.UsingAzureServiceBus((context, cfg) =>
                //{
                //    cfg.Host(configuration["AzureMessageBus:ConnectionString"]);

                //    cfg.UseMessageRetry(r => r.Interval(
                //        configuration.GetValue<int?>("MassTransit:RetryInterval:RetryCount") ?? 10,
                //        configuration.GetValue<TimeSpan?>("MassTransit:RetryInterval:Interval") ?? TimeSpan.FromSeconds(5)));

                //    cfg.ConfigureEndpoints(context);
                //    cfg.UseInMemoryOutbox(context);
                //    EndpointConventionRegistration();
                //});
                x.AddInMemoryInboxOutbox();
            });
            return services;
        }

        private static void AddConsumers(this IRegistrationConfigurator cfg)
        {
        }

        private static void EndpointConventionRegistration()
        {
            EndpointConvention.Map<MassTransit.AzureServiceBus.Services.RequestResponse.CQRS.CommandDtoReturn>(new Uri("queue:mass-transit.azure-service-bus.services.request-response.cqrs.command-dto-return"));
            EndpointConvention.Map<MassTransit.AzureServiceBus.Services.RequestResponse.CQRS.CommandGuidReturn>(new Uri("queue:mass-transit.azure-service-bus.services.request-response.cqrs.command-guid-return"));
            EndpointConvention.Map<MassTransit.AzureServiceBus.Services.RequestResponse.CQRS.CommandNoParam>(new Uri("queue:mass-transit.azure-service-bus.services.request-response.cqrs.command-no-param"));
            EndpointConvention.Map<MassTransit.AzureServiceBus.Services.RequestResponse.CQRS.CommandVoidReturn>(new Uri("queue:mass-transit.azure-service-bus.services.request-response.cqrs.command-void-return"));
            EndpointConvention.Map<MassTransit.AzureServiceBus.Services.RequestResponse.CQRS.QueryGuidReturn>(new Uri("queue:mass-transit.azure-service-bus.services.request-response.cqrs.query-guid-return"));
            EndpointConvention.Map<MassTransit.AzureServiceBus.Services.RequestResponse.CQRS.QueryNoInputDtoReturnCollection>(new Uri("queue:mass-transit.azure-service-bus.services.request-response.cqrs.query-no-input-dto-return-collection"));
            EndpointConvention.Map<MassTransit.AzureServiceBus.Services.RequestResponse.CQRS.QueryResponseDtoReturn>(new Uri("queue:mass-transit.azure-service-bus.services.request-response.cqrs.query-response-dto-return"));
            EndpointConvention.Map<MassTransit.RabbitMQ.Services.RequestResponse.CQRS.CommandDtoReturn>(new Uri("queue:mass-transit.rabbit-mq.services.request-response.cqrs.command-dto-return"));
            EndpointConvention.Map<MassTransit.RabbitMQ.Services.RequestResponse.CQRS.CommandGuidReturn>(new Uri("queue:mass-transit.rabbit-mq.services.request-response.cqrs.command-guid-return"));
            EndpointConvention.Map<MassTransit.RabbitMQ.Services.RequestResponse.CQRS.CommandNoParam>(new Uri("queue:mass-transit.rabbit-mq.services.request-response.cqrs.command-no-param"));
            EndpointConvention.Map<MassTransit.RabbitMQ.Services.RequestResponse.CQRS.CommandVoidReturn>(new Uri("queue:mass-transit.rabbit-mq.services.request-response.cqrs.command-void-return"));
            EndpointConvention.Map<MassTransit.RabbitMQ.Services.RequestResponse.CQRS.QueryGuidReturn>(new Uri("queue:mass-transit.rabbit-mq.services.request-response.cqrs.query-guid-return"));
            EndpointConvention.Map<MassTransit.RabbitMQ.Services.RequestResponse.CQRS.QueryNoInputDtoReturnCollection>(new Uri("queue:mass-transit.rabbit-mq.services.request-response.cqrs.query-no-input-dto-return-collection"));
            EndpointConvention.Map<MassTransit.RabbitMQ.Services.RequestResponse.CQRS.QueryResponseDtoReturn>(new Uri("queue:mass-transit.rabbit-mq.services.request-response.cqrs.query-response-dto-return"));
        }
    }
}