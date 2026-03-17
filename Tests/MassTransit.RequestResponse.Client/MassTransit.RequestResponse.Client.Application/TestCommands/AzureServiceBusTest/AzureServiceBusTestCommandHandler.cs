using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "2.0")]

namespace MassTransit.RequestResponse.Client.Application.TestCommands.AzureServiceBusTest
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class AzureServiceBusTestCommandHandler : IRequestHandler<AzureServiceBusTestCommand>
    {
        [IntentManaged(Mode.Merge)]
        public AzureServiceBusTestCommandHandler()
        {
        }

        [IntentManaged(Mode.Fully, Body = Mode.Merge)]
        public async Task Handle(AzureServiceBusTestCommand request, CancellationToken cancellationToken)
        {
            // TODO: Implement Handle (AzureServiceBusTestCommandHandler) functionality
            throw new NotImplementedException("Your implementation here...");
        }
    }
}