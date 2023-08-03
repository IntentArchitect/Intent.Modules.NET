using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Entities.PrivateSetters.TestApplication.Domain.Common.Exceptions;
using Entities.PrivateSetters.TestApplication.Domain.Contracts;
using Entities.PrivateSetters.TestApplication.Domain.Repositories;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "2.0")]

namespace Entities.PrivateSetters.TestApplication.Application.Invoices.Operation
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class OperationCommandHandler : IRequestHandler<OperationCommand>
    {
        private readonly IInvoiceRepository _invoiceRepository;

        [IntentManaged(Mode.Merge)]
        public OperationCommandHandler(IInvoiceRepository invoiceRepository)
        {
            _invoiceRepository = invoiceRepository;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task Handle(OperationCommand request, CancellationToken cancellationToken)
        {
            var existingInvoice = await _invoiceRepository.FindByIdAsync(request.Id, cancellationToken);
            if (existingInvoice is null)
            {
                throw new NotFoundException($"Could not find Invoice '{request.Id}'");
            }

#warning No supported convention for populating "tags" parameter
            existingInvoice.Operation(request.Date, tags: default, request.Lines.Select(CreateLineDataContract));
        }

        [IntentManaged(Mode.Fully)]
        public static LineDataContract CreateLineDataContract(OperationLineDataContractDto dto)
        {
            return new LineDataContract(description: dto.Description, quantity: dto.Quantity);
        }
    }
}