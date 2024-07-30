using AdvancedMappingCrud.DbContext.Tests.Application.Common.Interfaces;
using AdvancedMappingCrud.DbContext.Tests.Application.Common.Pagination;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.QueryModels", Version = "1.0")]

namespace AdvancedMappingCrud.DbContext.Tests.Application.Products.GetProductsPaginatedByNameOptionalWithOrder
{
    public class GetProductsPaginatedByNameOptionalWithOrderQuery : IRequest<PagedResult<ProductDto>>, IQuery
    {
        public GetProductsPaginatedByNameOptionalWithOrderQuery(string? name, int pageNo, int pageSize, string orderBy)
        {
            Name = name;
            PageNo = pageNo;
            PageSize = pageSize;
            OrderBy = orderBy;
        }

        public string? Name { get; set; }
        public int PageNo { get; set; }
        public int PageSize { get; set; }
        public string OrderBy { get; set; }
    }
}