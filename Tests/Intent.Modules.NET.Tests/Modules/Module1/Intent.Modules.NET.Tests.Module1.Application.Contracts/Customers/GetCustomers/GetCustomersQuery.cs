using Intent.Modules.NET.Tests.Application.Core.Common.Interfaces;
using Intent.Modules.NET.Tests.Application.Core.Common.Pagination;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.QueryModels", Version = "1.0")]

namespace Intent.Modules.NET.Tests.Module1.Application.Contracts.Customers.GetCustomers
{
    public class GetCustomersQuery : IRequest<PagedResult<CustomerDto>>, IQuery
    {
        public GetCustomersQuery(int pageNo, int pageSize, string? orderBy)
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