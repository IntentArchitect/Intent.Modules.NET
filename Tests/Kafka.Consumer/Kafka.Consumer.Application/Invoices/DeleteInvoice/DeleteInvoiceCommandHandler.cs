using System;
using System.Threading;
using System.Threading.Tasks;
using Intent.RoslynWeaver.Attributes;
using Kafka.Consumer.Domain.Common.Exceptions;
using Kafka.Consumer.Domain.Repositories;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "2.0")]

namespace Kafka.Consumer.Application.Invoices.DeleteInvoice
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class DeleteInvoiceCommandHandler : IRequestHandler<DeleteInvoiceCommand>
    {
        private readonly IInvoiceRepository _invoiceRepository;

        [IntentManaged(Mode.Merge)]
        public DeleteInvoiceCommandHandler(IInvoiceRepository invoiceRepository)
        {
            _invoiceRepository = invoiceRepository;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task Handle(DeleteInvoiceCommand request, CancellationToken cancellationToken)
        {
            var invoice = await _invoiceRepository.FindByIdAsync(request.Id, cancellationToken);
            if (invoice is null)
            {
                throw new NotFoundException($"Could not find Invoice '{request.Id}'");
            }

            _invoiceRepository.Remove(invoice);
        }
    }
}