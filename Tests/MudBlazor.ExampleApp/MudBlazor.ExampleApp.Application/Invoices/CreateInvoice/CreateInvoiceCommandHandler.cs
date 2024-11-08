using Intent.RoslynWeaver.Attributes;
using MediatR;
using MudBlazor.ExampleApp.Domain.Entities;
using MudBlazor.ExampleApp.Domain.Repositories;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "2.0")]

namespace MudBlazor.ExampleApp.Application.Invoices.CreateInvoice
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
                InvoiceNo = request.InvoiceNo,
                IssuedDate = request.InvoiceDate,
                DueDate = request.DueDate,
                Reference = request.Reference,
                CustomerId = request.CustomerId,
                OrderLines = request.OrderLines
                    .Select(ol => new InvoiceLine
                    {
                        ProductId = ol.ProductId,
                        Units = ol.Units,
                        UnitPrice = ol.UnitPrice,
                        Discount = ol.Discount
                    })
                    .ToList()
            };

            _invoiceRepository.Add(invoice);
            await _invoiceRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
            return invoice.Id;
        }
    }
}