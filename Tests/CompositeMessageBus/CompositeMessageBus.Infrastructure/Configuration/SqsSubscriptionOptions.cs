using Amazon.Lambda.SQSEvents;
using CompositeMessageBus.Application.Common.Eventing;
using CompositeMessageBus.Infrastructure.Eventing;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Aws.Sqs.SqsSubscriptionOptions", Version = "1.0")]

namespace CompositeMessageBus.Infrastructure.Configuration
{
    public class SqsSubscriptionOptions
    {
        private readonly List<SqsSubscriptionEntry> _entries = [];

        public IReadOnlyList<SqsSubscriptionEntry> Entries => _entries;

        public void Add<TMessage, THandler>()
            where TMessage : class
            where THandler : IIntegrationEventHandler<TMessage>
        {
            _entries.Add(new SqsSubscriptionEntry(
                typeof(TMessage),
                SqsMessageDispatcher.InvokeDispatchHandler<TMessage, THandler>));
        }
    }

    public delegate Task SqsDispatchHandler(
        IServiceProvider serviceProvider,
        SQSEvent.SQSMessage sqsMessage,
        CancellationToken cancellationToken);

    public record SqsSubscriptionEntry(Type MessageType, SqsDispatchHandler HandlerAsync);
}