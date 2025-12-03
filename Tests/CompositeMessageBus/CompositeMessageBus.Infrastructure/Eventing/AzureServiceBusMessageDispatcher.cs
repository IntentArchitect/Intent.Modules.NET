using System.Text.Json;
using Azure.Messaging.ServiceBus;
using CompositeMessageBus.Application.Common.Eventing;
using CompositeMessageBus.Infrastructure.Configuration;
using Intent.RoslynWeaver.Attributes;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Eventing.AzureServiceBus.AzureServiceBusMessageDispatcher", Version = "1.0")]

namespace CompositeMessageBus.Infrastructure.Eventing
{
    public class AzureServiceBusMessageDispatcher : IAzureServiceBusMessageDispatcher
    {
        private readonly Dictionary<string, AzureServiceBusDispatchHandler> _handlers;

        public AzureServiceBusMessageDispatcher(IOptions<AzureServiceBusSubscriptionOptions> options)
        {
            _handlers = options.Value.Entries.ToDictionary(k => k.MessageType.FullName!, v => v.HandlerAsync);
        }

        public async Task DispatchAsync(
            IServiceProvider scopedServiceProvider,
            ServiceBusReceivedMessage message,
            CancellationToken cancellationToken)
        {
            var messageTypeName = message.ApplicationProperties["MessageType"].ToString()!;

            if (_handlers.TryGetValue(messageTypeName, out var handlerAsync))
            {
                await handlerAsync(scopedServiceProvider, message, cancellationToken);
            }
        }

        public static async Task InvokeDispatchHandler<TMessage, THandler>(
            IServiceProvider serviceProvider,
            ServiceBusReceivedMessage message,
            CancellationToken cancellationToken)
            where TMessage : class
            where THandler : IIntegrationEventHandler<TMessage>
        {
            var messageObj = (await JsonSerializer.DeserializeAsync<TMessage>(message.Body.ToStream(), cancellationToken: cancellationToken))!;
            var handler = serviceProvider.GetRequiredService<THandler>();
            await handler.HandleAsync(messageObj, cancellationToken);
        }
    }
}