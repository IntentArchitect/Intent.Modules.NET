using AdvancedMapping.Repositories.Mapperly.Tests.Application.Common.Interfaces;
using AdvancedMapping.Repositories.Mapperly.Tests.Application.Common.Pagination;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.QueryModels", Version = "1.0")]

namespace AdvancedMapping.Repositories.Mapperly.Tests.Application.Customers.GetCustomersPagedMapped
{
    public class GetCustomersPagedMappedQuery : IRequest<PagedResult<CustomerSummaryDto>>, IQuery
    {
        public GetCustomersPagedMappedQuery(int pageNo, int pageSize, string? orderBy)
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