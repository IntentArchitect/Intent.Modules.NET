using System;
using System.Threading;
using System.Threading.Tasks;
using AwsLambdaFunction.Sqs.GroupA.Eventing.Messages;
using AwsLambdaFunction.Sqs.GroupB.Application.Common.Eventing;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Aws.Sqs.IntegrationEventHandler", Version = "1.0")]

namespace AwsLambdaFunction.Sqs.GroupB.Application.IntegrationEvents.EventHandlers
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class CreateOrderCommandHandler : IIntegrationEventHandler<CreateOrderCommand>
    {
        [IntentManaged(Mode.Merge)]
        public CreateOrderCommandHandler()
        {
        }

        [IntentManaged(Mode.Fully, Body = Mode.Merge)]
        public async Task HandleAsync(CreateOrderCommand message, CancellationToken cancellationToken = default)
        {
            // TODO: Implement HandleAsync (CreateOrderCommandHandler) functionality
            // TODO: Implement HandleAsync (H2) functionality                                                                                                                                      throw new NotImplementedException("Implement your handler logic here...");
        }
    }
}