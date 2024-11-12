using System;
using System.Linq;
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

namespace AdvancedMappingCrud.DbContext.ProjectTo.Tests.Application.Products.GetProductsPaginatedByNameOptional
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class GetProductsPaginatedByNameOptionalQueryHandler : IRequestHandler<GetProductsPaginatedByNameOptionalQuery, PagedResult<ProductDto>>
    {
        private readonly IMapper _mapper;
        private readonly IApplicationDbContext _dbContext;

        [IntentManaged(Mode.Merge)]
        public GetProductsPaginatedByNameOptionalQueryHandler(IMapper mapper, IApplicationDbContext dbContext)
        {
            _mapper = mapper;
            _dbContext = dbContext;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<PagedResult<ProductDto>> Handle(
            GetProductsPaginatedByNameOptionalQuery request,
            CancellationToken cancellationToken)
        {
            var queryable = _dbContext.Products.AsQueryable();

            if (request.Name != null)
            {
                queryable = queryable.Where(x => x.Name == request.Name);
            }

            var products = await queryable
                .ProjectTo<ProductDto>(_mapper.ConfigurationProvider)
                .ToPagedListAsync(request.PageNo, request.PageSize, cancellationToken);
            return products.MapToPagedResult();
        }
    }
}