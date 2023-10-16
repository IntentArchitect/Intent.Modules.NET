using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Intent.RoslynWeaver.Attributes;
using MediatR;
using TableStorage.Tests.Domain.Common.Exceptions;
using TableStorage.Tests.Domain.Repositories;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.QueryHandler", Version = "1.0")]

namespace TableStorage.Tests.Application.Invoices.GetInvoiceById
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class GetInvoiceByIdQueryHandler : IRequestHandler<GetInvoiceByIdQuery, InvoiceDto>
    {
        private readonly IInvoiceRepository _invoiceRepository;
        private readonly IMapper _mapper;

        [IntentManaged(Mode.Ignore)]
        public GetInvoiceByIdQueryHandler(IInvoiceRepository invoiceRepository, IMapper mapper)
        {
            _invoiceRepository = invoiceRepository;
            _mapper = mapper;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<InvoiceDto> Handle(GetInvoiceByIdQuery request, CancellationToken cancellationToken)
        {
            var invoice = await _invoiceRepository.FindByIdAsync((request.PartitionKey, request.RowKey), cancellationToken);
            if (invoice is null)
            {
                throw new NotFoundException($"Could not find Invoice '({request.PartitionKey}, {request.RowKey})'");
            }

            return invoice.MapToInvoiceDto(_mapper);
        }
    }
}