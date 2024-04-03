using System;
using System.Threading;
using System.Threading.Tasks;
using Intent.RoslynWeaver.Attributes;
using Kafka.Producer.Domain.Entities;
using Kafka.Producer.Domain.Repositories;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "2.0")]

namespace Kafka.Producer.Application.Invoices.CreateInvoice
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class CreateInvoiceCommandHandler : IRequestHandler<CreateInvoiceCommand, Guid>
    {
        private readonly IInvoiceRepository _invoiceRepository;

        [IntentManaged(Mode.Merge)]
        public CreateInvoiceCommandHandler(IInvoiceRepository invoiceRepository)
        {
            _invoiceRepository = invoiceRepository;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<Guid> Handle(CreateInvoiceCommand request, CancellationToken cancellationToken)
        {
            var invoice = new Invoice(
                note: request.Note);

            _invoiceRepository.Add(invoice);
            await _invoiceRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
            return invoice.Id;
        }
    }
}