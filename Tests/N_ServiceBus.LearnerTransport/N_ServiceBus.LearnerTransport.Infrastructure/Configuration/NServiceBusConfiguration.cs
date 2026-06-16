using Intent.RoslynWeaver.Attributes;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using N_ServiceBus.LearnerTransport.Application.Common.Eventing;
using N_ServiceBus.LearnerTransport.Eventing.Messages;
using N_ServiceBus.LearnerTransport.Infrastructure.Eventing;
using NServiceBus;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Eventing.NServiceBus.NServiceBusConfiguration", Version = "1.0")]

namespace N_ServiceBus.LearnerTransport.Infrastructure.Configuration
{
    public static class NServiceBusConfiguration
    {

        public static IServiceCollection AddNServiceBusConfiguration(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            services.AddScoped<NServiceBusMessageBus>();
            services.AddScoped<IMessageBus>(provider => provider.GetRequiredService<NServiceBusMessageBus>());

            services.AddNServiceBusEndpoint(ConfigureMainEndpoint(configuration));
            return services;
        }

        private static EndpointConfiguration ConfigureMainEndpoint(IConfiguration configuration)
        {
            var endpointName = configuration["NServiceBus:EndpointName"] ?? throw new InvalidOperationException("NServiceBus:EndpointName is not configured");
            var endpointConfiguration = new EndpointConfiguration(endpointName);

            var rawStoragePath = configuration["NServiceBus:LearningTransport:StorageDirectory"];
            var storageDirectory = rawStoragePath is not null
                ? Environment.ExpandEnvironmentVariables(rawStoragePath)
                : Path.Combine(Path.GetTempPath(), "nservicebus-learning");
            var routing = endpointConfiguration.UseTransport(new LearningTransport { StorageDirectory = storageDirectory });

            endpointConfiguration.EnableInstallers();
            endpointConfiguration.UseSerialization<SystemJsonSerializer>();

            endpointConfiguration.Recoverability()
                .Immediate(r => r.NumberOfRetries(configuration.GetValue<int>("NServiceBus:Recoverability:ImmediateRetries", 5)))
                .Delayed(r => r.NumberOfRetries(configuration.GetValue<int>("NServiceBus:Recoverability:DelayedRetries", 3)).TimeIncrease(TimeSpan.FromSeconds(configuration.GetValue<int>("NServiceBus:Recoverability:DelayIncreaseSeconds", 10))));
            endpointConfiguration.SendFailedMessagesTo(configuration["NServiceBus:ErrorQueue"] ?? "error");

            ConfigureMessageConventions(endpointConfiguration);
            RegisterHandlers(endpointConfiguration);

            routing.RouteToEndpoint(typeof(OrderAnimal), endpointName);
            routing.RouteToEndpoint(typeof(MakeSoundCommand), endpointName);
            routing.RouteToEndpoint(typeof(TalkToPersonCommand), endpointName);
            routing.RouteToEndpoint(typeof(CreatePersonIdentity), endpointName);

            return endpointConfiguration;
        }

        private static void ConfigureMessageConventions(EndpointConfiguration endpointConfiguration)
        {
            var conventions = endpointConfiguration.Conventions();
            conventions.DefiningEventsAs(new[] { typeof(TestMessageEvent) }.Contains);
            conventions.DefiningCommandsAs(new[] { typeof(CreatePersonIdentity), typeof(OrderAnimal), typeof(TalkToPersonCommand), typeof(MakeSoundCommand) }.Contains);
        }

        private static void RegisterHandlers(EndpointConfiguration endpointConfiguration)
        {
            RegisterHandler<NServiceBusMessageHandler<TestMessageEvent>, TestMessageEvent>(endpointConfiguration);
            RegisterHandler<NServiceBusMessageHandler<OrderAnimal>, OrderAnimal>(endpointConfiguration);
            RegisterHandler<NServiceBusMessageHandler<MakeSoundCommand>, MakeSoundCommand>(endpointConfiguration);
            RegisterHandler<NServiceBusMessageHandler<TalkToPersonCommand>, TalkToPersonCommand>(endpointConfiguration);
            RegisterHandler<NServiceBusMessageHandler<CreatePersonIdentity>, CreatePersonIdentity>(endpointConfiguration);
        }

        private static void RegisterHandler<THandler, TMessage>(EndpointConfiguration endpointConfiguration)
            where THandler : class, IHandleMessages<TMessage>
            where TMessage : class
        {
            var settings = NServiceBus.Configuration.AdvancedExtensibility.AdvancedExtensibilityExtensions.GetSettings(endpointConfiguration);
            var messageHandlerRegistry = settings.GetOrCreate<NServiceBus.Unicast.MessageHandlerRegistry>();
            var messageMetadataRegistry = settings.GetOrCreate<NServiceBus.Unicast.Messages.MessageMetadataRegistry>();
            messageHandlerRegistry.AddMessageHandlerForMessage<THandler, TMessage>();
            messageMetadataRegistry.RegisterMessageTypeWithHierarchy(typeof(TMessage), Array.Empty<Type>());
        }
    }
}