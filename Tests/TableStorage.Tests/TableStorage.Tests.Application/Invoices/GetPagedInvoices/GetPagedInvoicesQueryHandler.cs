using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Intent.RoslynWeaver.Attributes;
using MediatR;
using TableStorage.Tests.Application.Common.Pagination;
using TableStorage.Tests.Domain.Repositories;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.QueryHandler", Version = "1.0")]

namespace TableStorage.Tests.Application.Invoices.GetPagedInvoices
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class GetPagedInvoicesQueryHandler : IRequestHandler<GetPagedInvoicesQuery, CursorPagedResult<InvoiceDto>>
    {
        private readonly IInvoiceRepository _invoiceRepository;
        private readonly IMapper _mapper;

        [IntentManaged(Mode.Merge)]
        public GetPagedInvoicesQueryHandler(IInvoiceRepository invoiceRepository, IMapper mapper)
        {
            _invoiceRepository = invoiceRepository;
            _mapper = mapper;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<CursorPagedResult<InvoiceDto>> Handle(
            GetPagedInvoicesQuery request,
            CancellationToken cancellationToken)
        {
            var invoice = await _invoiceRepository.FindAllAsync(x => x.PartitionKey == request.PartitionKey, request.PageSize, request.CursorToken, cancellationToken);
            return invoice.MapToCursorPagedResult(x => x.MapToInvoiceDto(_mapper));
        }
    }
}