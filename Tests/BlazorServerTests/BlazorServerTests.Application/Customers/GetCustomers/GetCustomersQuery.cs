using BlazorServerTests.Application.Common.Interfaces;
using BlazorServerTests.Application.Common.Pagination;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.QueryModels", Version = "1.0")]

namespace BlazorServerTests.Application.Customers.GetCustomers
{
    public class GetCustomersQuery : IRequest<PagedResult<CustomerDto>>, IQuery
    {
        public GetCustomersQuery(int pageNo, int pageSize, string? orderBy, string? searchTerm, bool? isActive)
        {
            PageNo = pageNo;
            PageSize = pageSize;
            OrderBy = orderBy;
            SearchTerm = searchTerm;
            IsActive = isActive;
        }

        public int PageNo { get; set; }
        public int PageSize { get; set; }
        public string? OrderBy { get; set; }
        public string? SearchTerm { get; set; }
        public bool? IsActive { get; set; }
    }
}