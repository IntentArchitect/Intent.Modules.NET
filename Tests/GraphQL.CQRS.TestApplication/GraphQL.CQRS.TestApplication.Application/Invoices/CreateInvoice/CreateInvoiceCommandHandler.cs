using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using GraphQL.CQRS.TestApplication.Domain.Entities;
using GraphQL.CQRS.TestApplication.Domain.Repositories;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "2.0")]

namespace GraphQL.CQRS.TestApplication.Application.Invoices.CreateInvoice
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
            var newInvoice = new Invoice
            {
                No = request.No,
                Created = request.Created,
                CustomerId = request.CustomerId,
            };

            _invoiceRepository.Add(newInvoice);
            await _invoiceRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
            return newInvoice.Id;
        }
    }
}