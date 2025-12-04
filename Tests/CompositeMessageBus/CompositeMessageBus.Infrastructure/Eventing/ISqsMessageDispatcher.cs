using Amazon.Lambda.SQSEvents;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Aws.Sqs.SqsMessageDispatcherInterface", Version = "1.0")]

namespace CompositeMessageBus.Infrastructure.Eventing
{
    public interface ISqsMessageDispatcher
    {
        Task DispatchAsync(IServiceProvider scopedServiceProvider, SQSEvent.SQSMessage sqsMessage, CancellationToken cancellationToken);
    }
}