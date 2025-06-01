using AutoMapper;
using Intent.RoslynWeaver.Attributes;
using MediatR;
using MudBlazor.ExampleApp.Application.Common.Pagination;
using MudBlazor.ExampleApp.Domain.Repositories;
using static System.Linq.Dynamic.Core.DynamicQueryableExtensions;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.QueryHandler", Version = "1.0")]

namespace MudBlazor.ExampleApp.Application.Invoices.GetInvoices
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class GetInvoicesQueryHandler : IRequestHandler<GetInvoicesQuery, PagedResult<InvoiceDto>>
    {
        private readonly IInvoiceRepository _invoiceRepository;
        private readonly IMapper _mapper;

        [IntentManaged(Mode.Merge)]
        public GetInvoicesQueryHandler(IInvoiceRepository invoiceRepository, IMapper mapper)
        {
            _invoiceRepository = invoiceRepository;
            _mapper = mapper;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<PagedResult<InvoiceDto>> Handle(GetInvoicesQuery request, CancellationToken cancellationToken)
        {
            var invoices = await _invoiceRepository.FindAllAsync(request.PageNo, request.PageSize, queryOptions => queryOptions.OrderBy(request.OrderBy ?? "Id"), cancellationToken);
            return invoices.MapToPagedResult(x => x.MapToInvoiceDto(_mapper));
        }
    }
}