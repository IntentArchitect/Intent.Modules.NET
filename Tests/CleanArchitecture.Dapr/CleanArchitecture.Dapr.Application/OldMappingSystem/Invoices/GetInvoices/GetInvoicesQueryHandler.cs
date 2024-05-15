using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using CleanArchitecture.Dapr.Domain.Repositories;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.QueryHandler", Version = "1.0")]

namespace CleanArchitecture.Dapr.Application.OldMappingSystem.Invoices.GetInvoices
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class GetInvoicesQueryHandler : IRequestHandler<GetInvoicesQuery, List<InvoiceDto>>
    {
        private readonly IInvoiceRepository _invoiceRepository;
        private readonly IMapper _mapper;

        [IntentManaged(Mode.Ignore)]
        public GetInvoicesQueryHandler(IInvoiceRepository invoiceRepository, IMapper mapper)
        {
            _invoiceRepository = invoiceRepository;
            _mapper = mapper;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<List<InvoiceDto>> Handle(GetInvoicesQuery request, CancellationToken cancellationToken)
        {
            var invoices = await _invoiceRepository.FindAllAsync(cancellationToken);
            return invoices.MapToInvoiceDtoList(_mapper);
        }
    }
}