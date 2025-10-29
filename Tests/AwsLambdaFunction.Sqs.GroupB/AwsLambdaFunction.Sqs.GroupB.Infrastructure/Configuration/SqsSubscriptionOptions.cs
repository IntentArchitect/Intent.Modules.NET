using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Amazon.Lambda.SQSEvents;
using AwsLambdaFunction.Sqs.GroupB.Application.Common.Eventing;
using AwsLambdaFunction.Sqs.GroupB.Infrastructure.Eventing;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Aws.Sqs.SqsSubscriptionOptions", Version = "1.0")]

namespace AwsLambdaFunction.Sqs.GroupB.Infrastructure.Configuration
{
    public class SqsSubscriptionOptions
    {
        private readonly List<SubscriptionEntry> _entries = [];

        public IReadOnlyList<SubscriptionEntry> Entries => _entries;

        public void Add<TMessage, THandler>()
            where TMessage : class
            where THandler : IIntegrationEventHandler<TMessage>
        {
            _entries.Add(new SubscriptionEntry(
                typeof(TMessage),
                SqsMessageDispatcher.InvokeDispatchHandler<TMessage, THandler>));
        }
    }

    public delegate Task DispatchHandler(
        IServiceProvider serviceProvider,
        SQSEvent.SQSMessage sqsMessage,
        CancellationToken cancellationToken);

    public record SubscriptionEntry(Type MessageType, DispatchHandler HandlerAsync);
}