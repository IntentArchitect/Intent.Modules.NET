using System;
using System.Threading;
using System.Threading.Tasks;
using AwsLambdaFunction.Sqs.GroupA.Eventing.Messages;
using AwsLambdaFunction.Sqs.GroupB.Application.Common.Eventing;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Eventing.Contracts.IntegrationEventHandler", Version = "1.0")]

namespace AwsLambdaFunction.Sqs.GroupB.Application.IntegrationEvents.EventHandlers
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class ClientCreatedEventHandler : IIntegrationEventHandler<ClientCreatedEvent>
    {
        [IntentManaged(Mode.Merge)]
        public ClientCreatedEventHandler()
        {
        }

        [IntentManaged(Mode.Fully, Body = Mode.Merge)]
        public async Task HandleAsync(ClientCreatedEvent message, CancellationToken cancellationToken = default)
        {
            // TODO: Implement HandleAsync (ClientCreatedEventHandler) functionality
            // TODO: Implement HandleAsync (ClntCrtdEvntHndlr) functionality                                                                                                                                                                    throw new NotImplementedException("Implement your handler logic here...");
        }
    }
}