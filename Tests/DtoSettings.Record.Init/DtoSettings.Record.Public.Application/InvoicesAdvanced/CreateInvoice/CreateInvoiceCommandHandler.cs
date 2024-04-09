using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DtoSettings.Record.Public.Domain.Entities;
using DtoSettings.Record.Public.Domain.Repositories;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "2.0")]

namespace DtoSettings.Record.Public.Application.InvoicesAdvanced.CreateInvoice
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
            var invoice = new Invoice
            {
                Number = request.Number,
                InvoiceLines = request.InvoiceLines
                    .Select(il => new InvoiceLine
                    {
                        Description = il.Description,
                        Amount = il.Amount,
                        Currency = il.Currency
                    })
                    .ToList()
            };

            _invoiceRepository.Add(invoice);
            await _invoiceRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
            return invoice.Id;
        }
    }
}