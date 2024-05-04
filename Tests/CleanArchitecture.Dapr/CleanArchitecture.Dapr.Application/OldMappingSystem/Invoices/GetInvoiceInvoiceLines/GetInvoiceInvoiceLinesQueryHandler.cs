using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using CleanArchitecture.Dapr.Domain.Entities;
using CleanArchitecture.Dapr.Domain.Repositories;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.QueryHandler", Version = "1.0")]

namespace CleanArchitecture.Dapr.Application.OldMappingSystem.Invoices.GetInvoiceInvoiceLines
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class GetInvoiceInvoiceLinesQueryHandler : IRequestHandler<GetInvoiceInvoiceLinesQuery, List<InvoiceInvoiceLineDto>>
    {
        private readonly IInvoiceRepository _invoiceRepository;
        private readonly IMapper _mapper;

        [IntentManaged(Mode.Ignore)]
        public GetInvoiceInvoiceLinesQueryHandler(IInvoiceRepository invoiceRepository, IMapper mapper)
        {
            _invoiceRepository = invoiceRepository;
            _mapper = mapper;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<List<InvoiceInvoiceLineDto>> Handle(
            GetInvoiceInvoiceLinesQuery request,
            CancellationToken cancellationToken)
        {
            var aggregateRoot = await _invoiceRepository.FindByIdAsync(request.InvoiceId, cancellationToken);
            if (aggregateRoot == null)
            {
                throw new InvalidOperationException($"{nameof(Invoice)} of Id '{request.InvoiceId}' could not be found");
            }
            return aggregateRoot.InvoiceLines.MapToInvoiceInvoiceLineDtoList(_mapper);
        }
    }
}