using Intent.RoslynWeaver.Attributes;
using MediatR;
using MudBlazor.Sample.Domain.Common;
using MudBlazor.Sample.Domain.Common.Exceptions;
using MudBlazor.Sample.Domain.Entities;
using MudBlazor.Sample.Domain.Repositories;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "2.0")]

namespace MudBlazor.Sample.Application.Invoices.UpdateInvoice
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

            invoice.InvoiceNo = request.InvoiceNo;
            invoice.IssuedDate = request.InvoiceDate;
            invoice.DueDate = request.DueDate;
            invoice.Reference = request.Reference;
            invoice.CustomerId = request.CustomerId;
            invoice.OrderLines = UpdateHelper.CreateOrUpdateCollection(invoice.OrderLines, request.OrderLines, (e, d) => e.Id == d.Id, CreateOrUpdateInvoiceLine);
        }

        [IntentManaged(Mode.Fully)]
        private static InvoiceLine CreateOrUpdateInvoiceLine(InvoiceLine? entity, UpdateInvoiceCommandOrderLinesDto dto)
        {
            entity ??= new InvoiceLine();
            entity.Id = dto.Id;
            entity.ProductId = dto.ProductId;
            entity.Units = dto.Units;
            entity.UnitPrice = dto.UnitPrice;
            entity.Discount = dto.Discount;
            return entity;
        }
    }
}