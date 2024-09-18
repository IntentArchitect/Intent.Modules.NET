using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using CosmosDB.Domain.Common.Exceptions;
using CosmosDB.Domain.Entities;
using CosmosDB.Domain.Repositories;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.QueryHandler", Version = "1.0")]

namespace CosmosDB.Application.Invoices.GetInvoiceLineItemById
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class GetInvoiceLineItemByIdQueryHandler : IRequestHandler<GetInvoiceLineItemByIdQuery, InvoiceLineItemDto>
    {
        private readonly IInvoiceRepository _invoiceRepository;
        private readonly IMapper _mapper;

        [IntentManaged(Mode.Ignore)]
        public GetInvoiceLineItemByIdQueryHandler(IInvoiceRepository invoiceRepository, IMapper mapper)
        {
            _invoiceRepository = invoiceRepository;
            _mapper = mapper;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<InvoiceLineItemDto> Handle(
            GetInvoiceLineItemByIdQuery request,
            CancellationToken cancellationToken)
        {
            var aggregateRoot = await _invoiceRepository.FindByIdAsync(request.InvoiceId, cancellationToken);
            if (aggregateRoot is null)
            {
                throw new NotFoundException($"{nameof(Invoice)} of Id '{request.InvoiceId}' could not be found");
            }

            var element = aggregateRoot.LineItems.FirstOrDefault(p => p.Id == request.Id);
            if (element is null)
            {
                throw new NotFoundException($"Could not find LineItem '{request.Id}'");
            }

            return element.MapToInvoiceLineItemDto(_mapper);
        }
    }
}