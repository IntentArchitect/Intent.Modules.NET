using EventingSubscribers.Application.Common.Eventing;
using EventingSubscribers.Domain;
using EventingSubscribers.Domain.Entities;
using EventingSubscribers.Domain.Repositories;
using EventingSubscribers.Eventing.Messages;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Eventing.Contracts.IntegrationEventHandler", Version = "1.0")]

namespace EventingSubscribers.Application.IntegrationEvents.EventHandlers
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class InvoiceCreatedEventHandler : IIntegrationEventHandler<InvoiceCreatedEvent>
    {
        private readonly IInvoiceRepository _invoiceRepository;

        [IntentManaged(Mode.Merge)]
        public InvoiceCreatedEventHandler(IInvoiceRepository invoiceRepository)
        {
            _invoiceRepository = invoiceRepository;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task HandleAsync(InvoiceCreatedEvent message, CancellationToken cancellationToken = default)
        {
            var createInvoice = new Invoice
            {
                Description = message.Description,
                BillingAddress = new Address(
                    street: message.BillingStreet,
                    city: message.BillingCity)
            };

            _invoiceRepository.Add(createInvoice);
        }
    }
}