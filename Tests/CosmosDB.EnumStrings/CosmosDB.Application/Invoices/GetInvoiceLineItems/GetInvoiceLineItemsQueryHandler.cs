using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using CosmosDB.Domain.Entities;
using CosmosDB.Domain.Repositories;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.QueryHandler", Version = "1.0")]

namespace CosmosDB.Application.Invoices.GetInvoiceLineItems
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class GetInvoiceLineItemsQueryHandler : IRequestHandler<GetInvoiceLineItemsQuery, List<InvoiceLineItemDto>>
    {
        private readonly IInvoiceRepository _invoiceRepository;
        private readonly IMapper _mapper;

        [IntentManaged(Mode.Ignore)]
        public GetInvoiceLineItemsQueryHandler(IInvoiceRepository invoiceRepository, IMapper mapper)
        {
            _invoiceRepository = invoiceRepository;
            _mapper = mapper;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<List<InvoiceLineItemDto>> Handle(
            GetInvoiceLineItemsQuery request,
            CancellationToken cancellationToken)
        {
            var aggregateRoot = await _invoiceRepository.FindByIdAsync(request.InvoiceId, cancellationToken);
            if (aggregateRoot == null)
            {
                throw new InvalidOperationException($"{nameof(Invoice)} of Id '{request.InvoiceId}' could not be found");
            }
            return aggregateRoot.LineItems.MapToInvoiceLineItemDtoList(_mapper);
        }
    }
}