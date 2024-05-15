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

namespace CleanArchitecture.Dapr.Application.OldMappingSystem.Invoices.DeleteInvoiceInvoiceLine
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class DeleteInvoiceInvoiceLineCommandHandler : IRequestHandler<DeleteInvoiceInvoiceLineCommand>
    {
        private readonly IInvoiceRepository _invoiceRepository;

        [IntentManaged(Mode.Merge)]
        public DeleteInvoiceInvoiceLineCommandHandler(IInvoiceRepository invoiceRepository)
        {
            _invoiceRepository = invoiceRepository;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task Handle(DeleteInvoiceInvoiceLineCommand request, CancellationToken cancellationToken)
        {
            var aggregateRoot = await _invoiceRepository.FindByIdAsync(request.InvoiceId, cancellationToken);
            if (aggregateRoot is null)
            {
                throw new NotFoundException($"{nameof(Invoice)} of Id '{request.InvoiceId}' could not be found");
            }

            var existingInvoiceLine = aggregateRoot.InvoiceLines.FirstOrDefault(p => p.Id == request.Id);
            if (existingInvoiceLine is null)
            {
                throw new NotFoundException($"{nameof(InvoiceLine)} of Id '{request.Id}' could not be found associated with {nameof(Invoice)} of Id '{request.InvoiceId}'");
            }

            aggregateRoot.InvoiceLines.Remove(existingInvoiceLine);

            _invoiceRepository.Update(aggregateRoot);

        }
    }
}