using System;
using System.Threading;
using System.Threading.Tasks;
using AwsLambdaFunction.Sqs.GroupA.Application.Common.Eventing;
using AwsLambdaFunction.Sqs.GroupB.Eventing.Messages;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Aws.Sqs.IntegrationEventHandler", Version = "1.0")]

namespace AwsLambdaFunction.Sqs.GroupA.Application.IntegrationEvents.EventHandlers
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class SpecificTopicMessageHandler : IIntegrationEventHandler<SpecificTopicOneMessageEvent>, IIntegrationEventHandler<SpecificTopicTwoMessage>
    {
        [IntentManaged(Mode.Merge)]
        public SpecificTopicMessageHandler()
        {
        }

        [IntentManaged(Mode.Fully, Body = Mode.Merge)]
        public async Task HandleAsync(SpecificTopicOneMessageEvent message, CancellationToken cancellationToken = default)
        {
            // TODO: Implement HandleAsync (SpecificTopicMessageHandler) functionality
            throw new NotImplementedException("Implement your handler logic here...");
        }

        [IntentManaged(Mode.Fully, Body = Mode.Merge)]
        public async Task HandleAsync(SpecificTopicTwoMessage message, CancellationToken cancellationToken = default)
        {
            // TODO: Implement HandleAsync (SpecificTopicMessageHandler) functionality
            throw new NotImplementedException("Implement your handler logic here...");
        }
    }
}