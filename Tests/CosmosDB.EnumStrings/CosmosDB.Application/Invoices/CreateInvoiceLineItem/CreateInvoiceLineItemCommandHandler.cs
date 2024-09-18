using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CosmosDB.Domain.Common.Exceptions;
using CosmosDB.Domain.Entities;
using CosmosDB.Domain.Repositories;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "2.0")]

namespace CosmosDB.Application.Invoices.CreateInvoiceLineItem
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class CreateInvoiceLineItemCommandHandler : IRequestHandler<CreateInvoiceLineItemCommand, string>
    {
        private readonly IInvoiceRepository _invoiceRepository;

        [IntentManaged(Mode.Merge)]
        public CreateInvoiceLineItemCommandHandler(IInvoiceRepository invoiceRepository)
        {
            _invoiceRepository = invoiceRepository;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<string> Handle(CreateInvoiceLineItemCommand request, CancellationToken cancellationToken)
        {
            var aggregateRoot = await _invoiceRepository.FindByIdAsync(request.InvoiceId, cancellationToken);
            if (aggregateRoot is null)
            {
                throw new NotFoundException($"{nameof(Invoice)} of Id '{request.InvoiceId}' could not be found");
            }

            var newLineItem = new LineItem
            {
#warning No matching field found for InvoiceId
                Description = request.Description,
                Quantity = request.Quantity,
            };

            aggregateRoot.LineItems.Add(newLineItem);
            _invoiceRepository.Update(aggregateRoot);
            await _invoiceRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
            return newLineItem.Id;
        }
    }
}