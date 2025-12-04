using System.Text.Json;
using Amazon.Lambda.SQSEvents;
using CompositeMessageBus.Application.Common.Eventing;
using CompositeMessageBus.Infrastructure.Configuration;
using Intent.RoslynWeaver.Attributes;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Aws.Sqs.SqsMessageDispatcher", Version = "1.0")]

namespace CompositeMessageBus.Infrastructure.Eventing
{
    public class SqsMessageDispatcher : ISqsMessageDispatcher
    {
        private readonly Dictionary<string, SqsDispatchHandler> _handlers;

        public SqsMessageDispatcher(IOptions<SqsSubscriptionOptions> options)
        {
            _handlers = options.Value.Entries.ToDictionary(k => k.MessageType.FullName!, v => v.HandlerAsync);
        }

        public async Task DispatchAsync(
            IServiceProvider scopedServiceProvider,
            SQSEvent.SQSMessage sqsMessage,
            CancellationToken cancellationToken)
        {
            var messageTypeName = sqsMessage.MessageAttributes
    .TryGetValue("MessageType", out var messageTypeAttr)
    ? messageTypeAttr.StringValue
    : throw new Exception("MessageType attribute is missing");

            if (_handlers.TryGetValue(messageTypeName, out var handlerAsync))
            {
                await handlerAsync(scopedServiceProvider, sqsMessage, cancellationToken);
            }
        }

        public static async Task InvokeDispatchHandler<TMessage, THandler>(
            IServiceProvider serviceProvider,
            SQSEvent.SQSMessage sqsMessage,
            CancellationToken cancellationToken)
            where TMessage : class
            where THandler : IIntegrationEventHandler<TMessage>
        {
            var messageObj = JsonSerializer.Deserialize<TMessage>(sqsMessage.Body)!;
            var handler = serviceProvider.GetRequiredService<THandler>();
            await handler.HandleAsync(messageObj, cancellationToken);
        }
    }
}