using Intent.RoslynWeaver.Attributes;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NServiceBus.LearnerTransport.Application.Common.Eventing;
using NServiceBus.LearnerTransport.Eventing.Messages;
using NServiceBus.LearnerTransport.Infrastructure.Eventing;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Eventing.NServiceBus.NServiceBusConfiguration", Version = "1.0")]

namespace NServiceBus.LearnerTransport.Infrastructure.Configuration
{
    public static class NServiceBusConfiguration
    {
        public static IHostBuilder AddNServiceBus(this IHostBuilder hostBuilder, IConfiguration configuration)
        {
            return hostBuilder.UseNServiceBus(ctx => ConfigureEndpoint(configuration));
        }

        public static IServiceCollection AddNServiceBusConfiguration(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            services.AddScoped<NServiceBusMessageBus>();
            services.AddScoped<IMessageBus>(provider => provider.GetRequiredService<NServiceBusMessageBus>());
            return services;
        }

        private static EndpointConfiguration ConfigureEndpoint(IConfiguration configuration)
        {
            var endpointName = configuration["NServiceBus:EndpointName"] ?? throw new InvalidOperationException("NServiceBus:EndpointName is not configured");
            var endpointConfiguration = new EndpointConfiguration(endpointName);

            var rawStoragePath = configuration["NServiceBus:LearningTransport:StorageDirectory"];
            var storageDirectory = rawStoragePath is not null
    ? Environment.ExpandEnvironmentVariables(rawStoragePath)
    : Path.Combine(Path.GetTempPath(), "nservicebus-learning");
            var transportConfig = endpointConfiguration.UseTransport(new LearningTransport { StorageDirectory = storageDirectory });

            endpointConfiguration.EnableInstallers();
            endpointConfiguration.UseSerialization<SystemJsonSerializer>();

            var conventions = endpointConfiguration.Conventions();
            conventions.DefiningEventsAs(new[] { typeof(TestMessageEvent) }.Contains);
            conventions.DefiningCommandsAs(new[] { typeof(CreatePersonIdentity), typeof(OrderAnimal), typeof(TalkToPersonCommand), typeof(MakeSoundCommand) }.Contains);

            endpointConfiguration.Recoverability()
                .Immediate(r => r.NumberOfRetries(configuration.GetValue<int>("NServiceBus:Recoverability:ImmediateRetries", 5)))
                .Delayed(r => r.NumberOfRetries(configuration.GetValue<int>("NServiceBus:Recoverability:DelayedRetries", 3)).TimeIncrease(TimeSpan.FromSeconds(configuration.GetValue<int>("NServiceBus:Recoverability:DelayIncreaseSeconds", 10))));
            endpointConfiguration.SendFailedMessagesTo(configuration["NServiceBus:ErrorQueue"] ?? "error");

            transportConfig.RouteToEndpoint(typeof(CreatePersonIdentity), configuration["NServiceBus:Routing:Commands:CreatePersonIdentity"] ?? "CreatePersonIdentity");
            transportConfig.RouteToEndpoint(typeof(OrderAnimal), configuration["NServiceBus:Routing:Commands:OrderAnimal"] ?? "Animals");
            transportConfig.RouteToEndpoint(typeof(TalkToPersonCommand), configuration["NServiceBus:Routing:Commands:TalkToPersonCommand"] ?? "TalkToPersonCommand");
            transportConfig.RouteToEndpoint(typeof(MakeSoundCommand), configuration["NServiceBus:Routing:Commands:MakeSoundCommand"] ?? "Animals");
            RegisterHandler<NServiceBusMessageHandler<TestMessageEvent>, TestMessageEvent>(endpointConfiguration);
            RegisterHandler<NServiceBusMessageHandler<OrderAnimal>, OrderAnimal>(endpointConfiguration);
            RegisterHandler<NServiceBusMessageHandler<MakeSoundCommand>, MakeSoundCommand>(endpointConfiguration);
            RegisterHandler<NServiceBusMessageHandler<TalkToPersonCommand>, TalkToPersonCommand>(endpointConfiguration);
            RegisterHandler<NServiceBusMessageHandler<CreatePersonIdentity>, CreatePersonIdentity>(endpointConfiguration);

            return endpointConfiguration;
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