using System;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading;
using System.Threading.Tasks;
using AdvancedMappingCrud.DbContext.ProjectTo.Tests.Application.Common.Interfaces;
using AdvancedMappingCrud.DbContext.ProjectTo.Tests.Application.Common.Pagination;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.QueryHandler", Version = "1.0")]

namespace AdvancedMappingCrud.DbContext.ProjectTo.Tests.Application.Products.GetProductsPaginatedByNameWithOrder
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class GetProductsPaginatedByNameWithOrderQueryHandler : IRequestHandler<GetProductsPaginatedByNameWithOrderQuery, Common.Pagination.PagedResult<ProductDto>>
    {
        private readonly IMapper _mapper;
        private readonly IApplicationDbContext _dbContext;

        [IntentManaged(Mode.Merge)]
        public GetProductsPaginatedByNameWithOrderQueryHandler(IMapper mapper, IApplicationDbContext dbContext)
        {
            _mapper = mapper;
            _dbContext = dbContext;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<Common.Pagination.PagedResult<ProductDto>> Handle(
            GetProductsPaginatedByNameWithOrderQuery request,
            CancellationToken cancellationToken)
        {
            var products = await _dbContext.Products
    .Where(x => x.Name == request.Name)
                .ProjectTo<ProductDto>(_mapper.ConfigurationProvider)
                .OrderBy(request.OrderBy)
                .ToPagedListAsync(request.PageNo, request.PageSize, cancellationToken);
            return products.MapToPagedResult();
        }
    }
}