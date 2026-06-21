using Intent.RoslynWeaver.Attributes;
using MediatR;
using N_ServiceBus.AzureServiceBus.Application.Common.Eventing;
using N_ServiceBus.AzureServiceBus.Eventing.Messages;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "2.0")]

namespace N_ServiceBus.AzureServiceBus.Application.People.CreatePerson
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class CreatePersonCommandHandler : IRequestHandler<CreatePersonCommand>
    {
        private readonly IMessageBus _messageBus;

        [IntentManaged(Mode.Merge)]
        public CreatePersonCommandHandler(IMessageBus messageBus)
        {
            _messageBus = messageBus;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task Handle(CreatePersonCommand request, CancellationToken cancellationToken)
        {
            _messageBus.Send(new CreatePersonIdentity
            {
                FirstName = request.FirstName,
                LastName = request.LastName
            });
        }
    }
}