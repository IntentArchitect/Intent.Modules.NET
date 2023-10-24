using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Intent.RoslynWeaver.Attributes;
using MediatR;
using TableStorage.Tests.Domain.Common.Exceptions;
using TableStorage.Tests.Domain.Repositories;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "2.0")]

namespace TableStorage.Tests.Application.Invoices.UpdateInvoice
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
            var existingInvoice = await _invoiceRepository.FindByIdAsync((request.PartitionKey, request.RowKey), cancellationToken);
            if (existingInvoice is null)
            {
                throw new NotFoundException($"Could not find Invoice '({request.PartitionKey}, {request.RowKey})'");
            }

            existingInvoice.PartitionKey = request.PartitionKey;
            existingInvoice.RowKey = request.RowKey;
            existingInvoice.IssuedData = request.IssuedData;
            existingInvoice.OrderPartitionKey = request.OrderPartitionKey;
            existingInvoice.OrderRowKey = request.OrderRowKey;

            _invoiceRepository.Update(existingInvoice);
        }
    }
}