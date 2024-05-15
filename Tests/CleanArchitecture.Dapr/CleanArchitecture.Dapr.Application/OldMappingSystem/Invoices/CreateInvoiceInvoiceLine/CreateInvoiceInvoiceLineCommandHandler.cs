using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CleanArchitecture.Dapr.Domain.Common.Exceptions;
using CleanArchitecture.Dapr.Domain.Entities;
using CleanArchitecture.Dapr.Domain.Repositories;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "2.0")]

namespace CleanArchitecture.Dapr.Application.OldMappingSystem.Invoices.CreateInvoiceInvoiceLine
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class CreateInvoiceInvoiceLineCommandHandler : IRequestHandler<CreateInvoiceInvoiceLineCommand, string>
    {
        private readonly IInvoiceRepository _invoiceRepository;

        [IntentManaged(Mode.Merge)]
        public CreateInvoiceInvoiceLineCommandHandler(IInvoiceRepository invoiceRepository)
        {
            _invoiceRepository = invoiceRepository;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<string> Handle(CreateInvoiceInvoiceLineCommand request, CancellationToken cancellationToken)
        {
            var aggregateRoot = await _invoiceRepository.FindByIdAsync(request.InvoiceId, cancellationToken);
            if (aggregateRoot is null)
            {
                throw new NotFoundException($"{nameof(Invoice)} of Id '{request.InvoiceId}' could not be found");
            }

            var newInvoiceLine = new InvoiceLine
            {
#warning No matching field found for InvoiceId
                Description = request.Description,
                Quantity = request.Quantity,
            };

            aggregateRoot.InvoiceLines.Add(newInvoiceLine);
            _invoiceRepository.Update(aggregateRoot);
            await _invoiceRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
            return newInvoiceLine.Id;
        }
    }
}