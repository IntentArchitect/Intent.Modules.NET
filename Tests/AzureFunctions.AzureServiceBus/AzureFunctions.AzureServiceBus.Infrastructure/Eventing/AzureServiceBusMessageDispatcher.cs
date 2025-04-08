using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Azure.Messaging.ServiceBus;
using AzureFunctions.AzureServiceBus.Application.Common.Eventing;
using AzureFunctions.AzureServiceBus.Infrastructure.Configuration;
using Intent.RoslynWeaver.Attributes;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Eventing.AzureServiceBus.AzureServiceBusMessageDispatcher", Version = "1.0")]

namespace AzureFunctions.AzureServiceBus.Infrastructure.Eventing
{
    public class AzureServiceBusMessageDispatcher : IAzureServiceBusMessageDispatcher
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly Dictionary<string, DispatchHandler> _handlers;

        public AzureServiceBusMessageDispatcher(IServiceProvider serviceProvider, IOptions<SubscriptionOptions> options)
        {
            _serviceProvider = serviceProvider;
            _handlers = options.Value.Entries.ToDictionary(k => k.MessageType.FullName!, v => v.HandlerAsync);
        }

        public async Task DispatchAsync(ServiceBusReceivedMessage message, CancellationToken cancellationToken)
        {
            var messageTypeName = message.ApplicationProperties["MessageType"].ToString()!;

            if (_handlers.TryGetValue(messageTypeName, out var handlerAsync))
            {
                await handlerAsync(_serviceProvider, message, cancellationToken);
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