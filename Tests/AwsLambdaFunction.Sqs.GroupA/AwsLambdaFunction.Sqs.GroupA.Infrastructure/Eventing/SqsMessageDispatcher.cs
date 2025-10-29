using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Amazon.Lambda.SQSEvents;
using AwsLambdaFunction.Sqs.GroupA.Application.Common.Eventing;
using AwsLambdaFunction.Sqs.GroupA.Infrastructure.Configuration;
using Intent.RoslynWeaver.Attributes;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Aws.Sqs.SqsMessageDispatcher", Version = "1.0")]

namespace AwsLambdaFunction.Sqs.GroupA.Infrastructure.Eventing
{
    public class SqsMessageDispatcher : ISqsMessageDispatcher
    {
        private readonly Dictionary<string, DispatchHandler> _handlers;

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