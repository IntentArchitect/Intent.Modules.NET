using GrpcServer.Application.Common.Interfaces;
using GrpcServer.Application.Common.Pagination;
using GrpcServer.Application.Common.Security;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.QueryModels", Version = "1.0")]

namespace GrpcServer.Application.Products.GetProductsPaged
{
    [Authorize]
    public class GetProductsPagedQuery : IRequest<PagedResult<ProductDto>>, IQuery
    {
        public GetProductsPagedQuery(int pageNo, int pageSize, string? orderBy)
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