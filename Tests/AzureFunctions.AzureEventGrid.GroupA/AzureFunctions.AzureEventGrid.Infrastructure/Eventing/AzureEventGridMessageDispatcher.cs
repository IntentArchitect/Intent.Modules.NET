using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Azure.Messaging;
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
        private readonly Dictionary<string, AzureEventGridDispatchHandler> _handlers;

        public AzureEventGridMessageDispatcher(IOptions<AzureEventGridSubscriptionOptions> options)
        {
            _handlers = options.Value.Entries.ToDictionary(k => k.MessageType.FullName!, v => v.HandlerAsync);
        }

        public async Task DispatchAsync(
            IServiceProvider scopedServiceProvider,
            CloudEvent cloudEvent,
            CancellationToken cancellationToken)
        {
            var messageTypeName = cloudEvent.Type;

            if (_handlers.TryGetValue(messageTypeName, out var handlerAsync))
            {
                await handlerAsync(scopedServiceProvider, cloudEvent, cancellationToken);
            }
        }

        public static async Task InvokeDispatchHandler<TMessage, THandler>(
            IServiceProvider serviceProvider,
            CloudEvent cloudEvent,
            CancellationToken cancellationToken)
            where TMessage : class
            where THandler : IIntegrationEventHandler<TMessage>
        {
            var pipeline = serviceProvider.GetRequiredService<AzureEventGridBehaviors.AzureEventGridConsumerPipeline>();
            await pipeline.ExecuteAsync(cloudEvent, async (@event, token) =>
            {
                var messageObj = @event.Data?.ToObjectFromJson<TMessage>()!;
                var handler = serviceProvider.GetRequiredService<THandler>();
                await handler.HandleAsync(messageObj, token);
                return @event;
            }, cancellationToken);
        }
    }
}