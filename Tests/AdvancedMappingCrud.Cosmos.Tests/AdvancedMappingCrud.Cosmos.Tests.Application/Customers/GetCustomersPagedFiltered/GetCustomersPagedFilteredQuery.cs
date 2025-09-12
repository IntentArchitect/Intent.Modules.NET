using AdvancedMappingCrud.Cosmos.Tests.Application.Common.Interfaces;
using AdvancedMappingCrud.Cosmos.Tests.Application.Common.Pagination;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.QueryModels", Version = "1.0")]

namespace AdvancedMappingCrud.Cosmos.Tests.Application.Customers.GetCustomersPagedFiltered
{
    public class GetCustomersPagedFilteredQuery : IRequest<PagedResult<CustomerDto>>, IQuery
    {
        public GetCustomersPagedFilteredQuery(int pageNo, int pageSize, string? orderBy, bool isActive)
        {
            PageNo = pageNo;
            PageSize = pageSize;
            OrderBy = orderBy;
            IsActive = isActive;
        }

        public int PageNo { get; set; }
        public int PageSize { get; set; }
        public string? OrderBy { get; set; }
        public bool IsActive { get; set; }
    }
}