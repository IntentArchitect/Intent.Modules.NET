using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CosmosDB.Domain.Common.Exceptions;
using CosmosDB.Domain.Repositories;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "2.0")]

namespace CosmosDB.Application.Invoices.UpdateInvoiceByOp
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class UpdateInvoiceByOpCommandHandler : IRequestHandler<UpdateInvoiceByOpCommand>
    {
        private readonly IInvoiceRepository _invoiceRepository;

        [IntentManaged(Mode.Merge)]
        public UpdateInvoiceByOpCommandHandler(IInvoiceRepository invoiceRepository)
        {
            _invoiceRepository = invoiceRepository;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task Handle(UpdateInvoiceByOpCommand request, CancellationToken cancellationToken)
        {
            var existingInvoice = await _invoiceRepository.FindByIdAsync(request.Id, cancellationToken);
            if (existingInvoice is null)
            {
                throw new NotFoundException($"Could not find Invoice '{request.Id}'");
            }

            existingInvoice.Update(request.Date, request.Number, request.ClientIdentifier);

            _invoiceRepository.Update(existingInvoice);
        }
    }
}