using System;
using System.Threading;
using System.Threading.Tasks;
using Intent.RoslynWeaver.Attributes;
using MediatR;
using Solace.Tests.Application.Accounts.CreateAccount;
using Solace.Tests.Application.Common.Eventing;
using Solace.Tests.Eventing.Messages;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Eventing.Solace.IntegrationEventHandler", Version = "1.0")]

namespace Solace.Tests.Application.IntegrationEvents.EventHandlers.Customers
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class CustomerCreatedHandler : IIntegrationEventHandler<CustomerCreatedEvent>
    {
        private readonly ISender _mediator;

        [IntentManaged(Mode.Merge)]
        public CustomerCreatedHandler(ISender mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task HandleAsync(CustomerCreatedEvent message, CancellationToken cancellationToken = default)
        {
            //IntentIgnore
            Console.WriteLine("Received:CustomerCreatedEvent");

            var command = new CreateAccountCommand(customerId: message.Id);

            await _mediator.Send(command, cancellationToken);
        }
    }
}