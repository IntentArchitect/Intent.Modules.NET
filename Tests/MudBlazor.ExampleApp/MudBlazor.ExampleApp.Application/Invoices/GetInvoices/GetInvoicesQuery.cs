using Intent.RoslynWeaver.Attributes;
using MediatR;
using MudBlazor.ExampleApp.Application.Common.Interfaces;
using MudBlazor.ExampleApp.Application.Common.Pagination;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.QueryModels", Version = "1.0")]

namespace MudBlazor.ExampleApp.Application.Invoices.GetInvoices
{
    public class GetInvoicesQuery : IRequest<PagedResult<InvoiceDto>>, IQuery
    {
        public GetInvoicesQuery(int pageNo, int pageSize, string? orderBy)
        {
            PageNo = pageNo;
            PageSize = pageSize;
            OrderBy = orderBy;
        }

        public int PageNo { get; set; }
        public int PageSize { get; set; }
        public string? OrderBy { get; set; }
    }
}