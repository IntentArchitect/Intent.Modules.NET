using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using GraphQL.CQRS.TestApplication.Domain.Common.Exceptions;
using GraphQL.CQRS.TestApplication.Domain.Repositories;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "2.0")]

namespace GraphQL.CQRS.TestApplication.Application.Invoices.UpdateInvoice
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class UpdateInvoiceCommandHandler : IRequestHandler<UpdateInvoiceCommand, InvoiceDto>
    {
        private readonly IInvoiceRepository _invoiceRepository;
        private readonly IMapper _mapper;

        [IntentManaged(Mode.Ignore)]
        public UpdateInvoiceCommandHandler(IInvoiceRepository invoiceRepository, IMapper mapper)
        {
            _invoiceRepository = invoiceRepository;
            _mapper = mapper;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<InvoiceDto> Handle(UpdateInvoiceCommand request, CancellationToken cancellationToken)
        {
            var existingInvoice = await _invoiceRepository.FindByIdAsync(request.Id, cancellationToken);

            if (existingInvoice is null)
            {
                throw new NotFoundException($"Could not find Invoice '{request.Id}'");
            }
            existingInvoice.No = request.No;
            existingInvoice.Created = request.Created;
            existingInvoice.CustomerId = request.CustomerId;
            await _invoiceRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
            return existingInvoice.MapToInvoiceDto(_mapper);
        }
    }
}