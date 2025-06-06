using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Azure.Messaging;
using Azure.Messaging.EventGrid;
using AzureFunctions.AzureEventGrid.Application.Common.Eventing;
using AzureFunctions.AzureEventGrid.Infrastructure.Configuration;
using AzureFunctions.AzureEventGrid.Infrastructure.Eventing.Behaviors;
using Intent.RoslynWeaver.Attributes;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Eventing.AzureEventGrid.AzureEventGridMessageDispatcher", Version = "1.0")]

namespace AzureFunctions.AzureEventGrid.Infrastructure.Eventing
{
    public class AzureEventGridMessageDispatcher : IAzureEventGridMessageDispatcher
    {
        private readonly Dictionary<string, DispatchHandler> _handlers;

        public AzureEventGridMessageDispatcher(IOptions<SubscriptionOptions> options)
        {
            _handlers = options.Value.Entries.ToDictionary(k => k.MessageType.FullName!, v => v.HandlerAsync);
        }

        public async Task DispatchAsync(
            IServiceProvider scopedServiceProvider,
            CloudEvent message,
            CancellationToken cancellationToken)
        {
            var messageTypeName = message.Type;

            if (_handlers.TryGetValue(messageTypeName, out var handlerAsync))
            {
                await handlerAsync(scopedServiceProvider, message, cancellationToken);
            }
        }

        public static async Task InvokeDispatchHandler<TMessage, THandler>(
            IServiceProvider serviceProvider,
            CloudEvent message,
            CancellationToken cancellationToken)
            where TMessage : class
            where THandler : IIntegrationEventHandler<TMessage>
        {
            var pipeline = serviceProvider.GetRequiredService<AzureEventGridConsumerPipeline>();
            await pipeline.ExecuteAsync(message, async (@event, token) =>
            {
                var messageObj = message.Data?.ToObjectFromJson<TMessage>();
                var handler = serviceProvider.GetRequiredService<THandler>();
                await handler.HandleAsync(messageObj, token);
                return @event;
            }, cancellationToken);
        }
    }
}