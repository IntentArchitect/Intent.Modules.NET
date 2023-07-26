using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using GraphQL.CQRS.TestApplication.Domain.Common.Exceptions;
using GraphQL.CQRS.TestApplication.Domain.Repositories;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "2.0")]

namespace GraphQL.CQRS.TestApplication.Application.Invoices.DeleteInvoice
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class DeleteInvoiceCommandHandler : IRequestHandler<DeleteInvoiceCommand, InvoiceDto>
    {
        private readonly IInvoiceRepository _invoiceRepository;
        private readonly IMapper _mapper;

        [IntentManaged(Mode.Ignore)]
        public DeleteInvoiceCommandHandler(IInvoiceRepository invoiceRepository, IMapper mapper)
        {
            _invoiceRepository = invoiceRepository;
            _mapper = mapper;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<InvoiceDto> Handle(DeleteInvoiceCommand request, CancellationToken cancellationToken)
        {
            var existingInvoice = await _invoiceRepository.FindByIdAsync(request.Id, cancellationToken);

            if (existingInvoice is null)
            {
                throw new NotFoundException($"Could not find Invoice '{request.Id}' ");
            }
            _invoiceRepository.Remove(existingInvoice);
            return existingInvoice.MapToInvoiceDto(_mapper);
        }
    }
}