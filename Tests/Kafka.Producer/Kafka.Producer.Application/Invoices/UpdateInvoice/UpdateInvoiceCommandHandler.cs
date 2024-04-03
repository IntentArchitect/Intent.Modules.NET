using System;
using System.Threading;
using System.Threading.Tasks;
using Intent.RoslynWeaver.Attributes;
using Kafka.Producer.Domain.Common.Exceptions;
using Kafka.Producer.Domain.Repositories;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "2.0")]

namespace Kafka.Producer.Application.Invoices.UpdateInvoice
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class UpdateInvoiceCommandHandler : IRequestHandler<UpdateInvoiceCommand>
    {
        private readonly IInvoiceRepository _invoiceRepository;

        [IntentManaged(Mode.Merge)]
        public UpdateInvoiceCommandHandler(IInvoiceRepository invoiceRepository)
        {
            _invoiceRepository = invoiceRepository;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task Handle(UpdateInvoiceCommand request, CancellationToken cancellationToken)
        {
            var invoice = await _invoiceRepository.FindByIdAsync(request.Id, cancellationToken);
            if (invoice is null)
            {
                throw new NotFoundException($"Could not find Invoice '{request.Id}'");
            }

            invoice.Update(request.Note);
        }
    }
}