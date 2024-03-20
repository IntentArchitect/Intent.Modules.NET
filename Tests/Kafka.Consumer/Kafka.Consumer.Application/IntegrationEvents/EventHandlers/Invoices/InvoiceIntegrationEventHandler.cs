using System;
using System.Threading;
using System.Threading.Tasks;
using Intent.RoslynWeaver.Attributes;
using Kafka.Consumer.Application.Common.Eventing;
using Kafka.Consumer.Application.Invoices.CreateInvoice;
using Kafka.Consumer.Application.Invoices.UpdateInvoice;
using Kafka.Producer.Eventing.Messages;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Eventing.Kafka.IntegrationEventHandler", Version = "1.0")]

namespace Kafka.Consumer.Application.IntegrationEvents.EventHandlers.Invoices
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class InvoiceIntegrationEventHandler : IIntegrationEventHandler<InvoiceCreatedEvent>, IIntegrationEventHandler<InvoiceUpdatedEvent>
    {
        private readonly ISender _mediator;
        [IntentManaged(Mode.Merge)]
        public InvoiceIntegrationEventHandler(ISender mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task HandleAsync(InvoiceCreatedEvent message, CancellationToken cancellationToken = default)
        {
            var command = new CreateInvoiceCommand(id: message.Id, note: message.Note);

            await _mediator.Send(command, cancellationToken);
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task HandleAsync(InvoiceUpdatedEvent message, CancellationToken cancellationToken = default)
        {
            var command = new UpdateInvoiceCommand(id: message.Id, note: message.Note);

            await _mediator.Send(command, cancellationToken);
        }
    }
}