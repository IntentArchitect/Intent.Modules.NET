using Intent.RoslynWeaver.Attributes;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NServiceBus.AzureServiceBus.Application.Common.Eventing;
using NServiceBus.AzureServiceBus.Eventing.Messages;
using NServiceBus.AzureServiceBus.Infrastructure.Eventing;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Eventing.NServiceBus.NServiceBusConfiguration", Version = "1.0")]

namespace NServiceBus.AzureServiceBus.Infrastructure.Configuration
{
    public static class NServiceBusConfiguration
    {

        public static IServiceCollection AddNServiceBusConfiguration(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            services.AddScoped<NServiceBusMessageBus>();
            services.AddScoped<IMessageBus>(provider => provider.GetRequiredService<NServiceBusMessageBus>());

            services.AddNServiceBusEndpoint(ConfigureEndpointForAnimals(configuration));
            services.AddNServiceBusEndpoint(ConfigureEndpointForMakeSoundCommand(configuration));
            services.AddNServiceBusEndpoint(ConfigureEndpointForTalkToPersonCommand(configuration));
            services.AddNServiceBusEndpoint(ConfigureEndpointForCreatePersonIdentity(configuration));
            services.AddNServiceBusEndpoint(ConfigureMainEndpoint(configuration));
            return services;
        }

        private static EndpointConfiguration ConfigureMainEndpoint(IConfiguration configuration)
        {
            var endpointName = configuration["NServiceBus:EndpointName"] ?? throw new InvalidOperationException("NServiceBus:EndpointName is not configured");
            var endpointConfiguration = new EndpointConfiguration(endpointName);

            var connectionString = configuration.GetConnectionString("AzureServiceBus") ?? throw new InvalidOperationException("ConnectionStrings:AzureServiceBus is not configured");
            endpointConfiguration.UseTransport(new AzureServiceBusTransport(connectionString, TopicTopology.Default));

            endpointConfiguration.EnableInstallers();
            endpointConfiguration.UseSerialization<SystemJsonSerializer>();

            var conventions = endpointConfiguration.Conventions();
            conventions.DefiningEventsAs(new[] { typeof(TestMessageEvent) }.Contains);

            endpointConfiguration.Recoverability()
                .Immediate(r => r.NumberOfRetries(configuration.GetValue<int>("NServiceBus:Recoverability:ImmediateRetries", 5)))
                .Delayed(r => r.NumberOfRetries(configuration.GetValue<int>("NServiceBus:Recoverability:DelayedRetries", 3)).TimeIncrease(TimeSpan.FromSeconds(configuration.GetValue<int>("NServiceBus:Recoverability:DelayIncreaseSeconds", 10))));
            endpointConfiguration.SendFailedMessagesTo(configuration["NServiceBus:ErrorQueue"] ?? "error");
            RegisterHandler<NServiceBusMessageHandler<TestMessageEvent>, TestMessageEvent>(endpointConfiguration);

            return endpointConfiguration;
        }

        private static EndpointConfiguration ConfigureEndpointForAnimals(IConfiguration configuration)
        {
            var endpointName = "Animals";
            var endpointConfiguration = new EndpointConfiguration(endpointName);

            var connectionString = configuration.GetConnectionString("AzureServiceBus") ?? throw new InvalidOperationException("ConnectionStrings:AzureServiceBus is not configured");
            endpointConfiguration.UseTransport(new AzureServiceBusTransport(connectionString, TopicTopology.Default));

            endpointConfiguration.EnableInstallers();
            endpointConfiguration.UseSerialization<SystemJsonSerializer>();

            var conventions = endpointConfiguration.Conventions();
            conventions.DefiningCommandsAs(new[] { typeof(OrderAnimal) }.Contains);

            endpointConfiguration.Recoverability()
                .Immediate(r => r.NumberOfRetries(configuration.GetValue<int>("NServiceBus:Recoverability:ImmediateRetries", 5)))
                .Delayed(r => r.NumberOfRetries(configuration.GetValue<int>("NServiceBus:Recoverability:DelayedRetries", 3)).TimeIncrease(TimeSpan.FromSeconds(configuration.GetValue<int>("NServiceBus:Recoverability:DelayIncreaseSeconds", 10))));
            endpointConfiguration.SendFailedMessagesTo(configuration["NServiceBus:ErrorQueue"] ?? "error");
            RegisterHandler<NServiceBusMessageHandler<OrderAnimal>, OrderAnimal>(endpointConfiguration);

            return endpointConfiguration;
        }

        private static EndpointConfiguration ConfigureEndpointForMakeSoundCommand(IConfiguration configuration)
        {
            var endpointName = configuration["NServiceBus:Routing:Commands:MakeSoundCommand"] ?? "make-sound-command";
            var endpointConfiguration = new EndpointConfiguration(endpointName);

            var connectionString = configuration.GetConnectionString("AzureServiceBus") ?? throw new InvalidOperationException("ConnectionStrings:AzureServiceBus is not configured");
            endpointConfiguration.UseTransport(new AzureServiceBusTransport(connectionString, TopicTopology.Default));

            endpointConfiguration.EnableInstallers();
            endpointConfiguration.UseSerialization<SystemJsonSerializer>();

            var conventions = endpointConfiguration.Conventions();
            conventions.DefiningCommandsAs(new[] { typeof(MakeSoundCommand) }.Contains);

            endpointConfiguration.Recoverability()
                .Immediate(r => r.NumberOfRetries(configuration.GetValue<int>("NServiceBus:Recoverability:ImmediateRetries", 5)))
                .Delayed(r => r.NumberOfRetries(configuration.GetValue<int>("NServiceBus:Recoverability:DelayedRetries", 3)).TimeIncrease(TimeSpan.FromSeconds(configuration.GetValue<int>("NServiceBus:Recoverability:DelayIncreaseSeconds", 10))));
            endpointConfiguration.SendFailedMessagesTo(configuration["NServiceBus:ErrorQueue"] ?? "error");
            RegisterHandler<NServiceBusMessageHandler<MakeSoundCommand>, MakeSoundCommand>(endpointConfiguration);

            return endpointConfiguration;
        }

        private static EndpointConfiguration ConfigureEndpointForTalkToPersonCommand(IConfiguration configuration)
        {
            var endpointName = configuration["NServiceBus:Routing:Commands:TalkToPersonCommand"] ?? "talk-to-person-command";
            var endpointConfiguration = new EndpointConfiguration(endpointName);

            var connectionString = configuration.GetConnectionString("AzureServiceBus") ?? throw new InvalidOperationException("ConnectionStrings:AzureServiceBus is not configured");
            endpointConfiguration.UseTransport(new AzureServiceBusTransport(connectionString, TopicTopology.Default));

            endpointConfiguration.EnableInstallers();
            endpointConfiguration.UseSerialization<SystemJsonSerializer>();

            var conventions = endpointConfiguration.Conventions();
            conventions.DefiningCommandsAs(new[] { typeof(TalkToPersonCommand) }.Contains);

            endpointConfiguration.Recoverability()
                .Immediate(r => r.NumberOfRetries(configuration.GetValue<int>("NServiceBus:Recoverability:ImmediateRetries", 5)))
                .Delayed(r => r.NumberOfRetries(configuration.GetValue<int>("NServiceBus:Recoverability:DelayedRetries", 3)).TimeIncrease(TimeSpan.FromSeconds(configuration.GetValue<int>("NServiceBus:Recoverability:DelayIncreaseSeconds", 10))));
            endpointConfiguration.SendFailedMessagesTo(configuration["NServiceBus:ErrorQueue"] ?? "error");
            RegisterHandler<NServiceBusMessageHandler<TalkToPersonCommand>, TalkToPersonCommand>(endpointConfiguration);

            return endpointConfiguration;
        }

        private static EndpointConfiguration ConfigureEndpointForCreatePersonIdentity(IConfiguration configuration)
        {
            var endpointName = configuration["NServiceBus:Routing:Commands:CreatePersonIdentity"] ?? "create-person-identity";
            var endpointConfiguration = new EndpointConfiguration(endpointName);

            var connectionString = configuration.GetConnectionString("AzureServiceBus") ?? throw new InvalidOperationException("ConnectionStrings:AzureServiceBus is not configured");
            endpointConfiguration.UseTransport(new AzureServiceBusTransport(connectionString, TopicTopology.Default));

            endpointConfiguration.EnableInstallers();
            endpointConfiguration.UseSerialization<SystemJsonSerializer>();

            var conventions = endpointConfiguration.Conventions();
            conventions.DefiningCommandsAs(new[] { typeof(CreatePersonIdentity) }.Contains);

            endpointConfiguration.Recoverability()
                .Immediate(r => r.NumberOfRetries(configuration.GetValue<int>("NServiceBus:Recoverability:ImmediateRetries", 5)))
                .Delayed(r => r.NumberOfRetries(configuration.GetValue<int>("NServiceBus:Recoverability:DelayedRetries", 3)).TimeIncrease(TimeSpan.FromSeconds(configuration.GetValue<int>("NServiceBus:Recoverability:DelayIncreaseSeconds", 10))));
            endpointConfiguration.SendFailedMessagesTo(configuration["NServiceBus:ErrorQueue"] ?? "error");
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