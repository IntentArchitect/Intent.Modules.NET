using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using GraphQL.CQRS.TestApplication.Domain.Repositories;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.QueryHandler", Version = "1.0")]

namespace GraphQL.CQRS.TestApplication.Application.Invoices.GetInvoicesForCustomer
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class GetInvoicesForCustomerQueryHandler : IRequestHandler<GetInvoicesForCustomerQuery, List<InvoiceDto>>
    {
        private readonly IInvoiceRepository _invoiceRepository;
        private readonly IMapper _mapper;

        [IntentManaged(Mode.Ignore)]
        public GetInvoicesForCustomerQueryHandler(IInvoiceRepository invoiceRepository, IMapper mapper)
        {
            _invoiceRepository = invoiceRepository;
            _mapper = mapper;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<List<InvoiceDto>> Handle(
            GetInvoicesForCustomerQuery request,
            CancellationToken cancellationToken)
        {
            var invoices = await _invoiceRepository.FindAllAsync(cancellationToken);
            return invoices.MapToInvoiceDtoList(_mapper);
        }
    }
}