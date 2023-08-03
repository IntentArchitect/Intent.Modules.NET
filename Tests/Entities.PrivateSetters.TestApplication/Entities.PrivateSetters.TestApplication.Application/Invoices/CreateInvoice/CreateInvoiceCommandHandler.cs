using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Entities.PrivateSetters.TestApplication.Domain.Contracts;
using Entities.PrivateSetters.TestApplication.Domain.Entities;
using Entities.PrivateSetters.TestApplication.Domain.Repositories;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "2.0")]

namespace Entities.PrivateSetters.TestApplication.Application.Invoices.CreateInvoice
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
#warning No supported convention for populating "tags" parameter
            var newInvoice = new Invoice(request.Date, tags: default, request.Lines.Select(CreateLineDataContract));

            _invoiceRepository.Add(newInvoice);
            await _invoiceRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
            return newInvoice.Id;
        }

        [IntentManaged(Mode.Fully)]
        public static LineDataContract CreateLineDataContract(CreateInvoiceLineDataContractDto dto)
        {
            return new LineDataContract(description: dto.Description, quantity: dto.Quantity);
        }
    }
}