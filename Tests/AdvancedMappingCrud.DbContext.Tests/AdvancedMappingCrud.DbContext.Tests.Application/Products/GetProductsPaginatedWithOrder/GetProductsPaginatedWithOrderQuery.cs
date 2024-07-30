using AdvancedMappingCrud.DbContext.Tests.Application.Common.Interfaces;
using AdvancedMappingCrud.DbContext.Tests.Application.Common.Pagination;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.QueryModels", Version = "1.0")]

namespace AdvancedMappingCrud.DbContext.Tests.Application.Products.GetProductsPaginatedWithOrder
{
    public class GetProductsPaginatedWithOrderQuery : IRequest<PagedResult<ProductDto>>, IQuery
    {
        public GetProductsPaginatedWithOrderQuery(int pageNo, int pageSize, string orderBy)
        {
            PageNo = pageNo;
            PageSize = pageSize;
            OrderBy = orderBy;
        }

        public int PageNo { get; set; }
        public int PageSize { get; set; }
        public string OrderBy { get; set; }
    }
}