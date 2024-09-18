using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CosmosDB.Domain.Entities;
using CosmosDB.Domain.Repositories;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "2.0")]

namespace CosmosDB.Application.Invoices.CreateInvoice
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class CreateInvoiceCommandHandler : IRequestHandler<CreateInvoiceCommand, string>
    {
        private readonly IInvoiceRepository _invoiceRepository;

        [IntentManaged(Mode.Merge)]
        public CreateInvoiceCommandHandler(IInvoiceRepository invoiceRepository)
        {
            _invoiceRepository = invoiceRepository;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<string> Handle(CreateInvoiceCommand request, CancellationToken cancellationToken)
        {
            var newInvoice = new Invoice(request.ClientIdentifier, request.Date, request.Number);

            _invoiceRepository.Add(newInvoice);
            await _invoiceRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
            return newInvoice.Id;
        }
    }
}