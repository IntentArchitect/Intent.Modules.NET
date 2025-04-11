using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Azure.Messaging.EventGrid;
using AzureFunctions.AzureEventGrid.Application.Common.Eventing;
using AzureFunctions.AzureEventGrid.Infrastructure.Configuration;
using Intent.RoslynWeaver.Attributes;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Eventing.AzureEventGrid.AzureEventGridMessageDispatcher", Version = "1.0")]

namespace AzureFunctions.AzureEventGrid.Infrastructure.Eventing
{
    public class AzureEventGridMessageDispatcher : IAzureEventGridMessageDispatcher
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly Dictionary<string, DispatchHandler> _handlers;

        public AzureEventGridMessageDispatcher(IServiceProvider serviceProvider, IOptions<SubscriptionOptions> options)
        {
            _serviceProvider = serviceProvider;
            _handlers = options.Value.Entries.ToDictionary(k => k.MessageType.FullName!, v => v.HandlerAsync);
        }

        public async Task DispatchAsync(EventGridEvent message, CancellationToken cancellationToken)
        {
            var messageTypeName = message.EventType;

            if (_handlers.TryGetValue(messageTypeName, out var handlerAsync))
            {
                await handlerAsync(_serviceProvider, message, cancellationToken);
            }
        }

        public static async Task InvokeDispatchHandler<TMessage, THandler>(
            IServiceProvider serviceProvider,
            EventGridEvent message,
            CancellationToken cancellationToken)
            where TMessage : class
            where THandler : IIntegrationEventHandler<TMessage>
        {
            var messageObj = message.Data.ToObjectFromJson<TMessage>()!;
            var handler = serviceProvider.GetRequiredService<THandler>();
            await handler.HandleAsync(messageObj, cancellationToken);
        }
    }
}