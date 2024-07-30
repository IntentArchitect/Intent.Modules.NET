using System;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading;
using System.Threading.Tasks;
using AdvancedMappingCrud.DbContext.Tests.Application.Common.Interfaces;
using AdvancedMappingCrud.DbContext.Tests.Application.Common.Pagination;
using AutoMapper;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.QueryHandler", Version = "1.0")]

namespace AdvancedMappingCrud.DbContext.Tests.Application.Products.GetProductsPaginatedByNameWithOrder
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class GetProductsPaginatedByNameWithOrderQueryHandler : IRequestHandler<GetProductsPaginatedByNameWithOrderQuery, Common.Pagination.PagedResult<ProductDto>>
    {
        private readonly IApplicationDbContext _dbContext;
        private readonly IMapper _mapper;

        [IntentManaged(Mode.Merge)]
        public GetProductsPaginatedByNameWithOrderQueryHandler(IApplicationDbContext dbContext, IMapper mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<Common.Pagination.PagedResult<ProductDto>> Handle(
            GetProductsPaginatedByNameWithOrderQuery request,
            CancellationToken cancellationToken)
        {
            var products = await _dbContext.Products
    .Where(x => x.Name == request.Name)
                .OrderBy(request.OrderBy)
                .ToPagedListAsync(request.PageNo, request.PageSize, cancellationToken);
            return products.MapToPagedResult(x => x.MapToProductDto(_mapper));
        }
    }
}