using Intent.RoslynWeaver.Attributes;
using MediatR;
using MudBlazor.ExampleApp.Domain.Entities;
using MudBlazor.ExampleApp.Domain.Repositories;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "2.0")]

namespace MudBlazor.ExampleApp.Application.Invoices.CreateInvoiceConvoluted
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class CreateInvoiceConvolutedCommandHandler : IRequestHandler<CreateInvoiceConvolutedCommand>
    {
        private readonly IInvoiceRepository _invoiceRepository;

        [IntentManaged(Mode.Merge)]
        public CreateInvoiceConvolutedCommandHandler(IInvoiceRepository invoiceRepository)
        {
            _invoiceRepository = invoiceRepository;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task Handle(CreateInvoiceConvolutedCommand request, CancellationToken cancellationToken)
        {
            var entity = new Invoice
            {
                InvoiceNo = request.Invoice.InvoiceNo,
                IssuedDate = request.Invoice.IssuedDate,
                DueDate = request.Invoice.DueDate,
                Reference = request.Invoice.Reference,
                CustomerId = request.Invoice.CustomerId
            };

            _invoiceRepository.Add(entity);
        }
    }
}